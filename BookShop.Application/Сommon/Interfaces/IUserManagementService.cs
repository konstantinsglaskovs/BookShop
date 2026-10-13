using BookShop.Application.UserManagement.Dtos;
using ErrorOr;

namespace BookShop.Application.Сommon.Interfaces;

public interface IUserManagementService
{
    Task<ErrorOr<List<UserListItemDto>>> GetUsersAsync(string? search);

    Task<ErrorOr<Success>> ChangeUserRoleAsync(Guid userId,string role);

    Task<ErrorOr<Success>> SetUserLockAsync(Guid userId,bool locked);
}
