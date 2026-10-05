using BookShop.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookShop.Infrastructure.Identity;

public class ApplicationUser:IdentityUser<Guid>
{
    public UserProfile? UserProfile { get; set; }
}
