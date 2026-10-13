namespace BookShop.Application.Orders.Admins.Dtos;

public class AdminOrderItemDto
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}
