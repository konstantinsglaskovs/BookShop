using ErrorOr;
using MediatR;

namespace BookShop.Application.Profiles.Commands.UpdateProfileImage;

public class UpdateProfileImageCommand : IRequest<ErrorOr<string>>
{
    public Stream Stream { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string FileExtension { get; set; } = null!;
}
