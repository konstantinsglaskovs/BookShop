namespace BookShop.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Order Order { get; set; } = null!;
    public Guid OrderId { get; set; }
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
