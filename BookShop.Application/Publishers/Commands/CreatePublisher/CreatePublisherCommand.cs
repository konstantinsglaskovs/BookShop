using ErrorOr;
using MediatR;

namespace BookShop.Application.Publishers.Commands.CreatePublisher;

public class CreatePublisherCommand:IRequest<ErrorOr<Success>>
{
    public string Name { get; set; } = null!;
}
