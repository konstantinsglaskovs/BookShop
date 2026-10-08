using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Books.Commands.UpdateBook;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, ErrorOr<Guid>>
{
    private readonly IAppDbContext _db;

    public UpdateBookCommandHandler(IAppDbContext db)
    {
        _db = db;
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

        await _db.SaveChangesAsync(cancellationToken);

        return book.Id;

    }
}
