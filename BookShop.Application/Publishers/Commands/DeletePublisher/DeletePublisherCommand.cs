using ErrorOr;
using MediatR;

namespace BookShop.Application.Publishers.Commands.DeletePublisher;

public class DeletePublisherCommand:IRequest<ErrorOr<Success>>
{
    public Guid Id { get; set; }
}
