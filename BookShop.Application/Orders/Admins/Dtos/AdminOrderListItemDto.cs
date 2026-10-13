using BookShop.Domain.Enums;

namespace BookShop.Application.Orders.Admins.Dtos;

public class AdminOrderListItemDto
{
    public Guid Id { get; set; }
    public string RecipientName { get; set; } = null!;
    public string RecipientSurname { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public int ItemsCount { get; set; }
}
