using BookShop.Application.Books.Dtos;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Application.Сommon.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Books.Queries.GetAdminBooks;

public class GetAdminBooksQueryHandler : IRequestHandler<GetAdminBooksQuery, PagedResult<AdminBookListItemDto>>
{
    private readonly IAppDbContext _db;

    public GetAdminBooksQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<AdminBookListItemDto>> Handle(GetAdminBooksQuery request,CancellationToken cancellationToken)
    {
        var query = _db.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();

            query = query.Where(book =>
                book.Title.ToLower().Contains(search) ||
                book.BookAuthors.Any(bookAuthor =>
                    bookAuthor.Author.Name.ToLower().Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var books = await query
            .OrderBy(book => book.Title)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(book => new AdminBookListItemDto
            {
                Id = book.Id,
                Title = book.Title,

                Authors = book.BookAuthors
                    .Select(bookAuthor => bookAuthor.Author.Name)
                    .ToList(),

                Status = book.Status
            }).ToListAsync();

        return new PagedResult<AdminBookListItemDto>
        {
            Items = books,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
