using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Books.Commands.UpdateBook;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, ErrorOr<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly IFileStorage _fileStorage;

    public UpdateBookCommandHandler(IAppDbContext db, IFileStorage fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ErrorOr<Guid>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _db.Books.FirstOrDefaultAsync(book => book.Id == request.Id);

        if (book is null)
        {
            return Error.NotFound(code: "Book.NotFound",description: "Book was not found.");
        }

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

        var authorIds = request.AuthorIds.Distinct().ToList();

        if (authorIds.Count == 0)
        {
            return Error.Validation(code: "Book.AuthorRequired",description: "Book must have at least one author.");
        }

        var existingAuthorsCount = await _db.Authors.CountAsync(author => authorIds.Contains(author.Id));

        if (existingAuthorsCount != authorIds.Count)
        {
            return Error.NotFound(code: "Book.AuthorNotFound",description: "One or more authors were not found.");
        }

        var tagIds = request.TagIds.Distinct().ToList();

        if (tagIds.Count > 0)
        {
            var existingTagsCount = await _db.Tags.CountAsync(tag => tagIds.Contains(tag.Id));

            if (existingTagsCount != tagIds.Count)
            {
                return Error.NotFound(code: "Book.TagNotFound",description: "One or more tags were not found.");
            }
        }

        var existingImages = await _db.BookImages.Where(image => image.BookId == book.Id).ToListAsync();

        var requestedExistingImageIds = request.Images.Where(image =>image.ExistingImageId != Guid.Empty)
            .Select(image => image.ExistingImageId).ToList();

        if (requestedExistingImageIds.Count != requestedExistingImageIds.Distinct().Count())
        {
            return Error.Validation(code: "Book.DuplicateImage",description: "The same image cannot be used more than once.");
        }

        var existingImageIds = existingImages.Select(image => image.Id).ToHashSet();

        if (requestedExistingImageIds.Any(id => !existingImageIds.Contains(id)))
        {
            return Error.Validation(code: "Book.ImageNotFound",description: "One or more images were not found.");
        }

        foreach (var slot in request.Images)
        {
            if (slot.ExistingImageId != Guid.Empty && slot.NewImage is not null)
            {
                return Error.Validation(code: "Book.InvalidImageSlot",description: "An image slot cannot contain both an existing and a new image.");
            }
        }




        book.Title = request.Title.Trim();

        book.Description = string.IsNullOrWhiteSpace(request.Description)? null: request.Description.Trim();

        book.ISBN = string.IsNullOrWhiteSpace(request.ISBN) ? null : request.ISBN.Trim();
        book.Price = request.Price;
        book.StockQuantity = request.StockQuantity;
        book.CategoryId = request.CategoryId;
        book.PublisherId = request.PublisherId;

        var existingBookAuthors = await _db.BookAuthors.Where(bookAuthor => bookAuthor.BookId == book.Id).ToListAsync();

        _db.BookAuthors.RemoveRange(existingBookAuthors);

        foreach (var authorId in authorIds)
        {
            _db.BookAuthors.Add(new BookAuthor
            {
                BookId = book.Id,
                AuthorId = authorId
            });
        }

        var existingBookTags = await _db.BookTags.Where(bookTag => bookTag.BookId == book.Id).ToListAsync();

        _db.BookTags.RemoveRange(existingBookTags);

        foreach (var tagId in tagIds)
        {
            _db.BookTags.Add(new BookTag
            {
                BookId = book.Id,
                TagId = tagId
            });
        }

        var imagesToDelete = existingImages.Where(image =>!requestedExistingImageIds.Contains(image.Id)).ToList();

        foreach (var image in imagesToDelete)
        {
            var deleteResult = await _fileStorage.DeleteAsync(image.StorageKey);

            if (deleteResult.IsError)
            {
                return deleteResult.Errors;
            }
        }

        _db.BookImages.RemoveRange(imagesToDelete);

        for (var i = 0; i < request.Images.Count; i++)
        {
            var slot = request.Images[i];

            if (slot.ExistingImageId != Guid.Empty)
            {
                var image = existingImages.First(image =>image.Id == slot.ExistingImageId);

                image.DisplayOrder = i;

                continue;
            }

            if (slot.NewImage is null)
            {
                continue;
            }

            var storageKey =$"books/{book.Id}/{Guid.NewGuid()}{slot.NewImage.FileExtension}";

            var uploadResult = await _fileStorage.UploadAsync(slot.NewImage.Stream,storageKey,slot.NewImage.ContentType);

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

        await _db.SaveChangesAsync();

        return book.Id;

    }
}
