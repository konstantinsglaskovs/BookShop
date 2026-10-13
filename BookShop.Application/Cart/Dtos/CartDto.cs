namespace BookShop.Application.Cart.Dtos;

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = [];
    public decimal Total { get; set; }
}
