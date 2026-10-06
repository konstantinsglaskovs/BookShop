using ErrorOr;
using MediatR;

namespace BookShop.Application.Authors.Commands.UpdateAuthor;

public class UpdateAuthorCommand: IRequest<ErrorOr<Success>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Bio { get; set; }
}
