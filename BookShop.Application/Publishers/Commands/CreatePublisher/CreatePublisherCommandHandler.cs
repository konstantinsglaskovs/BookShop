using BookShop.Application.Сommon.Extensions;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Publishers.Commands.CreatePublisher;

public class CreatePublisherCommandHandler:IRequestHandler<CreatePublisherCommand,ErrorOr<Success>>
{
    private readonly IAppDbContext _db;

    public CreatePublisherCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(CreatePublisherCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Error.Validation(code: "Publisher.NameRequired",description: "Publisher name is required.");
        }

        var normalizedName = request.Name.ToNormalizedTitleCase();

        var publisherExists = await _db.Publishers.AnyAsync(publisher => publisher.Name == normalizedName);

        if (publisherExists)
        {
            return Error.Conflict(code: "Publisher.AlreadyExists",description: "Publisher with this name already exists.");
        }

        var publisher = new Publisher
        {
            Name = normalizedName
        };

        _db.Publishers.Add(publisher);

        await _db.SaveChangesAsync();

        return Result.Success;
    }
}
