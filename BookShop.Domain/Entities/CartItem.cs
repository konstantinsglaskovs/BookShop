namespace BookShop.Domain.Entities;

public class CartItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int Quantity { get; set; }

    

}
