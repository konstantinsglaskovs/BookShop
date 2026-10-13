using BookShop.Application.Orders.Users.Dtos;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Orders.Users.Queries.GetUserOrders;

public class GetUserOrdersQuery:IRequest<ErrorOr<List<UserOrderListItemDto>>>
{
}
