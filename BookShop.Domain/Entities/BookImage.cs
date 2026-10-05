namespace BookShop.Domain.Entities;

public class BookImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
    public string StorageKey { get; set; } = null!;
    public int DisplayOrder { get; set; }
}
