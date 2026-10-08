using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Books.Commands.DeleteBook;

public class DeleteBookCommandHandler:IRequestHandler<DeleteBookCommand,ErrorOr<Success>>
{
    private readonly IAppDbContext _db;
    private readonly IFileStorage _fileStorage;

    public DeleteBookCommandHandler(IAppDbContext db,IFileStorage fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteBookCommand request,CancellationToken cancellationToken)
    {
        var book = await _db.Books.FirstOrDefaultAsync(book => book.Id == request.Id);

        if (book is null)
        {
            return Error.NotFound(code: "Book.NotFound",description: "Book was not found.");
        }

        var isUsedInOrders = await _db.OrderItems.AnyAsync(orderItem => orderItem.BookId == book.Id);

        if (isUsedInOrders)
        {
            book.Status = BookStatus.Inactive;

            await _db.SaveChangesAsync();

            return Result.Success;
        }

        var imageStorageKeys = await _db.BookImages.Where(image => image.BookId == book.Id).Select(image => image.StorageKey).ToListAsync();

        foreach (var storageKey in imageStorageKeys)
        {
            var deleteResult = await _fileStorage.DeleteAsync(storageKey);

            if (deleteResult.IsError)
            {
                return deleteResult.Errors;
            }
        }

        _db.Books.Remove(book);

        await _db.SaveChangesAsync();

        return Result.Success;
    }
}
