using ErrorOr;
using MediatR;

namespace BookShop.Application.Cart.Commands.ChangeCartItemQuantity;

public class ChangeCartItemQuantityCommand:IRequest<ErrorOr<Success>>
{
    public Guid CartItemId { get; set; }
    public int QuantityChange { get; set; }
}
