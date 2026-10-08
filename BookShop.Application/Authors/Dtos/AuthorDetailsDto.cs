namespace BookShop.Application.Authors.Dtos;

public class AuthorDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Bio { get; set; }
}
