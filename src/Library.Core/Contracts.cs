using System.ComponentModel.DataAnnotations;

namespace Library.Core;
public record Page<T>(List<T> Items, int Total, int PageNumber, int Size);
public record BookDto(int Id, string Title, string Isbn, int Year, string Description, string ImagePath,
    int CategoryId, string Category, int PublisherId, string Publisher, List<int> AuthorIds, string Authors, bool Active, int TotalCopies, int Available);
public record CopyDto(int Id, int BookId, string Code, string Location, string Status);
public record NamedDto(int Id, string Name);
public record MetadataDto(List<NamedDto> Authors, List<NamedDto> Categories, List<NamedDto> Publishers);
public record UserDto(string Id, string UserName, string FullName, string Role, bool Active);
public record LoanDto(int Id, int LoanId, string ReaderId, string ReaderName, string CopyCode, string Title,
    DateTime BorrowedAt, DateOnly DueDate, DateTime? ReturnedAt, string Status, int DaysOverdue);
public record LoginRequest([Required] string UserName, [Required] string Password);
public record BorrowRequest([Required] string ReaderId, [Required, MinLength(1), MaxLength(100)] string[] CopyCodes, Guid RequestKey);
public record ReturnRequest([Required] string CopyCode);
public record PolicyRequest([Range(1, 50)] int LoanLimit, [Range(1, 365)] int LoanDays, bool BlockIfOverdue);
public record CopyRequest([Range(1, int.MaxValue)] int BookId, [Required, MaxLength(40)] string Code, [MaxLength(80)] string Location);
public record CopyUpdate([Required, MaxLength(80)] string Location, [Required] string Status);
public record NamedRequest([Required, MaxLength(120)] string Name);
public class BookRequest
{
    [Required, MaxLength(200)] public string Title { get; set; } = "";
    [Required, RegularExpression(@"[0-9Xx\-]{10,20}")] public string Isbn { get; set; } = "";
    [Range(1400, 2100)] public int Year { get; set; } = 2026;
    [MaxLength(4000)] public string Description { get; set; } = "";
    [Required] public string ImagePath { get; set; } = "images/books/default.svg";
    [Range(1, int.MaxValue)] public int CategoryId { get; set; }
    [Range(1, int.MaxValue)] public int PublisherId { get; set; }
    [MinLength(1)] public int[] AuthorIds { get; set; } = [];
    public bool Active { get; set; } = true;
}
public record UserRequest([Required, MaxLength(50)] string UserName, [Required, MaxLength(120)] string FullName,
    [Required, MinLength(8)] string Password, [Required] string Role, bool Active);
public record UserUpdate([Required, MaxLength(120)] string FullName, [Required] string Role, bool Active);
public class BusinessException(string code, string message) : Exception(message) { public string Code { get; } = code; }
public interface IBusinessClock { DateOnly Today { get; } DateTime UtcNow { get; } }
public class BusinessClock : IBusinessClock
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(UtcNow,
        TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")));
}
public static class LoanRules
{
    public static int DaysOverdue(DateOnly due, DateOnly today, bool returned) => returned ? 0 : Math.Max(0, today.DayNumber - due.DayNumber);
    public static string[] NormalizeCodes(string[] codes)
    {
        var normalized = codes.Select(x => x.Trim().ToUpperInvariant()).ToArray();
        if (normalized.Length == 0 || normalized.Length > 100 || normalized.Any(string.IsNullOrWhiteSpace) || normalized.Distinct().Count() != normalized.Length)
            throw new BusinessException("INVALID_COPIES", "Nhập 1–100 mã bản sao khác nhau.");
        return normalized.Order().ToArray();
    }
    public static void CheckEligibility(bool active, int current, int requested, bool overdue, PolicyVersion policy)
    {
        if (!active) throw new BusinessException("READER_BLOCKED", "Bạn đọc đã bị khóa hoặc không tồn tại.");
        if (current + requested > policy.LoanLimit) throw new BusinessException("LOAN_LIMIT", $"Vượt giới hạn {policy.LoanLimit} bản đang mượn.");
        if (overdue && policy.BlockIfOverdue) throw new BusinessException("OVERDUE", "Bạn đọc còn sách quá hạn chưa trả.");
    }
    public static bool IsRelativeBookImage(string path) => !string.IsNullOrWhiteSpace(path) &&
        path.StartsWith("images/books/", StringComparison.Ordinal) && !path.Contains("..") && !path.Contains('\\') &&
        !path.Contains(':') && path.All(c => char.IsLetterOrDigit(c) || c is '/' or '.' or '-' or '_') &&
        new[] { ".png", ".jpg", ".jpeg", ".webp", ".svg" }.Any(x => path.EndsWith(x, StringComparison.OrdinalIgnoreCase));
}
