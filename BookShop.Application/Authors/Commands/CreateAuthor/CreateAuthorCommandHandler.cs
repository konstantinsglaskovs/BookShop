using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Entities;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Authors.Commands.CreateAuthor;

public class CreateAuthorCommandHandler:IRequestHandler<CreateAuthorCommand,ErrorOr<Success>>
{
    private readonly IAppDbContext _db;

    public CreateAuthorCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(CreateAuthorCommand request,CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Error.Validation(code: "Author.NameRequired",description: "Author name is required.");
        }

        var author = new Author
        {
            Name = request.Name.Trim(),
            Bio = string.IsNullOrWhiteSpace(request.Bio)? null: request.Bio.Trim()
        };

        _db.Authors.Add(author);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
