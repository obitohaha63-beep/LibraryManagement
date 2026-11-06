using Library.Core;
using Library.Server.Data;
using Library.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true).AddEnvironmentVariables();
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Library")));
builder.Services.AddIdentity<AppUser, IdentityRole>(o => { o.Password.RequiredLength = 8; o.Lockout.MaxFailedAccessAttempts = 5; }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme, o => o.BearerTokenExpiration = TimeSpan.FromHours(4));
builder.Services.ConfigureApplicationCookie(o => { o.LoginPath = "/Account/Login"; o.Cookie.HttpOnly = true; o.Cookie.SameSite = SameSiteMode.Lax;
    o.Events.OnRedirectToLogin = c => { if (c.Request.Path.StartsWithSegments("/api")) c.Response.StatusCode = 401; else c.Response.Redirect(c.RedirectUri); return Task.CompletedTask; };
    o.Events.OnRedirectToAccessDenied = c => { c.Response.StatusCode = 403; return Task.CompletedTask; }; });
builder.Services.AddAuthorization(o => o.DefaultPolicy = new AuthorizationPolicyBuilder(IdentityConstants.ApplicationScheme, IdentityConstants.BearerScheme).RequireAuthenticatedUser().Build());
builder.Services.AddControllersWithViews(); builder.Services.AddOpenApi();
builder.Services.AddSingleton<IBusinessClock, BusinessClock>(); builder.Services.AddScoped<CatalogService>(); builder.Services.AddScoped<CirculationService>();
builder.Services.AddScoped<AdministrationService>(); builder.Services.AddScoped<CopyService>();
var app = builder.Build();
app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (BusinessException ex) { ctx.Response.StatusCode = ex.Code == "FORBIDDEN" ? 403 : ex.Code == "NOT_FOUND" ? 404 : 400; await ctx.Response.WriteAsJsonAsync(new { code = ex.Code, message = ex.Message }); }
    catch (Exception ex) { app.Logger.LogError(ex, "Request failed: {Path}", ctx.Request.Path); ctx.Response.StatusCode = ex is DbUpdateException or SqlException ? 409 : 500; await ctx.Response.WriteAsJsonAsync(new { code = "CONFLICT", message = "Không thể hoàn tất. Dữ liệu có thể đã thay đổi, vui lòng tải lại và thử lại." }); }
});
if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing")) { app.UseHsts(); app.UseHttpsRedirection(); }
app.UseStaticFiles(); app.UseRouting(); app.UseAuthentication();
// Re-read active state and roles so an issued session cannot retain revoked access.
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Headers.Authorization.ToString().StartsWith("Bearer ")) { var r = await ctx.AuthenticateAsync(IdentityConstants.BearerScheme); if (r.Succeeded) ctx.User = r.Principal!; }
    if (ctx.User.Identity?.IsAuthenticated == true)
    {
        var users = ctx.RequestServices.GetRequiredService<UserManager<AppUser>>(); var user = await users.GetUserAsync(ctx.User);
        if (user?.Active != true) { await ctx.SignOutAsync(IdentityConstants.ApplicationScheme); ctx.Response.StatusCode = 401; return; }
        var identity = (ClaimsIdentity)ctx.User.Identity;
        foreach (var role in identity.FindAll(identity.RoleClaimType).ToList()) identity.RemoveClaim(role);
        foreach (var role in await users.GetRolesAsync(user)) identity.AddClaim(new Claim(identity.RoleClaimType, role));
    }
    await next();
});
app.UseAuthorization(); app.MapControllers(); app.MapControllerRoute("default", "{controller=Catalog}/{action=Index}/{id?}");
app.MapOpenApi(); app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
if (!app.Environment.IsEnvironment("Testing")) { using var scope = app.Services.CreateScope(); await SeedData.InitializeAsync(scope.ServiceProvider); }
app.Run();
public partial class Program { }
