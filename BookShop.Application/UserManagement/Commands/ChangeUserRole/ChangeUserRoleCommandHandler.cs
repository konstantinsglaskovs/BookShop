using BookShop.Application.Auth;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;

namespace BookShop.Application.UserManagement.Commands.ChangeUserRole;

public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, ErrorOr<Success>>
{
    private readonly IUserManagementService _userManagementService;

    public ChangeUserRoleCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<ErrorOr<Success>> Handle(ChangeUserRoleCommand request,CancellationToken cancellationToken)
    {
        if (request.Role != AppRoles.User && request.Role != AppRoles.Admin)
        {
            return Error.Validation(code: "User.InvalidRole",description: "Invalid user role.");
        }

        return await _userManagementService.ChangeUserRoleAsync(request.UserId,request.Role);
    }
}
