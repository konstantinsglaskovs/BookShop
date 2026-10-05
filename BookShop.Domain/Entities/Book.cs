namespace BookShop.Domain.Entities;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ISBN { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;

    public ICollection<BookImage> Images { get; set; } = new List<BookImage>();

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public ICollection<BookTag> BookTags { get; set; } = new List<BookTag>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
