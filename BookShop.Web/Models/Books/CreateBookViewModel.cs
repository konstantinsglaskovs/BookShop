using System.ComponentModel.DataAnnotations;

namespace BookShop.Web.Models.Books
{
    public class CreateBookViewModel
    {
        [Required(ErrorMessage = "Book title is required.")]
        [StringLength(200, MinimumLength = 2)]
        public string Title { get; set; } = null!;

        [StringLength(4000)]
        public string? Description { get; set; }

        [StringLength(20)]
        public string? ISBN { get; set; }

        [Range(0, 999999.99)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Publisher is required.")]
        public Guid PublisherId { get; set; }
        [MinLength(1, ErrorMessage = "At least one author is required.")]
        public List<Guid> AuthorIds { get; set; } = [];

        public List<Guid> TagIds { get; set; } = [];

        [MinLength(1, ErrorMessage = "At least one image is required.")]
        public List<IFormFile> Images { get; set; } = [];
    }
}
