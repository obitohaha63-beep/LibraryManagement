using Library.Core;
using Library.Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Library.Server.Services;

// One SQL transaction lock keeps the POC easy to reason about, including the per-reader limit.
// The filtered unique index remains the final defence against double borrowing.
public class CirculationService(AppDbContext db, IBusinessClock clock)
{
    public async Task LockAsync()
    {
        await db.Database.ExecuteSqlRawAsync("DECLARE @result int; EXEC @result = sp_getapplock @Resource = 'Library.Circulation', @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 10000; IF @result < 0 THROW 51000, 'Circulation lock timeout', 1;");
    }
    public async Task<int> BorrowAsync(BorrowRequest request, string staffId)
    {
        var codes = LoanRules.NormalizeCodes(request.CopyCodes);
        if (request.RequestKey == Guid.Empty) throw new BusinessException("REQUEST_KEY", "Thiếu mã xác nhận giao dịch.");
        var fingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.ReaderId + ":" + string.Join(",", codes))));
        await using var tx = await db.Database.BeginTransactionAsync();
        await LockAsync();
        var previous = await db.Loans.SingleOrDefaultAsync(x => x.StaffId == staffId && x.RequestKey == request.RequestKey);
        if (previous != null)
        {
            if (previous.RequestFingerprint != fingerprint) throw new BusinessException("REQUEST_REUSED", "Mã xác nhận đã dùng cho nội dung khác.");
            await tx.CommitAsync();
            return previous.Id;
        }
        var reader = await db.Users.FindAsync(request.ReaderId);
        var readerRole = await db.Roles.SingleAsync(x => x.Name == Roles.Reader);
        var isReader = await db.UserRoles.AnyAsync(x => x.UserId == request.ReaderId && x.RoleId == readerRole.Id);
        var policy = await db.Policies.OrderByDescending(x => x.Id).FirstAsync();
        var activeLoans = db.LoanItems.Where(x => x.Loan.ReaderId == request.ReaderId && x.Status == "ON_LOAN");
        LoanRules.CheckEligibility(reader?.Active == true && isReader, await activeLoans.CountAsync(), codes.Length,
            await activeLoans.AnyAsync(x => x.DueDate < clock.Today), policy);
        var copies = await db.Copies.Include(x => x.Book).Where(x => codes.Contains(x.Code)).ToListAsync();
        if (copies.Count != codes.Length || copies.Any(x => x.Status != "AVAILABLE" || !x.Book.Active))
            throw new BusinessException("COPY_UNAVAILABLE", "Có bản sao không tồn tại, ngừng phục vụ hoặc đang được mượn.");
        var loan = new Loan { ReaderId = request.ReaderId, StaffId = staffId, RequestKey = request.RequestKey,
            RequestFingerprint = fingerprint, PolicyVersionId = policy.Id, CreatedAt = clock.UtcNow };
        foreach (var copy in copies)
        {
            copy.Status = "ON_LOAN";
            loan.Items.Add(new LoanItem { BookCopyId = copy.Id, DueDate = clock.Today.AddDays(policy.LoanDays) });
        }
        db.Loans.Add(loan);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return loan.Id;
    }
    public async Task ReturnAsync(string code, string staffId)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        await LockAsync();
        code = code.Trim().ToUpperInvariant();
        var item = await db.LoanItems.Include(x => x.BookCopy).SingleOrDefaultAsync(x => x.BookCopy.Code == code && x.Status == "ON_LOAN");
        if (item == null) throw new BusinessException("NO_ACTIVE_LOAN", "Bản sao không đang được mượn hoặc đã trả trước đó.");
        item.Status = "RETURNED";
        item.ReturnedAt = clock.UtcNow;
        item.ReturnedBy = staffId;
        item.BookCopy.Status = "AVAILABLE";
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task<Page<LoanDto>> QueryAsync(string? readerId, bool active, bool overdue, DateOnly? from, DateOnly? to, int page = 1, int? loanId = null)
    {
        if (from > to) throw new BusinessException("DATE_RANGE", "Ngày bắt đầu phải trước hoặc bằng ngày kết thúc.");
        page = Math.Max(1, page);
        var q = db.LoanItems.AsNoTracking().AsQueryable();
        if (readerId != null) q = q.Where(x => x.Loan.ReaderId == readerId);
        if (loanId.HasValue) q = q.Where(x => x.LoanId == loanId);
        if (active) q = q.Where(x => x.Status == "ON_LOAN");
        if (overdue) q = q.Where(x => x.Status == "ON_LOAN" && x.DueDate < clock.Today);
        // Loan timestamps are UTC; filters are Vietnamese business dates.
        if (from.HasValue) { var start = from.Value.ToDateTime(TimeOnly.MinValue).AddHours(-7); q = q.Where(x => x.Loan.CreatedAt >= start); }
        if (to.HasValue) { var end = to.Value.AddDays(1).ToDateTime(TimeOnly.MinValue).AddHours(-7); q = q.Where(x => x.Loan.CreatedAt < end); }
        var total = await q.CountAsync();
        var rows = await q.OrderByDescending(x => x.Id).Skip((page - 1) * 20).Take(20)
            .Select(x => new { x.Id, x.LoanId, x.Loan.ReaderId, x.BookCopy.Code, x.BookCopy.Book.Title, x.Loan.CreatedAt, x.DueDate, x.ReturnedAt, x.Status }).ToListAsync();
        var ids = rows.Select(x => x.ReaderId).Distinct().ToArray();
        var names = await db.Users.Where(x => ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.FullName);
        return new(rows.Select(x => new LoanDto(x.Id, x.LoanId, x.ReaderId, names.GetValueOrDefault(x.ReaderId, ""), x.Code, x.Title,
            x.CreatedAt, x.DueDate, x.ReturnedAt, x.Status, LoanRules.DaysOverdue(x.DueDate, clock.Today, x.Status == "RETURNED"))).ToList(), total, page, 20);
    }
}
