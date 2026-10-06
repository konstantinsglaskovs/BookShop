using ErrorOr;
using MediatR;

namespace BookShop.Application.Publishers.Commands.UpdatePublisher;

public class UpdatePublisherCommand:IRequest<ErrorOr<Success>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
