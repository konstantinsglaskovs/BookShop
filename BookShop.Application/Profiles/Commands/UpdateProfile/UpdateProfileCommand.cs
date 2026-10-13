using ErrorOr;
using MediatR;

namespace BookShop.Application.Profiles.Commands.UpdateProfile;

public class UpdateProfileCommand : IRequest<ErrorOr<Success>>
{
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
}
