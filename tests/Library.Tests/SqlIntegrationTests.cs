using Library.Core;
using Library.Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
namespace Library.Tests;
[Collection("SQL Server")]
public class SqlIntegrationTests(SqlFixture f)
{
    private Task<HttpResponseMessage> Borrow(UserDto reader, string[] codes, Guid? key = null, HttpClient? client = null) => (client ?? f.Librarian).PostAsJsonAsync("/api/loans", new BorrowRequest(reader.Id, codes, key ?? Guid.NewGuid()));
    [Fact] public async Task AC03_11_14_WebApiShareBorrowAndPartialReturn()
    {
        var (reader, codes) = await f.ScenarioAsync(); using var webReader = await f.LoginAsync(reader.UserName);
        var before = (await f.Manager.GetFromJsonAsync<Page<BookDto>>("/api/books"))!.Items.First();
        (await Borrow(reader, codes[..2])).EnsureSuccessStatusCode();
        var mine = (await webReader.GetFromJsonAsync<Page<LoanDto>>("/api/loans/mine?active=true"))!; Assert.Equal(2, mine.Total);
        Assert.All(mine.Items, x => Assert.Equal(new BusinessClock().Today.AddDays(14), x.DueDate));
        var after = (await f.Manager.GetFromJsonAsync<BookDto>($"/api/books/{before.Id}"))!; Assert.Equal(before.Available - 2, after.Available);
        (await f.Librarian.PostAsJsonAsync("/api/loans/return", new ReturnRequest(codes[0]))).EnsureSuccessStatusCode();
        Assert.Equal(1, (await webReader.GetFromJsonAsync<Page<LoanDto>>("/api/loans/mine?active=true"))!.Total);
        Assert.Contains("Những trang sách", await (await webReader.GetAsync("/MyLoans")).Content.ReadAsStringAsync());
        await f.WithDbAsync(async db => { var item = await db.LoanItems.Include(x => x.BookCopy).SingleAsync(x => x.BookCopy.Code == codes[0]); Assert.Equal("RETURNED", item.Status); Assert.NotNull(item.ReturnedAt); Assert.Equal("AVAILABLE", item.BookCopy.Status); Assert.NotNull(item.ReturnedBy); });
    }
    [Fact] public async Task AC13_ExactlyOneConcurrentBorrowWins()
    {
        var (a, codes) = await f.ScenarioAsync(); var (b, _) = await f.ScenarioAsync(1);
        var results = await Task.WhenAll(Borrow(a, [codes[0]]), Borrow(b, [codes[0]], client: f.Manager));
        Assert.Single(results, x => x.IsSuccessStatusCode); Assert.Single(results, x => x.StatusCode == HttpStatusCode.BadRequest);
        await f.WithDbAsync(async db => Assert.Equal(1, await db.LoanItems.CountAsync(x => x.BookCopy.Code == codes[0] && x.Status == "ON_LOAN")));
    }
    [Fact] public async Task AC12_ConcurrentLoansCannotExceedReaderLimit()
    {
        var (reader, codes) = await f.ScenarioAsync(); var results = await Task.WhenAll(Borrow(reader, codes[..2]), Borrow(reader, codes[2..], client: f.Manager));
        Assert.Single(results, x => x.IsSuccessStatusCode);
        await f.WithDbAsync(async db => Assert.Equal(2, await db.LoanItems.CountAsync(x => x.Loan.ReaderId == reader.Id)));
    }
    [Fact] public async Task AC19_OneUnavailableCopyRollsBackWholeLoan()
    {
        var (reader, codes) = await f.ScenarioAsync(); (await Borrow(reader, [codes[0]])).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.BadRequest, (await Borrow(reader, codes[..2])).StatusCode);
        await f.WithDbAsync(async db => { Assert.Equal(1, await db.Loans.CountAsync(x => x.ReaderId == reader.Id)); Assert.Equal("AVAILABLE", (await db.Copies.SingleAsync(x => x.Code == codes[1])).Status); });
    }
    [Fact] public async Task AC20_ConcurrentRetryReturnsSameLoan()
    {
        var (reader, codes) = await f.ScenarioAsync(); var key = Guid.NewGuid(); var results = await Task.WhenAll(Borrow(reader, codes[..1], key), Borrow(reader, codes[..1], key));
        Assert.All(results, x => Assert.True(x.IsSuccessStatusCode));
        var a = await results[0].Content.ReadFromJsonAsync<JsonElement>(); var b = await results[1].Content.ReadFromJsonAsync<JsonElement>(); Assert.Equal(a.GetProperty("loanId").GetInt32(), b.GetProperty("loanId").GetInt32());
        Assert.Equal(HttpStatusCode.BadRequest, (await Borrow(reader, codes[1..2], key)).StatusCode);
    }
    [Fact] public async Task AC15_ReturnTwiceIsRejected()
    {
        var (reader, codes) = await f.ScenarioAsync(); (await Borrow(reader, codes[..1])).EnsureSuccessStatusCode();
        (await f.Librarian.PostAsJsonAsync("/api/loans/return", new ReturnRequest(codes[0]))).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.BadRequest, (await f.Librarian.PostAsJsonAsync("/api/loans/return", new ReturnRequest(codes[0]))).StatusCode);
    }
    [Fact] public async Task AC15_FailureAfterDatabaseWriteRollsBackReturn()
    {
        var (reader, codes) = await f.ScenarioAsync(); (await Borrow(reader, codes[..1])).EnsureSuccessStatusCode(); f.Failure.FailAfterSave = 1;
        Assert.Equal(HttpStatusCode.InternalServerError, (await f.Librarian.PostAsJsonAsync("/api/loans/return", new ReturnRequest(codes[0]))).StatusCode);
        await f.WithDbAsync(async db => { var item = await db.LoanItems.Include(x => x.BookCopy).SingleAsync(x => x.BookCopy.Code == codes[0]); Assert.Equal("ON_LOAN", item.Status); Assert.Null(item.ReturnedAt); Assert.Equal("ON_LOAN", item.BookCopy.Status); });
    }
    [Fact] public async Task AC19_FailureAfterDatabaseWriteRollsBackBorrow()
    {
        var (reader, codes) = await f.ScenarioAsync(); f.Failure.FailAfterSave = 1;
        Assert.Equal(HttpStatusCode.InternalServerError, (await Borrow(reader, codes[..2])).StatusCode);
        await f.WithDbAsync(async db => { Assert.False(await db.Loans.AnyAsync(x => x.ReaderId == reader.Id)); Assert.All(await db.Copies.Where(x => codes.Contains(x.Code)).ToListAsync(), x => Assert.Equal("AVAILABLE", x.Status)); });
    }
    [Fact] public async Task AC04_05_18_ServerEnforcesPermissionsAndOwnership()
    {
        var (reader, codes) = await f.ScenarioAsync(); using var client = await f.LoginAsync(reader.UserName); (await Borrow(reader, codes[..1])).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/policy")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/loans?readerId=anything")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await f.Librarian.PostAsJsonAsync("/api/policy", new PolicyRequest(3, 21, true))).StatusCode);
        var mine = (await client.GetFromJsonAsync<Page<LoanDto>>("/api/loans/mine?readerId=other"))!; Assert.All(mine.Items, x => Assert.Equal(reader.Id, x.ReaderId));
        using var anonymous = f.Factory.CreateClient(); Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.PostAsJsonAsync("/api/auth/login", new LoginRequest(reader.UserName, "wrong"))).StatusCode);
        (await f.Manager.PutAsJsonAsync($"/api/users/{reader.Id}", new UserUpdate(reader.FullName, Roles.Reader, false))).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/loans/mine")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Borrow(reader, codes[1..2])).StatusCode);
    }
    [Fact] public async Task AC06_16_OverdueDatesAndBlockingAgree()
    {
        var (reader, codes) = await f.ScenarioAsync(); (await Borrow(reader, codes[..1])).EnsureSuccessStatusCode();
        await f.WithDbAsync(async db => { var item = await db.LoanItems.SingleAsync(x => x.Loan.ReaderId == reader.Id); item.DueDate = new BusinessClock().Today.AddDays(-2); await db.SaveChangesAsync(); });
        using var client = await f.LoginAsync(reader.UserName);
        Assert.Equal(2, (await client.GetFromJsonAsync<Page<LoanDto>>("/api/loans/mine"))!.Items.Single().DaysOverdue);
        Assert.Equal(2, (await f.Librarian.GetFromJsonAsync<Page<LoanDto>>($"/api/loans?readerId={reader.Id}&overdue=true"))!.Items.Single().DaysOverdue);
        Assert.Equal(HttpStatusCode.BadRequest, (await Borrow(reader, codes[1..2])).StatusCode);
    }
    [Fact] public async Task AC17_PolicyVersionDoesNotChangeSavedDueDates()
    {
        var (reader, codes) = await f.ScenarioAsync(); (await Borrow(reader, codes[..1])).EnsureSuccessStatusCode();
        try
        {
            (await f.Manager.PostAsJsonAsync("/api/policy", new PolicyRequest(3, 21, true))).EnsureSuccessStatusCode(); (await Borrow(reader, codes[1..2])).EnsureSuccessStatusCode();
            await f.WithDbAsync(async db => { var items = await db.LoanItems.Where(x => x.Loan.ReaderId == reader.Id).OrderBy(x => x.Id).ToListAsync(); Assert.Equal(new BusinessClock().Today.AddDays(14), items[0].DueDate); Assert.Equal(new BusinessClock().Today.AddDays(21), items[1].DueDate); });
        }
        finally { (await f.Manager.PostAsJsonAsync("/api/policy", new PolicyRequest(3, 14, true))).EnsureSuccessStatusCode(); }
    }
    [Fact] public async Task AC01_02_07_SearchAndPagingAndDateValidation()
    {
        var page = (await f.Manager.GetFromJsonAsync<Page<BookDto>>("/api/books?q=l%E1%BA%ACp%20TR%C3%ACnh"))!; Assert.Contains(page.Items, x => x.Title == "Lập trình C#"); Assert.True(page.Items.Count <= 12);
        Assert.Empty((await f.Manager.GetFromJsonAsync<Page<BookDto>>("/api/books?q=NO_SUCH_BOOK"))!.Items);
        Assert.Equal(HttpStatusCode.BadRequest, (await f.Librarian.GetAsync("/api/loans?from=2026-10-24&to=2026-10-01")).StatusCode);
    }
    [Fact] public async Task AC08_09_ValidationPreventsDuplicateAndAbsolutePaths()
    {
        var (_, codes) = await f.ScenarioAsync(); var b = (await f.Manager.GetFromJsonAsync<Page<BookDto>>("/api/books"))!.Items.First();
        Assert.Equal(HttpStatusCode.BadRequest, (await f.Librarian.PostAsJsonAsync("/api/copies", new CopyRequest(b.Id, codes[0], "Kệ"))).StatusCode);
        var r = new BookRequest { Title = "", Isbn = b.Isbn, Year = 2026, AuthorIds = b.AuthorIds.ToArray(), CategoryId = b.CategoryId, PublisherId = b.PublisherId, ImagePath = "C:/books/a.jpg" };
        Assert.Equal(HttpStatusCode.BadRequest, (await f.Librarian.PostAsJsonAsync("/api/books", r)).StatusCode);
        r.Title = "Sách mới"; Assert.Equal(HttpStatusCode.BadRequest, (await f.Librarian.PostAsJsonAsync("/api/books", r)).StatusCode);
    }
    [Fact] public async Task BR10_DatabaseIndexRejectsDuplicateActiveLoan()
    {
        var (reader, codes) = await f.ScenarioAsync(); (await Borrow(reader, codes[..1])).EnsureSuccessStatusCode();
        await f.WithDbAsync(async db => { var item = await db.LoanItems.SingleAsync(x => x.Loan.ReaderId == reader.Id); db.LoanItems.Add(new LoanItem { LoanId = item.LoanId, BookCopyId = item.BookCopyId, DueDate = item.DueDate }); await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync()); });
    }
}
