using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Tags.Commands.DeleteTag;

public class DeleteTagCommandHandler:IRequestHandler<DeleteTagCommand,ErrorOr<Success>>
{
    private readonly IAppDbContext _db;

    public DeleteTagCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(tag => tag.Id == request.Id);

        if (tag is null)
        {
            return Error.NotFound(code: "Tag.NotFound", description: "Tag was not found.");
        }

        _db.Tags.Remove(tag);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }   
}
