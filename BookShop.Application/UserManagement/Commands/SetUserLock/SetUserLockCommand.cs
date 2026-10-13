using ErrorOr;
using MediatR;

namespace BookShop.Application.UserManagement.Commands.SetUserLock;

public class SetUserLockCommand : IRequest<ErrorOr<Success>>
{
    public Guid UserId { get; set; }
    public bool Locked { get; set; }
}
