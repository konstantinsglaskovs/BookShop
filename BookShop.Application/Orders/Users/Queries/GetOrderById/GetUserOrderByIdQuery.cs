using BookShop.Application.Orders.Users.Dtos;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Orders.Users.Queries.GetOrderById;

public class GetUserOrderByIdQuery:IRequest<ErrorOr<UserOrderDetailsDto>>
{
    public Guid Id { get; set; }
}
