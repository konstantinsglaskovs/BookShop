using ErrorOr;
using MediatR;

namespace BookShop.Application.Cart.Queries.GetCartCount
{
    public class GetCartCountQuery:IRequest<ErrorOr<int>>
    {

    }
}
