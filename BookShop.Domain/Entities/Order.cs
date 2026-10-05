using BookShop.Domain.Enums;

namespace BookShop.Domain.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;

    public string RecipientName { get; set; } = null!;
    public string RecipientSurname { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public DateTime? ShippingDate { get; set; }

    public decimal Total { get; set; }

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public DateTime? PaymentDate { get; set; }

    public string? TrackingNumber { get; set; }

    public string? Carrier { get; set; }


    public OrderAddress OrderAddress { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
