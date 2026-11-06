using Library.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Library.Server.Data;
public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>(); await db.Database.MigrateAsync();
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var name in new[] { Roles.Reader, Roles.Librarian, Roles.Manager }) if (!await roles.RoleExistsAsync(name)) await roles.CreateAsync(new IdentityRole(name));
        var users = services.GetRequiredService<UserManager<AppUser>>();
        foreach (var (name, fullName, role) in new[] { ("manager", "Nguyễn Minh Anh", Roles.Manager), ("librarian", "Trần Thanh Hà", Roles.Librarian), ("SV001", "Lê Hoàng Nam", Roles.Reader), ("SV002", "Nguyễn Thu Thảo", Roles.Reader) })
        {
            if (await users.FindByNameAsync(name) != null) continue;
            var user = new AppUser { UserName = name, FullName = fullName };
            var result = await users.CreateAsync(user, "Library@123"); if (!result.Succeeded) throw new InvalidOperationException("Không tạo được dữ liệu mẫu.");
            await users.AddToRoleAsync(user, role);
        }
        if (!await db.Policies.AnyAsync()) db.Policies.Add(new PolicyVersion());
        if (!await db.Books.AnyAsync())
        {
            var category = new Category { Name = "Công nghệ thông tin" }; var literature = new Category { Name = "Văn học" }; var publisher = new Publisher { Name = "NXB Giáo dục" };
            var titles = new[] { "Lập trình C#", "Cấu trúc dữ liệu và giải thuật", "Cơ sở dữ liệu", "Tuổi trẻ đáng giá bao nhiêu", "Nhập môn công nghệ phần mềm", "Mạng máy tính" };
            for (var i = 0; i < titles.Length; i++)
            {
                var book = new Book { Title = titles[i], Isbn = $"978604000000{i}", Year = 2024 + i % 3,
                    Description = "Tài liệu dành cho bạn đọc đại học. Khám phá kiến thức nền tảng, ví dụ thực hành và những góc nhìn hữu ích cho học tập.",
                    Category = i == 3 ? literature : category, Publisher = publisher, Authors = [new Author { Name = i == 3 ? "Rosie Nguyễn" : "Nhóm tác giả " + (i + 1) }], ImagePath = $"images/books/book-{i + 1}.svg" };
                for (var j = 1; j <= 3; j++) book.Copies.Add(new BookCopy { Code = $"CP{i * 3 + j:000}", Location = $"Kệ A{i + 1}" }); db.Books.Add(book);
            }
        }
        await db.SaveChangesAsync();
    }
}
