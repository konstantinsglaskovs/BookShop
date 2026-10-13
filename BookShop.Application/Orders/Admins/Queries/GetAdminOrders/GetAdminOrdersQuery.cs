using BookShop.Application.Orders.Admins.Dtos;
using BookShop.Application.Сommon.Models;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Orders.Admins.Queries.GetAdminOrders;

public class GetAdminOrdersQuery: IRequest<ErrorOr<PagedResult<AdminOrderListItemDto>>>
{
    public string? Search { get; set; }

    public OrderStatus? OrderStatus { get; set; }

    public PaymentStatus? PaymentStatus { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
