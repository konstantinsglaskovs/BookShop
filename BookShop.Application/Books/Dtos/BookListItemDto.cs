namespace BookShop.Application.Books.Dtos;

public class BookListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public List<string> Authors { get; set; } = [];
    public string? ImageStorageKey { get; set; }

    public string? ImageUrl { get; set; }
}
