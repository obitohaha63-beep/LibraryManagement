using System.ComponentModel.DataAnnotations;

namespace Library.Core;

public static class Roles
{
    public const string Reader = "Reader", Librarian = "Librarian", Manager = "Manager";
    public const string Staff = Librarian + "," + Manager;
}
public class Book
{
    public int Id { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = "";
    [Required, MaxLength(20)] public string Isbn { get; set; } = "";
    public int Year { get; set; }
    public string Description { get; set; } = "";
    public string ImagePath { get; set; } = "images/books/default.svg";
    public bool Active { get; set; } = true;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;
    public List<Author> Authors { get; set; } = [];
    public List<BookCopy> Copies { get; set; } = [];
}
public class Author { public int Id { get; set; } [Required, MaxLength(120)] public string Name { get; set; } = ""; public List<Book> Books { get; set; } = []; }
public class Category { public int Id { get; set; } [Required, MaxLength(120)] public string Name { get; set; } = ""; }
public class Publisher { public int Id { get; set; } [Required, MaxLength(120)] public string Name { get; set; } = ""; }
public class BookCopy
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    [Required, MaxLength(40)] public string Code { get; set; } = "";
    [MaxLength(80)] public string Location { get; set; } = "";
    public string Status { get; set; } = "AVAILABLE";
}
public class PolicyVersion
{
    public int Id { get; set; }
    public int LoanLimit { get; set; } = 3;
    public int LoanDays { get; set; } = 14;
    public bool BlockIfOverdue { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "seed";
}
public class Loan
{
    public int Id { get; set; }
    public string ReaderId { get; set; } = "";
    public string StaffId { get; set; } = "";
    public Guid RequestKey { get; set; }
    public string RequestFingerprint { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int PolicyVersionId { get; set; }
    public PolicyVersion PolicyVersion { get; set; } = null!;
    public List<LoanItem> Items { get; set; } = [];
}
public class LoanItem
{
    public int Id { get; set; }
    public int LoanId { get; set; }
    public Loan Loan { get; set; } = null!;
    public int BookCopyId { get; set; }
    public BookCopy BookCopy { get; set; } = null!;
    public DateOnly DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public string? ReturnedBy { get; set; }
    public string Status { get; set; } = "ON_LOAN";
}
