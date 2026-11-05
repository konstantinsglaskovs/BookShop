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
        [Phone]
        [RegularExpression(
        @"^\+?[0-9\s\-\(\)]{7,20}$",
        ErrorMessage = "Enter a valid phone number.")]
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
        [RegularExpression(@"^((LV-)?\d{4}|LT-\d{5}|\d{5})$",ErrorMessage = "Enter a valid postal code.")]
        public string PostalCode { get; set; } = null!;

        [Required]
        public Country Country { get; set; }

        public CartDto Cart { get; set; } = null!;
    }
}
