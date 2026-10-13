using ErrorOr;
using MediatR;

namespace BookShop.Application.Orders.Users.Commands.ExpirePendingOrder
{
    public class ExpirePendingOrderCommand:IRequest<ErrorOr<Success>>
    {
        public Guid OrderId { get; set; }
    }
}
