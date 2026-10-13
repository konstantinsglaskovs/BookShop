using BookShop.Application.Orders.Users.Models;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Entities;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Orders.Users.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ErrorOr<string>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;
    private readonly IPaymentService _paymentService;

    public CreateOrderCommandHandler(IAppDbContext db,
        IIdentityService identityService, IPaymentService paymentService)
    {
        _db = db;
        _identityService = identityService;
        _paymentService = paymentService;
    }

    public async Task<ErrorOr<string>> Handle(CreateOrderCommand request,CancellationToken cancellationToken)
    {
        var profileResult =await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        var userProfileId = profileResult.Value;

        var cartItems = await _db.CartItems
            .Include(item => item.Book)
            .Where(item =>item.UserProfileId == userProfileId)
            .ToListAsync();

        if (cartItems.Count == 0)
        {
            return Error.Validation(code: "Order.EmptyCart",description: "The cart is empty.");
        }

        foreach (var cartItem in cartItems)
        {
            if (cartItem.Book.Status != BookStatus.Active)
            {
                return Error.Validation(code: "Order.BookUnavailable",
                    description:$"Book \"{cartItem.Book.Title}\" is no longer available.");
            }

            if (cartItem.Quantity >
                cartItem.Book.StockQuantity)
            {
                return Error.Validation(code: "Order.NotEnoughStock",description:$"Not enough copies of \"{cartItem.Book.Title}\" are in stock.");
            }
        }

        var order = new Order
        {
            UserProfileId = userProfileId,

            RecipientName = request.RecipientName,
            RecipientSurname = request.RecipientSurname,
            PhoneNumber = request.PhoneNumber,

            OrderAddress = new OrderAddress
            {
                StreetAddress = request.StreetAddress,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country
            }
        };

        foreach (var cartItem in cartItems)
        {
            order.OrderItems.Add(
                new OrderItem
                {
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Book.Price
                });

            cartItem.Book.StockQuantity -=cartItem.Quantity;
        }

        order.Total = order.OrderItems.Sum(item =>item.UnitPrice *item.Quantity);

        _db.Orders.Add(order);

        await _db.SaveChangesAsync();

        var paymentResult = await _paymentService.CreateCheckoutSessionAsync(
                    new PaymentSessionRequest
                    {
                        OrderId = order.Id,
                        Total = order.Total,

                        Items = cartItems
                            .Select(cartItem => new PaymentSessionItem
                            {
                                Name = cartItem.Book.Title,

                                UnitPrice = cartItem.Book.Price,

                                Quantity = cartItem.Quantity
                            })
                            .ToList()
                    });

        if (paymentResult.IsError)
        {
            foreach (var cartItem in cartItems)
            {
                cartItem.Book.StockQuantity +=cartItem.Quantity;
            }

            _db.Orders.Remove(order);

            await _db.SaveChangesAsync();

            return paymentResult.Errors;
        }

        await _db.SaveChangesAsync();

        return paymentResult.Value.CheckoutUrl;
    }
}
