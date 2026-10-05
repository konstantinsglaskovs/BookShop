namespace BookShop.Domain.Entities;

public class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    

    
}
