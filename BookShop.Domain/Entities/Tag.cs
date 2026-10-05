namespace BookShop.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;

    public ICollection<BookTag> BookTags { get; set; } = new List<BookTag>();
}
