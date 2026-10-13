namespace BookShop.Application.Reviews.Dtos;

public class BookReviewDto
{
    public Guid Id { get; set; }
    public string AuthorName { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
