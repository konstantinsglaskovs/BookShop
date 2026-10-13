using BookShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Сommon.Interfaces;

public interface IAppDbContext
{
    DbSet<Category> Categories { get; }

    DbSet<Publisher> Publishers { get; }

    DbSet<Author> Authors { get; }
    DbSet<BookAuthor> BookAuthors { get; }

    DbSet<Tag> Tags { get; }
    DbSet<BookTag> BookTags { get; }

    DbSet<Book> Books { get; }

    DbSet<BookImage> BookImages { get; }

    DbSet<OrderItem> OrderItems { get; }
    DbSet<CartItem> CartItems { get; }
    DbSet<Order> Orders { get; }
    DbSet<Review> Reviews { get; }
    DbSet<UserProfile> UserProfiles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
