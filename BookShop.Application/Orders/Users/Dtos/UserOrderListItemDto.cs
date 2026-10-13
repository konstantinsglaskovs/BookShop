using BookShop.Domain.Enums;

namespace BookShop.Application.Orders.Users.Dtos;

public class UserOrderListItemDto
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public int ItemsCount { get; set; }
}
