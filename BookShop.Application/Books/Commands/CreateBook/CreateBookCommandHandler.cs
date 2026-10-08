using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Books.Commands.CreateBook;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, ErrorOr<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly IFileStorage _fileStorage;

    public CreateBookCommandHandler(IAppDbContext db, IFileStorage fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateBookCommand request,CancellationToken cancellationToken)
    {
        var categoryExists = await _db.Categories.AnyAsync(category => category.Id == request.CategoryId);

        if (!categoryExists)
        {
            return Error.NotFound(code: "Book.CategoryNotFound",description: "Category was not found.");
        }

        var publisherExists = await _db.Publishers.AnyAsync(publisher => publisher.Id == request.PublisherId);

        if (!publisherExists)
        {
            return Error.NotFound(code: "Book.PublisherNotFound",description: "Publisher was not found.");
        }

        var distinctAuthorIds = request.AuthorIds.Distinct().ToList();

        var existingAuthorIds = await _db.Authors
            .Where(author => distinctAuthorIds.Contains(author.Id))
            .Select(author => author.Id).ToListAsync();

        if (existingAuthorIds.Count != distinctAuthorIds.Count)
        {
            return Error.NotFound(code: "Book.AuthorNotFound",description: "One or more authors were not found.");
        }

        var distinctTagIds = request.TagIds.Distinct().ToList();

        if (distinctTagIds.Count > 0)
        {
            var existingTagIds = await _db.Tags
                .Where(tag => distinctTagIds.Contains(tag.Id))
                .Select(tag => tag.Id)
                .ToListAsync();

            if (existingTagIds.Count != distinctTagIds.Count)
            {
                return Error.NotFound(code: "Book.TagNotFound",description: "One or more tags were not found.");
            }
        }

        var book = new Book
        {
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description)? null: request.Description.Trim(),

            ISBN = string.IsNullOrWhiteSpace(request.ISBN)? null: request.ISBN.Trim(),

            Price = request.Price,
            StockQuantity = request.StockQuantity,

            CategoryId = request.CategoryId,
            PublisherId = request.PublisherId
        };

        _db.Books.Add(book);

        foreach (var authorId in distinctAuthorIds)
        {
            _db.BookAuthors.Add(new BookAuthor
            {
                BookId = book.Id,
                AuthorId = authorId
            });
        }

        foreach (var tagId in distinctTagIds)
        {
            _db.BookTags.Add(new BookTag
            {
                BookId = book.Id,
                TagId = tagId
            });
        }

        for (var i = 0; i < request.Images.Count; i++)
        {
            var image = request.Images[i];

            var storageKey = $"books/{book.Id}/{Guid.NewGuid()}{image.FileExtension}";

            var uploadResult = await _fileStorage.UploadAsync(image.Stream,storageKey,image.ContentType);

            if (uploadResult.IsError)
            {
                return uploadResult.Errors;
            }

            _db.BookImages.Add(new BookImage
            {
                BookId = book.Id,
                StorageKey = storageKey,
                DisplayOrder = i
            });
        }


        await _db.SaveChangesAsync(cancellationToken);

        return book.Id;
    }
}
