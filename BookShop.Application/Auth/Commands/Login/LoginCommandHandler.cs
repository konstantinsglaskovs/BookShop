using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<Success>>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public Task<ErrorOr<Success>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return _identityService.LoginUserAsync(request); ;
    }
}
