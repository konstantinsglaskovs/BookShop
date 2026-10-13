using ErrorOr;
using MediatR;

namespace BookShop.Application.Profiles.Commands.RemoveProfileImage;

public class RemoveProfileImageCommand : IRequest<ErrorOr<Success>>
{
}
