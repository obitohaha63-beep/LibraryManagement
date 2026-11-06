using Library.Core;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
namespace Library.Tests;
[Collection("SQL Server")]
public class AdministrationTests(SqlFixture f)
{
    [Fact] public async Task ReaderCanOpenOwnLoanButCannotOpenAnotherReadersLoan()
    {
        var (reader, codes) = await f.ScenarioAsync(); var (other, _) = await f.ScenarioAsync(1);
        var response = await f.Librarian.PostAsJsonAsync("/api/loans", new BorrowRequest(reader.Id, [codes[0]], Guid.NewGuid())); response.EnsureSuccessStatusCode();
        var id = (await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>()).GetProperty("loanId").GetInt32();
        using var owner = await f.LoginAsync(reader.UserName); using var stranger = await f.LoginAsync(other.UserName);
        Assert.Contains(codes[0], await owner.GetStringAsync($"/MyLoans/Details/{id}"));
        Assert.Equal(HttpStatusCode.NotFound, (await stranger.GetAsync($"/MyLoans/Details/{id}")).StatusCode);
    }
    [Fact] public async Task DuplicateIsbnAndAbsoluteImagesAreSeparatelyRejected()
    {
        var b = (await f.Manager.GetFromJsonAsync<Page<BookDto>>("/api/books"))!.Items.First();
        var r = new BookRequest { Title = "Sách thử", Isbn = b.Isbn, Year = 2026, ImagePath = "images/books/default.svg", CategoryId = b.CategoryId, PublisherId = b.PublisherId, AuthorIds = b.AuthorIds.ToArray() };
        var duplicate = await f.Librarian.PostAsJsonAsync("/api/books", r);
        Assert.Equal(HttpStatusCode.BadRequest, duplicate.StatusCode); Assert.Contains("ISBN_EXISTS", await duplicate.Content.ReadAsStringAsync());
        r.Isbn = "978" + DateTime.UtcNow.Ticks.ToString()[^10..]; r.ImagePath = "D:/books/a.jpg";
        var absolute = await f.Librarian.PostAsJsonAsync("/api/books", r); Assert.Equal(HttpStatusCode.BadRequest, absolute.StatusCode); Assert.Contains("VALIDATION", await absolute.Content.ReadAsStringAsync());
    }
    [Fact] public async Task RevokedRoleInvalidatesIssuedTokenPermissions()
    {
        var username = "staff_" + Guid.NewGuid().ToString("N")[..8];
        (await f.Manager.PostAsJsonAsync("/api/users", new UserRequest(username, "Staff test", "Library@123", Roles.Manager, true))).EnsureSuccessStatusCode();
        var staff = (await f.Manager.GetFromJsonAsync<Page<UserDto>>($"/api/users?q={username}&role=Manager"))!.Items.Single();
        using var session = await f.LoginAsync(username); (await session.GetAsync("/api/policy")).EnsureSuccessStatusCode();
        (await f.Manager.PutAsJsonAsync($"/api/users/{staff.Id}", new UserUpdate(staff.FullName, Roles.Librarian, true))).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Forbidden, (await session.GetAsync("/api/policy")).StatusCode);
    }
    [Fact] public async Task LibrarianCannotCreateOrEditStaff()
    {
        Assert.Equal(HttpStatusCode.Forbidden, (await f.Librarian.PostAsJsonAsync("/api/users", new UserRequest("illegal", "Illegal", "Library@123", Roles.Manager, true))).StatusCode);
        var manager = (await f.Manager.GetFromJsonAsync<Page<UserDto>>("/api/users?q=manager&role=Manager"))!.Items.Single();
        Assert.Equal(HttpStatusCode.Forbidden, (await f.Librarian.PutAsJsonAsync($"/api/users/{manager.Id}", new UserUpdate("Changed", Roles.Reader, false))).StatusCode);
    }
    [Fact] public async Task CopyStateCannotBypassLoanTransactions()
    {
        var (reader, codes) = await f.ScenarioAsync(); var copy = (await f.Manager.GetFromJsonAsync<Page<CopyDto>>("/api/copies?q=" + codes[0]))!.Items.Single();
        Assert.Equal(HttpStatusCode.BadRequest, (await f.Librarian.PutAsJsonAsync($"/api/copies/{copy.Id}", new CopyUpdate("A", "ON_LOAN"))).StatusCode);
        (await f.Librarian.PostAsJsonAsync("/api/loans", new BorrowRequest(reader.Id, [codes[0]], Guid.NewGuid()))).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.BadRequest, (await f.Librarian.PutAsJsonAsync($"/api/copies/{copy.Id}", new CopyUpdate("A", "AVAILABLE"))).StatusCode);
        (await f.Librarian.PutAsJsonAsync($"/api/copies/{copy.Id}", new CopyUpdate("B2", "ON_LOAN"))).EnsureSuccessStatusCode();
    }
    [Fact] public async Task HiddenBookKeepsHistoryButCannotBeBorrowed()
    {
        var meta = (await f.Manager.GetFromJsonAsync<MetadataDto>("/api/books/metadata"))!;
        var request = new BookRequest { Title = "Sách ngừng hiển thị", Isbn = "978" + DateTime.UtcNow.Ticks.ToString()[^10..], Year = 2026, CategoryId = meta.Categories[0].Id, PublisherId = meta.Publishers[0].Id, AuthorIds = [meta.Authors[0].Id] };
        var create = await f.Librarian.PostAsJsonAsync("/api/books", request); create.EnsureSuccessStatusCode(); var body = await create.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>(); var id = body.GetProperty("id").GetInt32();
        var (reader, _) = await f.ScenarioAsync(1); var code = "HIDE_" + id;
        (await f.Librarian.PostAsJsonAsync("/api/copies", new CopyRequest(id, code, "A"))).EnsureSuccessStatusCode();
        (await f.Librarian.PostAsJsonAsync("/api/loans", new BorrowRequest(reader.Id, [code], Guid.NewGuid()))).EnsureSuccessStatusCode();
        request.Active = false; (await f.Librarian.PutAsJsonAsync($"/api/books/{id}", request)).EnsureSuccessStatusCode();
        using var guest = f.Factory.CreateClient(); Assert.Equal(HttpStatusCode.NotFound, (await guest.GetAsync($"/api/books/{id}")).StatusCode);
        (await f.Librarian.PostAsJsonAsync("/api/loans/return", new ReturnRequest(code))).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.BadRequest, (await f.Librarian.PostAsJsonAsync("/api/loans", new BorrowRequest(reader.Id, [code], Guid.NewGuid()))).StatusCode);
        using var owner = await f.LoginAsync(reader.UserName); Assert.Equal("RETURNED", (await owner.GetFromJsonAsync<Page<LoanDto>>("/api/loans/mine"))!.Items.Single().Status);
    }
    [Fact] public async Task SearchPaginatesMoreThanOnePageAndPreservesFilters()
    {
        var meta = (await f.Manager.GetFromJsonAsync<MetadataDto>("/api/books/metadata"))!; var prefix = "PageTest" + Guid.NewGuid().ToString("N")[..6];
        for (var i = 0; i < 13; i++)
        {
            var r = new BookRequest { Title = prefix + " " + i, Isbn = "979" + DateTime.UtcNow.Ticks.ToString()[^10..], Year = 2026, CategoryId = meta.Categories[0].Id, PublisherId = meta.Publishers[0].Id, AuthorIds = [meta.Authors[0].Id] };
            (await f.Manager.PostAsJsonAsync("/api/books", r)).EnsureSuccessStatusCode();
        }
        var first = (await f.Manager.GetFromJsonAsync<Page<BookDto>>($"/api/books?q={prefix}&page=1&category={meta.Categories[0].Id}"))!;
        var second = (await f.Manager.GetFromJsonAsync<Page<BookDto>>($"/api/books?q={prefix}&page=2&category={meta.Categories[0].Id}"))!;
        Assert.Equal(13, first.Total); Assert.Equal(12, first.Items.Count); Assert.Single(second.Items); Assert.DoesNotContain(second.Items[0].Id, first.Items.Select(x => x.Id));
    }
    [Fact] public async Task MetadataCanBeCreatedEditedAndLinkedToBook()
    {
        var name = "Tác giả " + Guid.NewGuid().ToString("N")[..8]; (await f.Librarian.PostAsJsonAsync("/api/metadata/authors", new NamedRequest(name))).EnsureSuccessStatusCode();
        var meta = (await f.Manager.GetFromJsonAsync<MetadataDto>("/api/books/metadata"))!; var author = meta.Authors.Single(x => x.Name == name);
        (await f.Librarian.PutAsJsonAsync($"/api/metadata/authors/{author.Id}", new NamedRequest(name + " sửa"))).EnsureSuccessStatusCode();
        Assert.Contains((await f.Manager.GetFromJsonAsync<MetadataDto>("/api/books/metadata"))!.Authors, x => x.Id == author.Id && x.Name.EndsWith("sửa"));
    }
    [Fact] public async Task CookieLoginProtectsPostsWithAntiforgery()
    {
        using var browser = f.Factory.CreateClient(new() { AllowAutoRedirect = false });
        var loginPage = await browser.GetStringAsync("/Account/Login"); Assert.Contains("__RequestVerificationToken", loginPage);
        var missingToken = await browser.PostAsync("/Account/Login", new FormUrlEncodedContent(new Dictionary<string,string> { ["UserName"]="SV001", ["Password"]="Library@123" }));
        Assert.Equal(HttpStatusCode.BadRequest, missingToken.StatusCode);
        var token = System.Text.RegularExpressions.Regex.Match(loginPage, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value; Assert.NotEmpty(token);
        var response = await browser.PostAsync("/Account/Login", new FormUrlEncodedContent(new Dictionary<string,string> { ["UserName"]="SV001", ["Password"]="Library@123", ["__RequestVerificationToken"]=token }));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode); (await browser.GetAsync("/MyLoans")).EnsureSuccessStatusCode();
    }
}
