using BookShop.Domain.Entities;
using BookShop.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Infrastructure.Persistance;

public class AppDbContext: IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public DbSet<BookImage> BookImages { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }
    public DbSet<BookTag> BookTags { get; set; }

    public DbSet<Review> Reviews { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderAddress> OrderAddresses { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookAuthor>().HasKey(x => new { x.BookId, x.AuthorId });

        modelBuilder.Entity<BookTag>().HasKey(x => new { x.BookId, x.TagId });

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(x => x.UserProfile)
            .WithOne()
            .HasForeignKey<UserProfile>(x => x.ApplicationUserId);
    }

}
