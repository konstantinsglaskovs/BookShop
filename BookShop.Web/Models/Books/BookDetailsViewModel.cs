namespace BookShop.Web.Models.Books
{
    public class BookDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? ISBN { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string CategoryName { get; set; } = null!;
        public Guid CategoryId { get; set; }

        public string PublisherName { get; set; } = null!;

        public List<string> Authors { get; set; } = [];

        public List<string> Tags { get; set; } = [];

        public List<BookDetailsImageViewModel> Images { get; set; } = [];
    }
}
