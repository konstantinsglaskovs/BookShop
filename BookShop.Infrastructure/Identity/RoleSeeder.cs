using BookShop.Application.Auth;
using Microsoft.AspNetCore.Identity;

namespace BookShop.Infrastructure.Identity;

public  class RoleSeeder
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RoleSeeder(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        foreach (var role in AppRoles.All)
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(role));

            if (result.Succeeded)
            {
                continue;
            }

            var errors = string.Join("; ",result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(errors);
        }
    }
}
