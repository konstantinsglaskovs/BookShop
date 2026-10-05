using ErrorOr;
using MediatR;

namespace BookShop.Application.Auth.Commands.Logout;

public class LogoutCommand : IRequest<ErrorOr<Success>>
{

}
