using BookShop.Domain.Enums;

namespace BookShop.Application.Books.Dtos;

public class AdminBookListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public List<string> Authors { get; set; } = [];
    public BookStatus Status { get; set; }
}
