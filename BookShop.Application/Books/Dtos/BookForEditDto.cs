namespace BookShop.Application.Books.Dtos;

public class BookForEditDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ISBN { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Guid CategoryId { get; set; }
    public Guid PublisherId { get; set; }
    public List<Guid> AuthorIds { get; set; } = [];
    public List<Guid> TagIds { get; set; } = [];

    public List<BookForEditImageDto> Images { get; set; } = [];
}
