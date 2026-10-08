using ErrorOr;
using MediatR;

namespace BookShop.Application.Tags.Commands.UpdateTag;

public class UpdateTagCommand:IRequest<ErrorOr<Success>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
