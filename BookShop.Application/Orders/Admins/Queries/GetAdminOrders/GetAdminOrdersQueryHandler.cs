using BookShop.Application.Orders.Admins.Dtos;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Application.Сommon.Models;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Orders.Admins.Queries.GetAdminOrders;

public class GetAdminOrdersQueryHandler : IRequestHandler<GetAdminOrdersQuery, ErrorOr<PagedResult<AdminOrderListItemDto>>>
{
    private readonly IAppDbContext _db;

    public GetAdminOrdersQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<PagedResult<AdminOrderListItemDto>>> Handle(GetAdminOrdersQuery request,CancellationToken cancellationToken)
    {
        var query = _db.Orders.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();

            query = query.Where(order =>
                order.RecipientName.ToLower().Contains(search) ||
                order.RecipientSurname.ToLower().Contains(search) ||
                order.Id.ToString().ToLower().Contains(search));
        }

        if (request.OrderStatus is not null)
        {
            query = query.Where(order =>order.OrderStatus == request.OrderStatus);
        }

        if (request.PaymentStatus is not null)
        {
            query = query.Where(order =>order.PaymentStatus == request.PaymentStatus);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(order => order.OrderDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(order => new AdminOrderListItemDto
            {
                Id = order.Id,

                RecipientName = order.RecipientName,
                RecipientSurname = order.RecipientSurname,

                OrderDate = order.OrderDate,

                Total = order.Total,

                OrderStatus = order.OrderStatus,

                PaymentStatus = order.PaymentStatus,

                ItemsCount = order.OrderItems.Sum(
                    item => item.Quantity)
            })
            .ToListAsync();

        return new PagedResult<AdminOrderListItemDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
