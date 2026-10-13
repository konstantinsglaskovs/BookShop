using BookShop.Domain.Enums;

namespace BookShop.Web.Models.Orders;

public class AdminOrdersViewModel
{
    public List<AdminOrderListItemViewModel> Orders { get; set; } = [];

    public int Page { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }
}
