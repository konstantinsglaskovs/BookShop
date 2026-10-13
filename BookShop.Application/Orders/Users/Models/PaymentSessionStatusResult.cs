namespace BookShop.Application.Orders.Users.Models;

public class PaymentSessionStatusResult
{
    public Guid OrderId { get; set; }
    public string PaymentStatus { get; set; } = null!;
}

