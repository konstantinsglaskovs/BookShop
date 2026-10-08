using System.ComponentModel.DataAnnotations;

namespace BookShop.Web.Models.Publishers
{
    public class UpdatePublisherViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Publisher name is required.")]
        [StringLength(100,MinimumLength = 2,ErrorMessage = "Publisher name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;
    }
}
