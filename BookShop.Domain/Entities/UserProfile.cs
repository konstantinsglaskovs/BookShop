namespace BookShop.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ApplicationUserId { get; set; }

    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;

    public string? ProfileImageStorageKey { get; set; }



    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
