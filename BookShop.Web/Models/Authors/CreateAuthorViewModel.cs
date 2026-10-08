using System.ComponentModel.DataAnnotations;

namespace BookShop.Web.Models.Authors
{
    public class CreateAuthorViewModel
    {
        [Required(ErrorMessage = "Author name is required.")]
        [StringLength(100,MinimumLength = 2,ErrorMessage = "Author name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;

        [StringLength(2000,ErrorMessage = "Biography cannot exceed 2000 characters.")]
        public string? Bio { get; set; }
    }
}
