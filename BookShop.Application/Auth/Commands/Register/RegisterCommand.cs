using ErrorOr;
using MediatR;

namespace BookShop.Application.Auth.Commands.Register;

public class RegisterCommand:IRequest<ErrorOr<Success>>
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string ConfirmPassword { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
}
