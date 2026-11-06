using Library.Core;
using Library.Server.Data;
using Microsoft.EntityFrameworkCore;
namespace Library.Server.Services;
public class CatalogService(AppDbContext db)
{
    private static readonly System.Linq.Expressions.Expression<Func<Book, BookDto>> Project = b => new BookDto(b.Id, b.Title, b.Isbn, b.Year,
        b.Description, b.ImagePath, b.CategoryId, b.Category.Name, b.PublisherId, b.Publisher.Name,
        b.Authors.Select(a => a.Id).ToList(), string.Join(", ", b.Authors.Select(a => a.Name)), b.Active, b.Copies.Count, b.Copies.Count(c => c.Status == "AVAILABLE"));
    public async Task<Page<BookDto>> SearchAsync(string? q, int? category, int? author, bool available, int page, bool staff = false)
    {
        page = Math.Clamp(page, 1, 100000);
        var query = db.Books.AsNoTracking().Where(b => staff || b.Active);
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(b => b.Title.Contains(q) || b.Isbn.Contains(q) || b.Authors.Any(a => a.Name.Contains(q)));
        if (category.HasValue) query = query.Where(b => b.CategoryId == category);
        if (author.HasValue) query = query.Where(b => b.Authors.Any(a => a.Id == author));
        if (available) query = query.Where(b => b.Copies.Any(c => c.Status == "AVAILABLE"));
        return new(await query.OrderBy(b => b.Title).Skip((page - 1) * 6).Take(6).Select(Project).ToListAsync(), await query.CountAsync(), page, 6);
    }
    public Task<BookDto?> GetAsync(int id, bool staff = false) => db.Books.AsNoTracking().Where(b => b.Id == id && (staff || b.Active)).Select(Project).SingleOrDefaultAsync();
    public async Task<MetadataDto> MetadataAsync() => new(await db.Authors.OrderBy(x => x.Name).Select(x => new NamedDto(x.Id, x.Name)).ToListAsync(),
        await db.Categories.OrderBy(x => x.Name).Select(x => new NamedDto(x.Id, x.Name)).ToListAsync(), await db.Publishers.OrderBy(x => x.Name).Select(x => new NamedDto(x.Id, x.Name)).ToListAsync());
    public async Task<int> SaveAsync(int? id, BookRequest r)
    {
        if (r.Year > DateTime.UtcNow.Year + 1 || !LoanRules.IsRelativeBookImage(r.ImagePath)) throw new BusinessException("VALIDATION", "Năm xuất bản hoặc đường dẫn ảnh tương đối không hợp lệ.");
        if (await db.Books.AnyAsync(x => x.Isbn == r.Isbn && x.Id != id)) throw new BusinessException("ISBN_EXISTS", "ISBN đã tồn tại.");
        var authors = await db.Authors.Where(x => r.AuthorIds.Contains(x.Id)).ToListAsync();
        if (!await db.Categories.AnyAsync(x => x.Id == r.CategoryId) || !await db.Publishers.AnyAsync(x => x.Id == r.PublisherId) || authors.Count != r.AuthorIds.Distinct().Count() || authors.Count == 0) throw new BusinessException("METADATA", "Chọn thể loại, nhà xuất bản và tác giả hợp lệ.");
        var book = id.HasValue ? await db.Books.Include(x => x.Authors).SingleOrDefaultAsync(x => x.Id == id) : new Book();
        if (book == null) throw new BusinessException("NOT_FOUND", "Không tìm thấy sách.");
        book.Title = r.Title.Trim(); book.Isbn = r.Isbn.Trim(); book.Year = r.Year; book.Description = r.Description;
        book.ImagePath = r.ImagePath; book.CategoryId = r.CategoryId; book.PublisherId = r.PublisherId; book.Active = r.Active; book.Authors = authors;
        if (!id.HasValue) db.Books.Add(book);
        await db.SaveChangesAsync(); return book.Id;
    }
    public async Task DeleteAsync(int id)
    {
        var book = await db.Books.Include(x => x.Copies).SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new BusinessException("NOT_FOUND", "Không tìm thấy sách.");
        if (book.Copies.Count > 0 || await db.LoanItems.AnyAsync(x => x.BookCopy.BookId == id))
            throw new BusinessException("BOOK_HAS_HISTORY", "Không thể xóa cứng sách đã có bản sách hoặc lịch sử mượn. Hãy ẩn sách để giữ dữ liệu lịch sử.");
        db.Books.Remove(book);
        await db.SaveChangesAsync();
    }
}
