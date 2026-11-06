using Library.Core;
using Library.Server.Data;
using Library.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Library.Server.Controllers;
public class CatalogController(CatalogService catalog) : Controller
{
    public async Task<IActionResult> Index(string? q, int? category, int? author, bool available = false, int page = 1)
    {
        ViewBag.Metadata = await catalog.MetadataAsync(); ViewBag.Query = q; ViewBag.Category = category; ViewBag.Author = author; ViewBag.Available = available;
        return View(await catalog.SearchAsync(q, category, author, available, page));
    }
    public async Task<IActionResult> Details(int id) => await catalog.GetAsync(id) is { } b ? View(b) : NotFound();
}
public class AccountController(UserManager<AppUser> users, SignInManager<AppUser> signIn) : Controller
{
    [HttpGet] public IActionResult Login() => View();
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Login(LoginRequest r)
    {
        if (ModelState.IsValid)
        {
            var u = await users.FindByNameAsync(r.UserName);
            if (u?.Active == true && (await signIn.PasswordSignInAsync(u, r.Password, false, true)).Succeeded) return RedirectToAction("Index", "Catalog");
        }
        ModelState.AddModelError("", "Thông tin đăng nhập không hợp lệ."); return View(r);
    }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Logout() { await signIn.SignOutAsync(); return RedirectToAction("Index", "Catalog"); }
}
[Authorize(Roles = Roles.Reader)]
public class MyLoansController(CirculationService circulation) : Controller
{
    public async Task<IActionResult> Details(int id, int page = 1)
    {
        var result = await circulation.QueryAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), false, false, null, null, page, id);
        return result.Total == 0 ? NotFound() : View(result);
    }
    public async Task<IActionResult> Index(bool active = true, DateOnly? from = null, DateOnly? to = null, int page = 1)
    {
        ViewBag.Active = active; ViewBag.From = from; ViewBag.To = to;
        if (from > to) { ViewBag.Error = "Ngày bắt đầu phải trước ngày kết thúc."; return View(new Page<LoanDto>([], 0, 1, 20)); }
        return View(await circulation.QueryAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), active, false, from, to, page));
    }
}
