using BookShop.Application.Books.Dtos;
using BookShop.Application.Сommon.Extensions;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Application.Сommon.Models;
using BookShop.Domain.Enums;
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
        var query = _db.Books.AsNoTracking().Where(book => book.Status == BookStatus.Active);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();

            query = query.Where(book =>
                book.Title.ToLower().Contains(search) ||
                book.BookAuthors.Any(bookAuthor => bookAuthor.Author.Name.ToLower().Contains(search)));
        }

        if (request.CategoryIds.Count > 0)
        {
            query = query.Where(book => request.CategoryIds.Contains(book.CategoryId));
        }

        if (request.PublisherIds.Count > 0)
        {
            query = query.Where(book => request.PublisherIds.Contains(book.PublisherId));
        }

        if (request.AuthorIds.Count > 0)
        {
            query = query.Where(book => book.BookAuthors.Any(bookAuthor => request.AuthorIds.Contains(bookAuthor.AuthorId)));
        }

        if (request.TagIds.Count > 0)
        {
            query = query.Where(book => book.BookTags.Any(bookTag => request.TagIds.Contains(bookTag.TagId)));
        }

        var totalCount = await query.CountAsync();

        var books = await query
            .OrderBy(book => book.Title)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(book => new BookListItemDto
            {
                Id = book.Id,
                Title = book.Title,
                Price = book.Price,

                Authors = book.BookAuthors
                    .Select(bookAuthor => bookAuthor.Author.Name)
                    .ToList(),

                ImageStorageKey = book.Images
                    .OrderBy(image => image.DisplayOrder)
                    .Select(image => image.StorageKey)
                    .FirstOrDefault()
            }).ToListAsync();

        foreach (var book in books)
        {
            if (book.ImageStorageKey is not null)
            {
                book.ImageUrl =_fileStorage.GetPublicUrl(book.ImageStorageKey);
            }
        }


        return new PagedResult<BookListItemDto>
        {
            Items = books,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
