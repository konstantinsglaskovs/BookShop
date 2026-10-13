using BookShop.Application.Cart.Dtos;
using BookShop.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Web.Models.Checkout
{
    public class CheckoutViewModel
    {
        [Required]
        [StringLength(100)]
        public string RecipientName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string RecipientSurname { get; set; } = null!;

        [Required]
        [StringLength(30)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string StreetAddress { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = null!;

        [StringLength(100)]
        public string? State { get; set; }

        [Required]
        [StringLength(20)]
        public string PostalCode { get; set; } = null!;

        [Required]
        public Country Country { get; set; }

        public CartDto Cart { get; set; } = null!;
    }
}
