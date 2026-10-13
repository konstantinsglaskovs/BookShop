using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Orders.Admins.Commands.UpdateOrder;

public class UpdateOrderCommandHandler:IRequestHandler<UpdateOrderCommand,ErrorOr<Success>>
{
    private readonly IAppDbContext _db;

    public UpdateOrderCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateOrderCommand request,CancellationToken cancellationToken)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(order => order.Id == request.Id);

        if (order is null)
        {
            return Error.NotFound(code: "Order.NotFound",description: "Order was not found.");
        }

        if (order.PaymentStatus != PaymentStatus.Paid)
        {
            return Error.Validation(code: "Order.NotPaid",description: "Only paid orders can be processed.");
        }

        if (order.OrderStatus == OrderStatus.Cancelled)
        {
            return Error.Validation(code: "Order.Cancelled",description: "Cancelled order cannot be updated.");
        }

        if (request.OrderStatus == OrderStatus.Cancelled)
        {
            return Error.Validation(code: "Order.InvalidStatus",description: "Paid order cannot be cancelled this way.");
        }

        if (request.OrderStatus == OrderStatus.Shipped)
        {
            if (string.IsNullOrWhiteSpace(request.Carrier))
            {
                return Error.Validation(code: "Order.CarrierRequired",description: "Carrier is required when the order is shipped.");
            }

            if (string.IsNullOrWhiteSpace(request.TrackingNumber))
            {
                return Error.Validation(
                    code: "Order.TrackingNumberRequired",
                    description: "Tracking number is required when the order is shipped.");
            }
        }

        order.OrderStatus = request.OrderStatus;
        order.TrackingNumber = request.TrackingNumber;
        order.Carrier = request.Carrier;

        if (request.OrderStatus == OrderStatus.Shipped)
        {
            order.ShippingDate = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
