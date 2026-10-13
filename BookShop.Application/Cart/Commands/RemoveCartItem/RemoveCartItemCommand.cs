using ErrorOr;
using MediatR;

namespace BookShop.Application.Cart.Commands.RemoveCartItem;

public class RemoveCartItemCommand:IRequest<ErrorOr<Success>>
{
    public Guid CartItemId { get; set; }
}


