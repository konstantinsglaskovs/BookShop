using BookShop.Application.Сommon.Extensions;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Tags.Commands.UpdateTag;

public class UpdateTagCommandHandler:IRequestHandler<UpdateTagCommand,ErrorOr<Success>>
{
    private readonly IAppDbContext _db;

    public UpdateTagCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateTagCommand request,CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Error.Validation(code: "Tag.NameRequired",description: "Tag name is required.");
        }

        var tag = await _db.Tags.FirstOrDefaultAsync(tag => tag.Id == request.Id);

        if (tag is null)
        {
            return Error.NotFound(code: "Tag.NotFound",description: "Tag was not found.");
        }

        var normalizedName = request.Name.ToNormalizedTitleCase();

        var tagExists = await _db.Tags.AnyAsync(otherTag =>
                    otherTag.Id != request.Id && otherTag.Name == normalizedName);

        if (tagExists)
        {
            return Error.Conflict(code: "Tag.AlreadyExists",description: "Tag with this name already exists.");
        }

        tag.Name = normalizedName;

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
