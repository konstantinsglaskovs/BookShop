using BookShop.Application.Auth;
using BookShop.Domain.Entities;
using BookShop.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BookShop.Infrastructure.Identity;

public class AdminSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _db;
    private readonly IConfiguration _configuration;

    public AdminSeeder(UserManager<ApplicationUser> userManager,AppDbContext db,IConfiguration configuration)
    {
        _userManager = userManager;
        _db = db;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        var email = _configuration["AdminSeed:Email"];
        var password = _configuration["AdminSeed:Password"];
        var name = _configuration["AdminSeed:Name"];
        var surname = _configuration["AdminSeed:Surname"];

        if (string.IsNullOrWhiteSpace(email) ||string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            return;
        }

        user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var createResult =await _userManager.CreateAsync(user,password);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ",createResult.Errors.Select(x => x.Description));

            throw new InvalidOperationException($"Failed to seed admin user: {errors}");
        }

        await _userManager.AddToRoleAsync(user,AppRoles.Admin);

        _db.UserProfiles.Add(new UserProfile
            {
                ApplicationUserId = user.Id,
                Name = name ?? "Admin",
                Surname = surname ?? "User"
            });

        await _db.SaveChangesAsync();
    }
}
