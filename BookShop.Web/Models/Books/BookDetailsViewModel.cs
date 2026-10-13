using BookShop.Application.Books.Dtos;
using BookShop.Application.Reviews.Dtos;

namespace BookShop.Web.Models.Books
{
    public class BookDetailsViewModel
    {
        public BookDetailsDto Book { get; set; } = null!;

        public List<BookReviewDto> Reviews { get; set; } = [];
    }
}
