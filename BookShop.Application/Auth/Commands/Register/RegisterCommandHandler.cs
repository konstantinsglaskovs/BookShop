using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<Success>>
    {
        private readonly IIdentityService _identityService;

        public RegisterCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<ErrorOr<Success>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ConfirmPassword)
            {
                return Error.Validation(code: "Auth.PasswordMismatch",description: "Passwords do not match.");
            }

            return await _identityService.RegisterUserAsync(request);
        }
    }
}
