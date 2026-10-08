using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Authors.Commands.UpdateAuthor;

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _db;

    public UpdateAuthorCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Error.Validation(code: "Author.NameRequired", description: "Author name is required.");
        }

        var author = await _db.Authors.FirstOrDefaultAsync(author => author.Id == request.Id);

        if (author is null)
        {
            return Error.NotFound(code: "Author.NotFound", description: "Author was not found.");
        }

        author.Name = request.Name.Trim();

        author.Bio = string.IsNullOrWhiteSpace(request.Bio) ? null : request.Bio.Trim();

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
