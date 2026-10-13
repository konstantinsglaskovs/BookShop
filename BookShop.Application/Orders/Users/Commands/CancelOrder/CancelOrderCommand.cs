using ErrorOr;
using MediatR;

namespace BookShop.Application.Orders.Users.Commands.CancelOrder;

public class CancelOrderCommand:IRequest<ErrorOr<Success>>
{
    public Guid OrderId { get; set; }
}
