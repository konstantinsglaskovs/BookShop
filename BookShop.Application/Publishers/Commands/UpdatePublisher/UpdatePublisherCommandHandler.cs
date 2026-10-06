using BookShop.Application.Сommon.Extensions;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Publishers.Commands.UpdatePublisher;

public class UpdatePublisherCommandHandler:IRequestHandler<UpdatePublisherCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _db;

    public UpdatePublisherCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(UpdatePublisherCommand request,CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Error.Validation(code: "Publisher.NameRequired",description: "Publisher name is required.");
        }

        var publisher = await _db.Publishers.FirstOrDefaultAsync(publisher => publisher.Id == request.Id);

        if (publisher is null)
        {
            return Error.NotFound(code: "Publisher.NotFound",description: "Publisher was not found.");
        }

        var normalizedName = request.Name.ToNormalizedTitleCase();

        var publisherExists = await _db.Publishers.AnyAsync(otherPublisher =>
                    otherPublisher.Id != request.Id &&otherPublisher.Name == normalizedName);

        if (publisherExists)
        {
            return Error.Conflict(code: "Publisher.AlreadyExists",description: "Publisher with this name already exists.");
        }

        publisher.Name = normalizedName;

        await _db.SaveChangesAsync();

        return Result.Success;
    }
}
