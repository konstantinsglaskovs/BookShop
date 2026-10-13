namespace BookShop.Application.Orders.Users.Models
{
    public class PaymentSessionRequest
    {
        public Guid OrderId { get; set; }
        public decimal Total { get; set; }
        public List<PaymentSessionItem> Items { get; set; } = [];
    }
}
