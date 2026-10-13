using BookShop.Application.Orders.Admins.Dtos;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Orders.Admins.Queries.GetAdminOrderById;

public class GetAdminOrderByIdQueryHandler:IRequestHandler<GetAdminOrderByIdQuery, ErrorOr<AdminOrderDetailsDto>>
{
    private readonly IAppDbContext _db;

    public GetAdminOrderByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<AdminOrderDetailsDto>> Handle(GetAdminOrderByIdQuery request,CancellationToken cancellationToken)
    {
        var order = await _db.Orders.AsNoTracking()
            .Where(order =>order.Id == request.Id)
            .Select(order => new AdminOrderDetailsDto
            {
                Id = order.Id,

                UserProfileId = order.UserProfileId,

                OrderDate = order.OrderDate,

                Total = order.Total,

                OrderStatus = order.OrderStatus,

                PaymentStatus = order.PaymentStatus,

                PaymentDate = order.PaymentDate,

                ShippingDate = order.ShippingDate,

                TrackingNumber = order.TrackingNumber,

                Carrier = order.Carrier,

                RecipientName = order.RecipientName,

                RecipientSurname = order.RecipientSurname,

                PhoneNumber = order.PhoneNumber,

                StreetAddress =order.OrderAddress.StreetAddress,

                City =order.OrderAddress.City,

                State =order.OrderAddress.State,

                PostalCode =order.OrderAddress.PostalCode,

                Country =order.OrderAddress.Country,

                Items = order.OrderItems
                    .Select(item => new AdminOrderItemDto
                    {
                        BookId = item.BookId,

                        Title = item.Book.Title,

                        Quantity = item.Quantity,

                        UnitPrice = item.UnitPrice,

                        Subtotal =item.UnitPrice *item.Quantity
                    }).ToList()
            })
            .FirstOrDefaultAsync();

        if (order is null)
        {
            return Error.NotFound(code: "Order.NotFound",description: "Order was not found.");
        }

        return order;
    }
}
