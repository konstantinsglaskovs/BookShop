namespace BookShop.Application.Cart.Dtos;

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Subtotal { get; set; }
}
