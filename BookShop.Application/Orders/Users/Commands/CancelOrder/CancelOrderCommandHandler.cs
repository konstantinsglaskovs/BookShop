using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Orders.Users.Commands.CancelOrder;

public class CancelOrderCommandHandler:IRequestHandler<CancelOrderCommand,ErrorOr<Success>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;

    public CancelOrderCommandHandler(IAppDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<ErrorOr<Success>> Handle(CancelOrderCommand request,CancellationToken cancellationToken)
    {
        var profileResult =await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        var userProfileId = profileResult.Value;

        var order = await _db.Orders
            .Include(order => order.OrderItems).ThenInclude(item => item.Book)
            .FirstOrDefaultAsync(order =>order.Id == request.OrderId && order.UserProfileId == userProfileId);

        if (order is null)
        {
            return Error.NotFound(code: "Order.NotFound", description: "Order was not found.");
        }

        if (order.PaymentStatus == PaymentStatus.Paid)
        {
            return Error.Validation(code: "Order.AlreadyPaid",description: "Paid order cannot be cancelled this way.");
        }

        if (order.OrderStatus == OrderStatus.Cancelled)
        {
            return Result.Success;
        }

        foreach (var item in order.OrderItems)
        {
            item.Book.StockQuantity += item.Quantity;
        }

        order.OrderStatus = OrderStatus.Cancelled;
        order.PaymentStatus = PaymentStatus.Cancelled;

        await _db.SaveChangesAsync();

        return Result.Success;
    }
}
