using BookShop.Application.Сommon.Extensions;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Tags.Commands.CreateTag;

public class CreateTagCommandHandler:IRequestHandler<CreateTagCommand,ErrorOr<Success>>
{
    private readonly IAppDbContext _db;

    public CreateTagCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(CreateTagCommand request,CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Error.Validation(code: "Tag.NameRequired",description: "Tag name is required.");
        }

        var normalizedName = request.Name.ToNormalizedTitleCase();

        var tagExists = await _db.Tags.AnyAsync(tag => tag.Name == normalizedName);

        if (tagExists)
        {
            return Error.Conflict(code: "Tag.AlreadyExists",description: "Tag with this name already exists.");
        }

        var tag = new Tag
        {
            Name = normalizedName
        };

        _db.Tags.Add(tag);

        await _db.SaveChangesAsync();

        return Result.Success;
    }
}
