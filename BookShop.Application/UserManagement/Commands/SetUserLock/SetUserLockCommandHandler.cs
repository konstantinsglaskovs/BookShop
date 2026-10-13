using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;

namespace BookShop.Application.UserManagement.Commands.SetUserLock;

public class SetUserLockCommandHandler : IRequestHandler<SetUserLockCommand, ErrorOr<Success>>
{
    private readonly IUserManagementService _userManagementService;

    public SetUserLockCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<ErrorOr<Success>> Handle(SetUserLockCommand request,CancellationToken cancellationToken)
    {
        return await _userManagementService.SetUserLockAsync(request.UserId,request.Locked);
    }
}
