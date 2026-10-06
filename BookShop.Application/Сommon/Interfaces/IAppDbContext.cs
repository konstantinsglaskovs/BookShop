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

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
