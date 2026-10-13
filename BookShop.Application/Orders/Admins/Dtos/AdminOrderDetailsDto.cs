using BookShop.Domain.Enums;

namespace BookShop.Application.Orders.Admins.Dtos;

public class AdminOrderDetailsDto
{
    public Guid Id { get; set; }

    public Guid UserProfileId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal Total { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime? ShippingDate { get; set; }

    public string? TrackingNumber { get; set; }

    public string? Carrier { get; set; }

    public string RecipientName { get; set; } = null!;

    public string RecipientSurname { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string StreetAddress { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? State { get; set; }

    public string PostalCode { get; set; } = null!;

    public Country Country { get; set; }

    public List<AdminOrderItemDto> Items { get; set; } = [];
}
