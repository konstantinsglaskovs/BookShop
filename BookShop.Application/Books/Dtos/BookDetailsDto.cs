namespace BookShop.Application.Books.Dtos;

public class BookDetailsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ISBN { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string CategoryName { get; set; } = null!;
    public string PublisherName { get; set; } = null!;
    public List<string> Authors { get; set; } = [];

    public List<string> Tags { get; set; } = [];

    public List<BookImageDto> Images { get; set; } = [];
}
