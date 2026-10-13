using BookShop.Application.Auth.Commands.Login;
using BookShop.Application.Auth.Commands.Register;
using ErrorOr;

namespace BookShop.Application.Сommon.Interfaces;

public interface IIdentityService
{
    Task<ErrorOr<Success>> RegisterUserAsync(RegisterCommand request);

    Task<ErrorOr<Success>> LoginUserAsync(LoginCommand request);

    Task<ErrorOr<Success>> LogoutUserAsync();

    Task<ErrorOr<Guid>> GetCurrentUserProfileIdAsync();
    Task<ErrorOr<string>> GetCurrentUserEmailAsync();
}
