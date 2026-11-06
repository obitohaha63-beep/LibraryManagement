using Library.Core;
using Library.Server.Data;
using Microsoft.EntityFrameworkCore;
namespace Library.Server.Services;
public class CopyService(AppDbContext db, CirculationService circulation)
{
    public async Task<Page<CopyDto>> QueryAsync(int? bookId, string? q, int page)
    {
        page = Math.Max(1, page); var query = db.Copies.AsNoTracking().Where(x => (!bookId.HasValue || x.BookId == bookId) && (q == null || x.Code.Contains(q)));
        return new(await query.OrderBy(x => x.Code).Skip((page - 1) * 20).Take(20).Select(x => new CopyDto(x.Id, x.BookId, x.Code, x.Location, x.Status)).ToListAsync(), await query.CountAsync(), page, 20);
    }
    public async Task<int> CreateAsync(CopyRequest r)
    {
        if (!await db.Books.AnyAsync(x => x.Id == r.BookId)) throw new BusinessException("BOOK", "Đầu sách không tồn tại.");
        var code = r.Code.Trim().ToUpperInvariant();
        if (await db.Copies.AnyAsync(x => x.Code == code)) throw new BusinessException("CODE_EXISTS", "Mã bản sao đã tồn tại, không được tái sử dụng.");
        var copy = new BookCopy { BookId = r.BookId, Code = code, Location = r.Location }; db.Copies.Add(copy); await db.SaveChangesAsync(); return copy.Id;
    }
    public async Task UpdateAsync(int id, CopyUpdate r)
    {
        await using var tx = await db.Database.BeginTransactionAsync(); await circulation.LockAsync();
        var copy = await db.Copies.FindAsync(id) ?? throw new BusinessException("NOT_FOUND", "Bản sao không tồn tại.");
        if (copy.Status != r.Status && (copy.Status == "ON_LOAN" || r.Status is not ("AVAILABLE" or "INACTIVE"))) throw new BusinessException("COPY_STATE", "Chỉ giao dịch mượn/trả được thay đổi trạng thái đang mượn.");
        copy.Location = r.Location; copy.Status = r.Status; await db.SaveChangesAsync(); await tx.CommitAsync();
    }
}
