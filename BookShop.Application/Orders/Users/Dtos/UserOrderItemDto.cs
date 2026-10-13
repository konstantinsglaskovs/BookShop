namespace BookShop.Application.Orders.Users.Dtos;

public class UserOrderItemDto
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}
