using ErrorOr;
using MediatR;

namespace BookShop.Application.UserManagement.Commands.ChangeUserRole;

public class ChangeUserRoleCommand : IRequest<ErrorOr<Success>>
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = null!;
}
