using System.ComponentModel.DataAnnotations;

namespace BookShop.Web.Models.Tags
{
    public class UpdateTagViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tag name is required.")]
        [StringLength(100,MinimumLength = 2,ErrorMessage = "Tag name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;
    }
}
