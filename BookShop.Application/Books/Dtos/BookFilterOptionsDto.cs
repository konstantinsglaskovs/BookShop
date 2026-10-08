namespace BookShop.Application.Books.Dtos;

public class BookFilterOptionsDto
{
    public List<FilterOptionDto> Categories { get; set; } = [];
    public List<FilterOptionDto> Publishers { get; set; } = [];
    public List<FilterOptionDto> Authors { get; set; } = [];
    public List<FilterOptionDto> Tags { get; set; } = [];
}
