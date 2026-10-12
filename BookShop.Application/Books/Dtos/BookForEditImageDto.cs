namespace BookShop.Application.Books.Dtos;

public class BookForEditImageDto
{
    public Guid Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int DisplayOrder { get; set; }
}
