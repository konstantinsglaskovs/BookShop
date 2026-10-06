using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Authors.Commands.DeleteAuthor;

public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _db;

    public DeleteAuthorCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteAuthorCommand request,CancellationToken cancellationToken)
    {
        var author = await _db.Authors.FirstOrDefaultAsync(author => author.Id == request.Id);

        if (author is null)
        {
            return Error.NotFound(code: "Author.NotFound",description: "Author was not found.");
        }

        var authorBookIds = await _db.BookAuthors.Where(bookAuthor => bookAuthor.AuthorId == request.Id)
            .Select(bookAuthor => bookAuthor.BookId).ToListAsync();

        var isOnlyAuthorForAnyBook = await _db.BookAuthors.Where(bookAuthor => authorBookIds.Contains(bookAuthor.BookId))
            .GroupBy(bookAuthor => bookAuthor.BookId)
            .AnyAsync(group => group.Count() == 1);

        if (isOnlyAuthorForAnyBook)
        {
            return Error.Conflict(
                code: "Author.IsOnlyAuthor",
                description:"Author cannot be deleted because they are the only author of one or more books.");
        }

        _db.Authors.Remove(author);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
