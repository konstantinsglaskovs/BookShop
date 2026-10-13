using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Orders.Users.Commands.ConfirmPayment;

public class ConfirmPaymentCommandHandler:IRequestHandler<ConfirmPaymentCommand,ErrorOr<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;
    private readonly IPaymentService _paymentService;

    public ConfirmPaymentCommandHandler(IAppDbContext db,IIdentityService identityService,IPaymentService paymentService)
    {
        _db = db;
        _identityService = identityService;
        _paymentService = paymentService;
    }

    public async Task<ErrorOr<Guid>> Handle(ConfirmPaymentCommand request,CancellationToken cancellationToken)
    {
        var sessionResult =await _paymentService.GetCheckoutSessionAsync(request.SessionId);

        if (sessionResult.IsError)
        {
            return sessionResult.Errors;
        }

        if (sessionResult.Value.PaymentStatus != "paid")
        {
            return Error.Validation(code: "Payment.NotPaid",description: "Payment has not been completed.");
        }

        var profileResult =await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        var order = await _db.Orders
            .FirstOrDefaultAsync(order =>
                    order.Id == sessionResult.Value.OrderId && order.UserProfileId == profileResult.Value);

        if (order is null)
        {
            return Error.NotFound(code: "Order.NotFound",description: "Order was not found.");
        }

        if (order.PaymentStatus == PaymentStatus.Paid)
        {
            return order.Id;
        }

        if (order.OrderStatus == OrderStatus.Cancelled)
        {
            return Error.Validation(code: "Order.Cancelled",description: "Order has already been cancelled.");
        }

        order.PaymentStatus =PaymentStatus.Paid;

        order.PaymentDate =DateTime.UtcNow;

        order.OrderStatus =OrderStatus.Processing;

        var cartItems = await _db.CartItems
            .Where(item =>item.UserProfileId == order.UserProfileId).ToListAsync();

        _db.CartItems.RemoveRange(cartItems);

        await _db.SaveChangesAsync();

        return order.Id;
    }
}
