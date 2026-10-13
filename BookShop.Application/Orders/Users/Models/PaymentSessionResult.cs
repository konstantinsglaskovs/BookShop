namespace BookShop.Application.Orders.Users.Models;

public class PaymentSessionResult
{
    public string SessionId { get; set; } = null!;
    public string CheckoutUrl { get; set; } = null!;
}
