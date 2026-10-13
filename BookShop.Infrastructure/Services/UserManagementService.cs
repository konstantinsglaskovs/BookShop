using BookShop.Application.Auth;
using BookShop.Application.UserManagement.Dtos;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Infrastructure.Identity;
using BookShop.Infrastructure.Persistance;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Infrastructure.Services;

public class UserManagementService : IUserManagementService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserManagementService(AppDbContext db, UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ErrorOr<Success>> ChangeUserRoleAsync(Guid userId, string role)
    {
        var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Error.NotFound(code: "User.NotFound", description: "User was not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var currentRole = roles.FirstOrDefault();

        if (currentUser is not null && currentUser.Id == userId &&
            currentRole == AppRoles.Admin && role != AppRoles.Admin)
        {
            return Error.Validation(
                code: "User.CannotRemoveOwnAdminRole",
                description: "You cannot remove your own admin role.");
        }

        if (currentRole == role)
        {
            return Result.Success;
        }

        if (currentRole is not null)
        {
            var removeResult =await _userManager.RemoveFromRoleAsync(user,currentRole);

            if (!removeResult.Succeeded)
            {
                return Error.Failure(code: "User.RoleUpdateFailed",description: "Failed to update user role.");
            }
        }

        var addResult =await _userManager.AddToRoleAsync(user,role);

        if (!addResult.Succeeded)
        {
            return Error.Failure(code: "User.RoleUpdateFailed",description: "Failed to update user role.");
        }

        return Result.Success;
    }

    public async Task<ErrorOr<List<UserListItemDto>>> GetUsersAsync(string? search)
    {
        var users = await _userManager.Users.AsNoTracking().OrderBy(user => user.Email).ToListAsync();

        var profiles = await _db.UserProfiles.AsNoTracking().ToListAsync();

        var result = new List<UserListItemDto>();

        foreach (var user in users)
        {
            var profile = profiles.FirstOrDefault(profile => profile.ApplicationUserId == user.Id);

            if (profile is null)
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim().ToLower();

                var matches =(user.Email?.ToLower().Contains(normalizedSearch) ?? false) ||profile.Name.ToLower().Contains(normalizedSearch) || profile.Surname.ToLower().Contains(normalizedSearch);

                if (!matches)
                {
                    continue;
                }
            }

            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserListItemDto
            {
                Id = user.Id,
                Email = user.Email!,
                Name = profile.Name,
                Surname = profile.Surname,
                Role = roles.FirstOrDefault() ?? "User",
                IsLocked =user.LockoutEnabled &&user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow
            });
        }

        return result;
    }

    public async Task<ErrorOr<Success>> SetUserLockAsync(Guid userId, bool locked)
    {
        var currentUser =await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);

        if (currentUser is not null &&currentUser.Id == userId)
        {
            return Error.Validation(code: "User.CannotLockSelf",description: "You cannot lock your own account.");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Error.NotFound(code: "User.NotFound",description: "User was not found.");
        }


        user.LockoutEnd = locked? DateTimeOffset.MaxValue: null;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ",result.Errors.Select(x => x.Description));

            return Error.Failure(code: "User.LockUpdateFailed",description: errors);
        }

        return Result.Success;
    }
}
