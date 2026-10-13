using ErrorOr;
using MediatR;

namespace BookShop.Application.Cart.Commands.AddToCart;

public class AddToCartCommand:IRequest<ErrorOr<Success>>
{
    public Guid BookId { get; set; }
    public int Quantity { get; set; } = 1;
}
