using BookShop.Application.Books.Dtos;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Books.Queries.GetBookById;

public class GetBookByIdQueryHandler:IRequestHandler<GetBookByIdQuery,ErrorOr<BookDetailsDto>>
{
    private readonly IAppDbContext _db;
    private readonly IFileStorage _fileStorage;

    public GetBookByIdQueryHandler(IAppDbContext db, IFileStorage fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ErrorOr<BookDetailsDto>> Handle(
        GetBookByIdQuery request,
        CancellationToken cancellationToken)
    {
        var book = await _db.Books
             .AsNoTracking()
             .Where(book => book.Id == request.Id)
             .Select(book => new
             {
                 book.Id,
                 book.Title,
                 book.Description,
                 book.ISBN,
                 book.Price,
                 book.StockQuantity,

                 CategoryName = book.Category.Name,
                 PublisherName = book.Publisher.Name,
                 CategoryId = book.Category.Id,

                 Authors = book.BookAuthors
                     .Select(bookAuthor => bookAuthor.Author.Name)
                     .ToList(),

                 Tags = book.BookTags
                     .Select(bookTag => bookTag.Tag.Name)
                     .ToList(),

                 Images = book.Images
                     .OrderBy(image => image.DisplayOrder)
                     .Select(image => new
                     {
                         image.StorageKey,
                         image.DisplayOrder
                     }).ToList()
             })
             .FirstOrDefaultAsync();

        if (book is null)
        {
            return Error.NotFound(code: "Book.NotFound",description: "Book was not found.");
        }

        return new BookDetailsDto
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            ISBN = book.ISBN,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            CategoryId = book.CategoryId,
            CategoryName = book.CategoryName,
            PublisherName = book.PublisherName,
            Authors = book.Authors,
            Tags = book.Tags,

            Images = book.Images
                .Select(image => new BookImageDto
                {
                    ImageUrl = _fileStorage.GetPublicUrl(
                        image.StorageKey),

                    DisplayOrder = image.DisplayOrder
                })
                .ToList()
        };
    }
}
