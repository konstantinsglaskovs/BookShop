using ErrorOr;
using MediatR;

namespace BookShop.Application.Tags.Commands.CreateTag;

public class CreateTagCommand:IRequest<ErrorOr<Success>>
{
    public string Name { get; set; } = null!;
}
