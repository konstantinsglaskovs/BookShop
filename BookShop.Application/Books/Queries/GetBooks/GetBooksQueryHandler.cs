using BookShop.Application.Books.Dtos;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Application.Сommon.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Books.Queries.GetBooks;

public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, PagedResult<BookListItemDto>>
{
    private readonly IAppDbContext _db;
    private readonly IFileStorage _fileStorage;


    public GetBooksQueryHandler(IAppDbContext db, IFileStorage fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<PagedResult<BookListItemDto>> Handle(GetBooksQuery request,CancellationToken cancellationToken)
    {
        var query = _db.Books.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(book => book.Title.Contains(search));
        }

        if (request.CategoryIds.Count > 0)
        {
            var categoryIds = request.CategoryIds.Distinct().ToList();

            query = query.Where(book => categoryIds.Contains(book.CategoryId));
        }

        if (request.PublisherIds.Count > 0)
        {
            var publisherIds = request.PublisherIds.Distinct().ToList();

            query = query.Where(book => publisherIds.Contains(book.PublisherId));
        }

        if (request.AuthorIds.Count > 0)
        {
            var authorIds = request.AuthorIds.Distinct().ToList();

            query = query.Where(book => book.BookAuthors.Any(bookAuthor => authorIds.Contains(bookAuthor.AuthorId)));
        }

        if (request.TagIds.Count > 0)
        {
            var tagIds = request.TagIds.Distinct().ToList();

            query = query.Where(book => book.BookTags.Any(bookTag => tagIds.Contains(bookTag.TagId)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var books = await query
            .OrderBy(book => book.Title)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(book => new BookListItemDto
            {
                Id = book.Id,
                Title = book.Title,
                Price = book.Price,

                Authors = book.BookAuthors.Select(bookAuthor => bookAuthor.Author.Name).ToList(),

                ImageUrl = book.Images.OrderBy(image => image.DisplayOrder)
                    .Select(image => image.StorageKey)
                    .FirstOrDefault()!
            }).ToListAsync();

        var items = books.Select(book => new BookListItemDto
        {
            Id = book.Id,
            Title = book.Title,
            Price = book.Price,
            Authors = book.Authors,

            ImageUrl = _fileStorage.GetPublicUrl(book.ImageUrl)
        })
        .ToList();

        return new PagedResult<BookListItemDto>
        {
            Items = books,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
