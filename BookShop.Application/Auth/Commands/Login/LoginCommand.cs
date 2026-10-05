using ErrorOr;
using MediatR;

namespace BookShop.Application.Auth.Commands.Login;

public class LoginCommand:IRequest<ErrorOr<Success>>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; }
}
