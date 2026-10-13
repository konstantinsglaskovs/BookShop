using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Profiles.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;

    public UpdateProfileCommandHandler(IAppDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateProfileCommand request,CancellationToken cancellationToken)
    {
        var profileResult =await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        var profile = await _db.UserProfiles.FirstOrDefaultAsync(profile =>profile.Id == profileResult.Value);

        if (profile is null)
        {
            return Error.NotFound(code: "Profile.NotFound",description: "Profile was not found.");
        }

        profile.Name = request.Name;
        profile.Surname = request.Surname;

        await _db.SaveChangesAsync();

        return Result.Success;
    }
}
