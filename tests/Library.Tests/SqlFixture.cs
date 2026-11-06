using Library.Server.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Library.Core;
namespace Library.Tests;
public class FailureSwitch : SaveChangesInterceptor
{
    public int FailAfterSave;
    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if (Interlocked.Exchange(ref FailAfterSave, 0) == 1) throw new InvalidOperationException("Simulated failure after database write and before commit.");
        return ValueTask.FromResult(result);
    }
}
public class TestFactory(string connection, FailureSwitch failure) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(s => { s.RemoveAll<AppDbContext>(); s.RemoveAll<DbContextOptions<AppDbContext>>(); s.AddDbContext<AppDbContext>(o => o.UseSqlServer(connection).AddInterceptors(failure)); });
    }
}
public class SqlFixture : IAsyncLifetime
{
    public readonly string DatabaseName = "LibraryPoc_Test_" + Guid.NewGuid().ToString("N");
    public readonly FailureSwitch Failure = new();
    public TestFactory Factory { get; private set; } = null!;
    public HttpClient Manager { get; private set; } = null!;
    public HttpClient Librarian { get; private set; } = null!;
    public async Task InitializeAsync()
    {
        var server = Environment.GetEnvironmentVariable("LIBRARY_TEST_SERVER") ?? ".\\SQLEXPRESS";
        Factory = new($"Server={server};Database={DatabaseName};Trusted_Connection=True;TrustServerCertificate=True", Failure);
        using var scope = Factory.Services.CreateScope(); await SeedData.InitializeAsync(scope.ServiceProvider);
        Manager = await LoginAsync("manager"); Librarian = await LoginAsync("librarian");
    }
    public async Task<HttpClient> LoginAsync(string name)
    {
        var client = Factory.CreateClient(new() { AllowAutoRedirect = false });
        var r = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(name, "Library@123")); r.EnsureSuccessStatusCode();
        var body = await r.Content.ReadFromJsonAsync<JsonElement>(); client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body.GetProperty("accessToken").GetString()); return client;
    }
    public async Task<(UserDto Reader, string[] Codes)> ScenarioAsync(int copies = 4)
    {
        var unique = Guid.NewGuid().ToString("N")[..10]; var username = "test_" + unique;
        var r = await Manager.PostAsJsonAsync("/api/users", new UserRequest(username, "Bạn đọc thử " + unique, "Library@123", Roles.Reader, true)); r.EnsureSuccessStatusCode();
        var users = (await Manager.GetFromJsonAsync<Page<UserDto>>("/api/users?q=" + username))!; var reader = users.Items.Single();
        var codes = Enumerable.Range(1, copies).Select(i => $"T{unique}_{i}".ToUpperInvariant()).ToArray();
        var book = (await Manager.GetFromJsonAsync<Page<BookDto>>("/api/books"))!.Items.First();
        foreach (var code in codes) (await Manager.PostAsJsonAsync("/api/copies", new CopyRequest(book.Id, code, "Kệ test"))).EnsureSuccessStatusCode();
        return (reader, codes);
    }
    public async Task WithDbAsync(Func<AppDbContext, Task> action) { using var scope = Factory.Services.CreateScope(); await action(scope.ServiceProvider.GetRequiredService<AppDbContext>()); }
    public async Task DisposeAsync()
    {
        // Only the unique, task-owned test database is removed; never the demo database.
        if (!DatabaseName.StartsWith("LibraryPoc_Test_", StringComparison.Ordinal)) throw new InvalidOperationException("Unsafe test database name.");
        await WithDbAsync(db => db.Database.EnsureDeletedAsync()); Manager.Dispose(); Librarian.Dispose(); await Factory.DisposeAsync();
    }
}
[CollectionDefinition("SQL Server", DisableParallelization = true)] public class SqlCollection : ICollectionFixture<SqlFixture> { }
