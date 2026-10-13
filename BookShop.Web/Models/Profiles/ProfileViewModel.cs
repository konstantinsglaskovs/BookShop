namespace BookShop.Web.Models.Profiles
{
    public class ProfileViewModel
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ProfileImageUrl { get; set; }
    }
}
