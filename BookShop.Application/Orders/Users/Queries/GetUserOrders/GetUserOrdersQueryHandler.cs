using BookShop.Application.Orders.Users.Dtos;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Orders.Users.Queries.GetUserOrders;

public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, ErrorOr<List<UserOrderListItemDto>>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;

    public GetUserOrdersQueryHandler(IAppDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<ErrorOr<List<UserOrderListItemDto>>> Handle(
        GetUserOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var profileResult =await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        var orders = await _db.Orders
            .AsNoTracking()
            .Where(order =>order.UserProfileId == profileResult.Value && order.PaymentStatus != PaymentStatus.Pending)
            .OrderByDescending(order => order.OrderDate)
            .Select(order => new UserOrderListItemDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Total = order.Total,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,
                ItemsCount = order.OrderItems.Sum(item => item.Quantity)
            })
            .ToListAsync();

        return orders;
    }
}
