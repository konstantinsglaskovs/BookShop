using ErrorOr;
using MediatR;

namespace BookShop.Application.Tags.Commands.DeleteTag;

public class DeleteTagCommand:IRequest<ErrorOr<Success>>
{
    public Guid Id { get; set; }
}
