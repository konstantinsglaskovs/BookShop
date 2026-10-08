namespace BookShop.Application.Сommon.Models;

public class BookImageInput
{
    public Stream Stream { get; set; } = null!;
    public string FileExtension { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}
