using Library.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Server.Data;
public class AppUser : IdentityUser
{
    public string FullName { get; set; } = "";
    public bool Active { get; set; } = true;
}
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookCopy> Copies => Set<BookCopy>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<LoanItem> LoanItems => Set<LoanItem>();
    public DbSet<PolicyVersion> Policies => Set<PolicyVersion>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.UseCollation("Vietnamese_100_CI_AI");
        b.Entity<Book>().HasIndex(x => x.Isbn).IsUnique();
        b.Entity<BookCopy>().HasIndex(x => x.Code).IsUnique();
        b.Entity<Loan>().Property(x => x.StaffId).HasMaxLength(450);
        b.Entity<Loan>().HasIndex(x => new { x.StaffId, x.RequestKey }).IsUnique();
        b.Entity<LoanItem>().HasIndex(x => x.BookCopyId).IsUnique().HasFilter("[Status] = 'ON_LOAN'");
        b.Entity<LoanItem>().ToTable(t => t.HasCheckConstraint("CK_LoanItem_Return", "([Status] = 'ON_LOAN' AND [ReturnedAt] IS NULL) OR ([Status] = 'RETURNED' AND [ReturnedAt] IS NOT NULL)"));
        b.Entity<BookCopy>().ToTable(t => t.HasCheckConstraint("CK_Copy_Status", "[Status] IN ('AVAILABLE','ON_LOAN','INACTIVE')"));
        b.Entity<Loan>().HasOne<AppUser>().WithMany().HasForeignKey(x => x.ReaderId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Loan>().HasOne<AppUser>().WithMany().HasForeignKey(x => x.StaffId).OnDelete(DeleteBehavior.Restrict);
        foreach (var fk in b.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys())) fk.DeleteBehavior = DeleteBehavior.Restrict;
    }
}
