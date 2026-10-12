using BookShop.Domain.Enums;

namespace BookShop.Web.Models.Books
{
    public class AdminBookListItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public List<string> Authors { get; set; } = [];
        public BookStatus Status { get; set; }
    }
}
