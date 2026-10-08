using System.ComponentModel.DataAnnotations;

namespace BookShop.Web.Models.Categories
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, MinimumLength = 2,
         ErrorMessage = "Category name must be between 2 and 100 characters.")]
        [Display(Name = "Category name")]
        public string Name { get; set; } = null!;
    }
}
