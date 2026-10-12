using BookShop.Application.Books.Dtos;

namespace BookShop.Web.Models.Books
{
    public class BooksIndexViewModel
    {
        public string? Search { get; set; }

        public List<Guid> CategoryIds { get; set; } = [];
        public List<Guid> PublisherIds { get; set; } = [];
        public List<Guid> AuthorIds { get; set; } = [];
        public List<Guid> TagIds { get; set; } = [];

        public List<BookListItemViewModel> Books { get; set; } = [];

        public int Page { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}
