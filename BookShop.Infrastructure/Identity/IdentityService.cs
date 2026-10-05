using BookShop.Application.Auth;
using BookShop.Application.Auth.Commands.Login;
using BookShop.Application.Auth.Commands.Register;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Entities;
using BookShop.Infrastructure.Persistance;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BookShop.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AppDbContext _db;

    public IdentityService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,AppDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
    }

    public async Task<ErrorOr<Success>> LoginUserAsync(LoginCommand request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Email,request.Password,request.RememberMe,lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return Result.Success;
        }

        if (result.IsLockedOut)
        {
            return Error.Validation(code: "Auth.LockedOut",description: "Account is temporarily locked. Please try again later.");
        }

        return Error.Validation(code: "Auth.InvalidCredentials",description: "Invalid email or password.");
    }

    public async Task<ErrorOr<Success>> LogoutUserAsync()
    {
        await _signInManager.SignOutAsync();

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> RegisterUserAsync(
    RegisterCommand request)
    {

        await using var transaction =await _db.Database.BeginTransactionAsync();

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors
                .Where(error => error.Code != "DuplicateUserName")
                .Select(error => Error.Validation(code: error.Code,description: error.Description))
                .ToList();

            return errors;
        }

        var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.User);

        if (!roleResult.Succeeded)
        {
            return Error.Failure(code: "Registration.Failed",description: "Registration failed. Please try again later.");
        }

        var profile = new UserProfile
        {
            ApplicationUserId = user.Id,
            Name = request.Name,
            Surname = request.Surname
        };

        _db.UserProfiles.Add(profile);

        await _db.SaveChangesAsync();

        await transaction.CommitAsync();

        return Result.Success;
    }


}
