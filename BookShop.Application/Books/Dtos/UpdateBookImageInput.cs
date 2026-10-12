using BookShop.Application.Сommon.Models;

namespace BookShop.Application.Books.Dtos;

public class UpdateBookImageInput
{
    public Guid ExistingImageId { get; set; }
    public BookImageInput? NewImage { get; set; }
}
