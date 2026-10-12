using System.ComponentModel.DataAnnotations;

namespace BookShop.Web.Models.Books
{
    public class UpdateBookViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Book title is required.")]
        [StringLength(200,MinimumLength = 2,ErrorMessage = "Title must be between 2 and 200 characters.")]
        public string Title { get; set; } = null!;

        [StringLength(4000,ErrorMessage = "Description cannot exceed 4000 characters.")]
        public string? Description { get; set; }

        [StringLength(20,ErrorMessage = "ISBN cannot exceed 20 characters.")]
        public string? ISBN { get; set; }

        [Range(0,999999.99,ErrorMessage = "Price cannot be negative.")]
        public decimal Price { get; set; }

        [Range(0,int.MaxValue,ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Publisher is required.")]
        public Guid PublisherId { get; set; }

        [MinLength(1,ErrorMessage = "At least one author is required.")]
        public List<Guid> AuthorIds { get; set; } = [];

        public List<Guid> TagIds { get; set; } = [];

        public List<BookEditImageViewModel> Images { get; set; } = [];
    }
}
