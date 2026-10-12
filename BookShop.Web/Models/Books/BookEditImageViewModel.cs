namespace BookShop.Web.Models.Books
{
    public class BookEditImageViewModel
    {
        public Guid ExistingImageId { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile? NewImage { get; set; }
    }
}
