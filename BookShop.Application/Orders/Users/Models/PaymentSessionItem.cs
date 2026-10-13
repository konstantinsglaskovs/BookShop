namespace BookShop.Application.Orders.Users.Models;

public class PaymentSessionItem
{
    public string Name { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
