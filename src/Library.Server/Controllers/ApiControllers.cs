using Library.Core;
using Library.Server.Data;
using Library.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace Library.Server.Controllers;
[ApiController, Authorize(Roles = Roles.Staff), Route("api/book-images")]
public class BookImagesApiController(IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost, RequestSizeLimit(5_000_000)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0 || file.Length > 5_000_000)
            return BadRequest(new { message = "Chọn ảnh hợp lệ có dung lượng tối đa 5 MB." });
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowed = new Dictionary<string, string>
        {
            [".jpg"] = "image/jpeg", [".jpeg"] = "image/jpeg", [".png"] = "image/png",
            [".gif"] = "image/gif", [".webp"] = "image/webp"
        };
        if (!allowed.TryGetValue(extension, out var contentType) || file.ContentType != contentType)
            return BadRequest(new { message = "Chỉ nhận ảnh JPG, PNG, GIF hoặc WebP." });
        var folder = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "images", "books");
        Directory.CreateDirectory(folder);
        var safeName = $"{Guid.NewGuid():N}{extension}";
        var destination = Path.Combine(folder, safeName);
        await using (var output = System.IO.File.Create(destination))
            await file.CopyToAsync(output);
        return Ok(new { imagePath = $"images/books/{safeName}" });
    }
}
[ApiController, Route("api/auth")]
public class AuthApiController(UserManager<AppUser> users, SignInManager<AppUser> signIn) : ControllerBase
{
    [HttpPost("login")] public async Task<IActionResult> Login(LoginRequest r)
    {
        var user = await users.FindByNameAsync(r.UserName);
        if (user == null || !user.Active || !(await signIn.CheckPasswordSignInAsync(user, r.Password, true)).Succeeded) return Unauthorized(new { message = "Thông tin đăng nhập không hợp lệ." });
        return SignIn(await signIn.CreateUserPrincipalAsync(user), new AuthenticationProperties(), IdentityConstants.BearerScheme);
    }
    [Authorize, HttpGet("me")] public async Task<object> Me() { var u = (await users.GetUserAsync(User))!; return new UserDto(u.Id, u.UserName!, u.FullName, (await users.GetRolesAsync(u)).Single(), u.Active); }
}
[ApiController, Route("api/books")]
public class BooksApiController(CatalogService catalog) : ControllerBase
{
    [HttpGet] public Task<Page<BookDto>> Search(string? q, int? category, int? author, bool available = false, int page = 1) => catalog.SearchAsync(q, category, author, available, page, User.IsInRole(Roles.Librarian) || User.IsInRole(Roles.Manager));
    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id) => await catalog.GetAsync(id, User.IsInRole(Roles.Librarian) || User.IsInRole(Roles.Manager)) is { } b ? Ok(b) : NotFound();
    [HttpGet("metadata")] public Task<MetadataDto> Metadata() => catalog.MetadataAsync();
    [Authorize(Roles = Roles.Staff), HttpPost] public async Task<object> Create(BookRequest r) => new { id = await catalog.SaveAsync(null, r) };
    [Authorize(Roles = Roles.Staff), HttpPut("{id:int}")] public async Task<object> Update(int id, BookRequest r) => new { id = await catalog.SaveAsync(id, r) };
    [Authorize(Roles = Roles.Manager), HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) { await catalog.DeleteAsync(id); return NoContent(); }
}
[ApiController, Authorize(Roles = Roles.Staff), Route("api/copies")]
public class CopiesApiController(CopyService copies) : ControllerBase
{
    [HttpGet] public Task<Page<CopyDto>> Get(int? bookId, string? q, int page = 1) => copies.QueryAsync(bookId, q, page);
    [HttpPost] public async Task<object> Create(CopyRequest r) => new { id = await copies.CreateAsync(r) };
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, CopyUpdate r) { await copies.UpdateAsync(id, r); return NoContent(); }
}
[ApiController, Authorize, Route("api/loans")]
public class LoansApiController(CirculationService circulation) : ControllerBase
{
    [Authorize(Roles = Roles.Staff), HttpPost] public async Task<object> Borrow(BorrowRequest r) => new { loanId = await circulation.BorrowAsync(r, User.FindFirstValue(ClaimTypes.NameIdentifier)!) };
    [Authorize(Roles = Roles.Staff), HttpPost("return")] public async Task<IActionResult> Return(ReturnRequest r) { await circulation.ReturnAsync(r.CopyCode, User.FindFirstValue(ClaimTypes.NameIdentifier)!); return NoContent(); }
    [Authorize(Roles = Roles.Reader), HttpGet("mine")] public Task<Page<LoanDto>> Mine(bool active = false, DateOnly? from = null, DateOnly? to = null, int page = 1) => circulation.QueryAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), active, false, from, to, page);
    [Authorize(Roles = Roles.Staff), HttpGet] public Task<Page<LoanDto>> Get(string? readerId, bool active = false, bool overdue = false, DateOnly? from = null, DateOnly? to = null, int page = 1) => circulation.QueryAsync(readerId, active, overdue, from, to, page);
}
[ApiController, Authorize(Roles = Roles.Staff), Route("api/users")]
public class UsersApiController(AdministrationService admin) : ControllerBase
{
    [HttpGet] public Task<Page<UserDto>> Get(string? q, string role = Roles.Reader, int page = 1) => admin.UsersAsync(q, role, page, User.IsInRole(Roles.Manager));
    [HttpPost] public async Task<object> Create(UserRequest r) => new { id = await admin.CreateUserAsync(r, User.IsInRole(Roles.Manager)) };
    [HttpPut("{id}")] public async Task<IActionResult> Update(string id, UserUpdate r) { await admin.UpdateUserAsync(id, r, User.IsInRole(Roles.Manager), User.FindFirstValue(ClaimTypes.NameIdentifier)!); return NoContent(); }
}
[ApiController, Authorize(Roles = Roles.Manager), Route("api/policy")]
public class PolicyApiController(AdministrationService admin) : ControllerBase
{
    [HttpGet] public Task<PolicyVersion> Get() => admin.PolicyAsync();
    [HttpPost] public async Task<object> Save(PolicyRequest r) => new { id = await admin.SavePolicyAsync(r, User.FindFirstValue(ClaimTypes.NameIdentifier)!) };
}
[ApiController, Authorize(Roles = Roles.Staff), Route("api/metadata")]
public class MetadataApiController(AdministrationService admin) : ControllerBase
{
    [HttpPost("{kind}")] public async Task<IActionResult> Create(string kind, NamedRequest r) { await admin.SaveMetadataAsync(kind, null, r); return Ok(); }
    [HttpPut("{kind}/{id:int}")] public async Task<IActionResult> Update(string kind, int id, NamedRequest r) { await admin.SaveMetadataAsync(kind, id, r); return Ok(); }
}
