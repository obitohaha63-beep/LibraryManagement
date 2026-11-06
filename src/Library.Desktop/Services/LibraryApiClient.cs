using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Library.Core;
namespace Library.Desktop.Services;
public class LibraryApiClient : IDisposable
{
    private HttpClient http = new();
    public UserDto? CurrentUser { get; private set; }
    public void SetBaseUrl(string url)
    {
        var address = new Uri(url.TrimEnd('/') + "/");
        if (http.BaseAddress == address) return;
        // A new server starts a new session; HttpClient.BaseAddress cannot change after a request.
        http.Dispose();
        http = new HttpClient { BaseAddress = address };
        CurrentUser = null;
    }
    public async Task LoginAsync(string user, string password)
    {
        var response = await http.PostAsJsonAsync("api/auth/login", new LoginRequest(user, password)); await EnsureAsync(response);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", json.GetProperty("accessToken").GetString());
        CurrentUser = await GetAsync<UserDto>("api/auth/me");
        if (CurrentUser.Role == Roles.Reader) { http.DefaultRequestHeaders.Authorization = null; throw new InvalidOperationException("Bạn đọc vui lòng sử dụng Website."); }
    }
    public async Task<T> GetAsync<T>(string path) { using var r = await http.GetAsync(path); await EnsureAsync(r); return (await r.Content.ReadFromJsonAsync<T>())!; }
    public async Task<JsonElement?> SaveAsync(string path, object data, bool update = false)
    {
        Validator.ValidateObject(data, new ValidationContext(data), true);
        using var r = update ? await http.PutAsJsonAsync(path, data) : await http.PostAsJsonAsync(path, data); await EnsureAsync(r);
        var body = await r.Content.ReadAsStringAsync(); return string.IsNullOrWhiteSpace(body) ? null : JsonSerializer.Deserialize<JsonElement>(body);
    }
    public async Task DeleteAsync(string path)
    {
        using var response = await http.DeleteAsync(path);
        await EnsureAsync(response);
    }
    public async Task<string> UploadBookImageAsync(string filePath)
    {
        using var form = new MultipartFormDataContent();
        await using var stream = File.OpenRead(filePath);
        using var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue(Path.GetExtension(filePath).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg", ".png" => "image/png", ".gif" => "image/gif", _ => "image/webp"
        });
        form.Add(content, "file", Path.GetFileName(filePath));
        using var response = await http.PostAsync("api/book-images", form);
        await EnsureAsync(response);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("imagePath").GetString()!;
    }
    private static async Task EnsureAsync(HttpResponseMessage r)
    {
        if (r.IsSuccessStatusCode) return;
        if ((int)r.StatusCode == 401) throw new InvalidOperationException("Phiên không hợp lệ, tài khoản bị khóa hoặc thông tin đăng nhập sai. Vui lòng đăng nhập lại.");
        if ((int)r.StatusCode == 403) throw new InvalidOperationException("Bạn không có quyền thực hiện chức năng này.");
        try { var e = await r.Content.ReadFromJsonAsync<JsonElement>(); if (e.TryGetProperty("message", out var m)) throw new InvalidOperationException(m.GetString());
            if (e.TryGetProperty("errors", out var errors)) throw new InvalidOperationException(string.Join("\n", errors.EnumerateObject().SelectMany(p => p.Value.EnumerateArray().Select(v => v.GetString())))); }
        catch (JsonException) { }
        throw new InvalidOperationException("Không thể hoàn tất yêu cầu. Vui lòng tải lại dữ liệu.");
    }
    public void Dispose() => http.Dispose();
}
public class BorrowWorkflow(LibraryApiClient api)
{
    private Guid key = Guid.NewGuid();
    private string lastPayload = "";
    public async Task<int> BorrowAsync(string readerId, string codes)
    {
        var normalized = codes.Split([',', ';', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        var payload = readerId + ":" + string.Join(",", normalized);
        if (lastPayload != payload) { key = Guid.NewGuid(); lastPayload = payload; }
        var result = await api.SaveAsync("api/loans", new BorrowRequest(readerId, normalized, key));
        key = Guid.NewGuid(); lastPayload = "";
        return result!.Value.GetProperty("loanId").GetInt32();
    }
}
