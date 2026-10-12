namespace BookShop.Web.Models.Books
{
    public class AdminBooksIndexViewModel
    {
        public string? Search { get; set; }
        public List<AdminBookListItemViewModel> Books { get; set; } = [];
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}
