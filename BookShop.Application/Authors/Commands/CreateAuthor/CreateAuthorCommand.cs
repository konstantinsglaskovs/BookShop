using ErrorOr;
using MediatR;

namespace BookShop.Application.Authors.Commands.CreateAuthor;

public class CreateAuthorCommand:IRequest<ErrorOr<Success>>
{
    public string Name { get; set; } = null!;
    public string? Bio { get; set; }
}
