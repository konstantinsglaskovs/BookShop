using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Orders.Users.Commands.CreateOrder;

public class CreateOrderCommand:IRequest<ErrorOr<string>>
{
    public string RecipientName { get; set; } = null!;

    public string RecipientSurname { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string StreetAddress { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? State { get; set; }

    public string PostalCode { get; set; } = null!;

    public Country Country { get; set; }
}
