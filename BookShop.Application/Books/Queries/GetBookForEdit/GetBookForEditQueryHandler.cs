using BookShop.Application.Books.Dtos;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Books.Queries.GetBookForEdit;

public class GetBookForEditQueryHandler : IRequestHandler<GetBookForEditQuery, ErrorOr<BookForEditDto>>
{
    private readonly IAppDbContext _db;
    private readonly IFileStorage _fileStorage;

    public GetBookForEditQueryHandler(IAppDbContext db,IFileStorage fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ErrorOr<BookForEditDto>> Handle(GetBookForEditQuery request,CancellationToken cancellationToken)
    {
        var book = await _db.Books.AsNoTracking()
            .Where(book => book.Id == request.Id)
            .Select(book => new
            {
                book.Id,
                book.Title,
                book.Description,
                book.ISBN,
                book.Price,
                book.StockQuantity,
                book.CategoryId,
                book.PublisherId,

                AuthorIds = book.BookAuthors
                    .Select(bookAuthor => bookAuthor.AuthorId).ToList(),

                TagIds = book.BookTags
                    .Select(bookTag => bookTag.TagId).ToList(),

                Images = book.Images
                    .OrderBy(image => image.DisplayOrder)
                    .Select(image => new
                    {
                        image.Id,
                        image.StorageKey,
                        image.DisplayOrder
                    }).ToList()
            }).FirstOrDefaultAsync();

        if (book is null)
        {
            return Error.NotFound(code: "Book.NotFound",description: "Book was not found.");
        }

        return new BookForEditDto
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            ISBN = book.ISBN,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            CategoryId = book.CategoryId,
            PublisherId = book.PublisherId,
            AuthorIds = book.AuthorIds,
            TagIds = book.TagIds,

            Images = book.Images
                .Select(image => new BookForEditImageDto
                {
                    Id = image.Id,
                    ImageUrl = _fileStorage.GetPublicUrl(image.StorageKey),
                    DisplayOrder = image.DisplayOrder
                })
                .ToList()
        };
    }
}
