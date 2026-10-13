using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Orders.Users.Commands.ExpirePendingOrder
{
    public class ExpirePendingOrderCommandHandler : IRequestHandler<ExpirePendingOrderCommand, ErrorOr<Success>>
    {
        private readonly IAppDbContext _db;

        public ExpirePendingOrderCommandHandler(
            IAppDbContext db)
        {
            _db = db;
        }

        public async Task<ErrorOr<Success>> Handle(ExpirePendingOrderCommand request,CancellationToken cancellationToken)
        {
            var order = await _db.Orders.Include(order => order.OrderItems).ThenInclude(item => item.Book)
                .FirstOrDefaultAsync(order => order.Id == request.OrderId);

            if (order is null)
            {
                return Error.NotFound(code: "Order.NotFound",description: "Order was not found.");
            }

            if (order.OrderStatus != OrderStatus.Pending ||order.PaymentStatus != PaymentStatus.Pending)
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
}
