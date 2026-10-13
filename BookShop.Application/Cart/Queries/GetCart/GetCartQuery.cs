using BookShop.Application.Cart.Dtos;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Cart.Queries.GetCart;

public class GetCartQuery:IRequest<ErrorOr<CartDto>>
{

}
