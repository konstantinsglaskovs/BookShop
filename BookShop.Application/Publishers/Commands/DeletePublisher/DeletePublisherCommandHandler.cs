using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Publishers.Commands.DeletePublisher;

public class DeletePublisherCommandHandler : IRequestHandler<DeletePublisherCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _db;

    public DeletePublisherCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(DeletePublisherCommand request, CancellationToken cancellationToken)
    {
        var publisher = await _db.Publishers.FirstOrDefaultAsync(publisher => publisher.Id == request.Id);

        if (publisher is null)
        {
            return Error.NotFound(code: "Publisher.NotFound", description: "Publisher was not found.");
        }

        var isUsedByBooks = await _db.Books.AnyAsync(book => book.PublisherId == request.Id);

        if (isUsedByBooks)
        {
            return Error.Conflict(code: "Publisher.InUse", description: "Publisher cannot be deleted because it is used by one or more books.");
        }

        _db.Publishers.Remove(publisher);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
