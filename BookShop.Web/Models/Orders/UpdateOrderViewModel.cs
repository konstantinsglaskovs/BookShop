using BookShop.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Web.Models.Orders
{
    public class UpdateOrderViewModel
    {
        public Guid Id { get; set; }
        public OrderStatus OrderStatus { get; set; }

        [StringLength(100)]
        public string? TrackingNumber { get; set; }

        [StringLength(100)]
        public string? Carrier { get; set; }
    }
}
