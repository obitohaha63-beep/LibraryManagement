using Library.Core;
using Library.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Library.Server.Services;

// Small POC application service: accounts, policy versions and shared lookup tables.
// Authentication and HTTP responses stay in controllers; database work stays here.
public class AdministrationService(AppDbContext db, UserManager<AppUser> users, CirculationService circulation)
{
    public async Task<Page<UserDto>> UsersAsync(string? q, string role, int page, bool manager)
    {
        if (role != Roles.Reader && !manager) throw new BusinessException("FORBIDDEN", "Chỉ quản lý được xem nhân viên.");
        page = Math.Max(1, page);
        var query = from u in db.Users.AsNoTracking() join ur in db.UserRoles on u.Id equals ur.UserId join r in db.Roles on ur.RoleId equals r.Id
                    where r.Name == role && (q == null || u.UserName!.Contains(q) || u.FullName.Contains(q)) orderby u.UserName
                    select new UserDto(u.Id, u.UserName!, u.FullName, r.Name!, u.Active);
        return new(await query.Skip((page - 1) * 20).Take(20).ToListAsync(), await query.CountAsync(), page, 20);
    }
    public async Task<string> CreateUserAsync(UserRequest r, bool manager)
    {
        RequireRole(r.Role, manager);
        await using var tx = await db.Database.BeginTransactionAsync(); await circulation.LockAsync();
        var u = new AppUser { UserName = r.UserName.Trim(), FullName = r.FullName.Trim(), Active = r.Active };
        Check(await users.CreateAsync(u, r.Password)); Check(await users.AddToRoleAsync(u, r.Role)); await tx.CommitAsync(); return u.Id;
    }
    public async Task UpdateUserAsync(string id, UserUpdate r, bool manager, string actorId)
    {
        RequireRole(r.Role, manager);
        await using var tx = await db.Database.BeginTransactionAsync(); await circulation.LockAsync();
        var u = await users.FindByIdAsync(id) ?? throw new BusinessException("NOT_FOUND", "Tài khoản không tồn tại."); var oldRoles = await users.GetRolesAsync(u);
        if (!manager && !oldRoles.Contains(Roles.Reader)) throw new BusinessException("FORBIDDEN", "Chỉ quản lý được sửa nhân viên.");
        if (id == actorId && (!r.Active || r.Role != Roles.Manager)) throw new BusinessException("SELF_UPDATE", "Không được tự khóa hoặc tự hạ quyền quản lý.");
        if (oldRoles.Contains(Roles.Reader) && r.Role != Roles.Reader && await db.Loans.AnyAsync(x => x.ReaderId == id)) throw new BusinessException("READER_HISTORY", "Bạn đọc có lịch sử phải giữ vai trò Reader.");
        u.FullName = r.FullName; u.Active = r.Active; Check(await users.UpdateAsync(u)); Check(await users.RemoveFromRolesAsync(u, oldRoles)); Check(await users.AddToRoleAsync(u, r.Role)); await tx.CommitAsync();
    }
    private static void RequireRole(string role, bool manager)
    {
        if (!(role == Roles.Reader || manager && role is Roles.Librarian or Roles.Manager)) throw new BusinessException("FORBIDDEN", "Không có quyền cấp vai trò này.");
    }
    private static void Check(IdentityResult r) { if (!r.Succeeded) throw new BusinessException("ACCOUNT", string.Join(" ", r.Errors.Select(x => x.Description))); }
    public Task<PolicyVersion> PolicyAsync() => db.Policies.AsNoTracking().OrderByDescending(x => x.Id).FirstAsync();
    public async Task<int> SavePolicyAsync(PolicyRequest r, string actorId)
    {
        await using var tx = await db.Database.BeginTransactionAsync(); await circulation.LockAsync();
        var p = new PolicyVersion { LoanLimit = r.LoanLimit, LoanDays = r.LoanDays, BlockIfOverdue = r.BlockIfOverdue, CreatedBy = actorId };
        db.Policies.Add(p); await db.SaveChangesAsync(); await tx.CommitAsync(); return p.Id;
    }
    public async Task SaveMetadataAsync(string kind, int? id, NamedRequest r)
    {
        var name = r.Name.Trim(); if (name.Length == 0) throw new BusinessException("NAME", "Tên không được để trống.");
        switch (kind)
        {
            case "authors": { var x = id.HasValue ? await db.Authors.FindAsync(id) : new Author(); if (x == null) throw Missing(); x.Name = name; if (!id.HasValue) db.Authors.Add(x); break; }
            case "categories": { var x = id.HasValue ? await db.Categories.FindAsync(id) : new Category(); if (x == null) throw Missing(); x.Name = name; if (!id.HasValue) db.Categories.Add(x); break; }
            case "publishers": { var x = id.HasValue ? await db.Publishers.FindAsync(id) : new Publisher(); if (x == null) throw Missing(); x.Name = name; if (!id.HasValue) db.Publishers.Add(x); break; }
            default: throw Missing();
        }
        await db.SaveChangesAsync();
    }
    private static BusinessException Missing() => new("NOT_FOUND", "Danh mục không tồn tại.");
}
