using BookShop.Application.Orders.Admins.Dtos;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Orders.Admins.Queries.GetAdminOrderById;

public class GetAdminOrderByIdQuery:IRequest<ErrorOr<AdminOrderDetailsDto>>
{
    public Guid Id { get; set; }
}
