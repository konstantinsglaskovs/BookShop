using BookShop.Application.Orders.Users.Dtos;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Orders.Users.Queries.GetOrderById;

public class GetUserOrderByIdQueryHandler: IRequestHandler<GetUserOrderByIdQuery, ErrorOr<UserOrderDetailsDto>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;

    public GetUserOrderByIdQueryHandler(IAppDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<ErrorOr<UserOrderDetailsDto>> Handle(GetUserOrderByIdQuery request,CancellationToken cancellationToken)
    {
        var profileResult =await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        var order = await _db.Orders.AsNoTracking()
            .Where(order =>order.Id == request.Id && order.UserProfileId == profileResult.Value && order.PaymentStatus != PaymentStatus.Pending)
            .Select(order => new UserOrderDetailsDto
            {
                Id = order.Id,
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

                StreetAddress = order.OrderAddress.StreetAddress,
                City = order.OrderAddress.City,
                State = order.OrderAddress.State,
                PostalCode = order.OrderAddress.PostalCode,
                Country = order.OrderAddress.Country,

                Items = order.OrderItems
                    .Select(item => new UserOrderItemDto
                    {
                        BookId = item.BookId,
                        Title = item.Book.Title,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Subtotal = item.UnitPrice * item.Quantity
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
