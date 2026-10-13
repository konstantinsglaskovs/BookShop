using BookShop.Application.Profiles.Dtos;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Profiles.Queries.GetProfile;

public class GetProfileQueryHandler:IRequestHandler<GetProfileQuery,ErrorOr<ProfileDto>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;
    private readonly IFileStorage _fileStorage;

    public GetProfileQueryHandler(IAppDbContext db, IIdentityService identityService, IFileStorage fileStorage)
    {
        _db = db;
        _identityService = identityService;
        _fileStorage = fileStorage;
    }

    public async Task<ErrorOr<ProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var profileResult = await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }



        var profile = await _db.UserProfiles
            .AsNoTracking()
            .Where(profile => profile.Id == profileResult.Value)
            .Select(profile => new
            {
                profile.Name,
                profile.Surname,
                profile.ProfileImageStorageKey
            })
            .FirstOrDefaultAsync();

        var email = await _identityService.GetCurrentUserEmailAsync();

        if (profile is null || email.IsError)
        {
            return Error.NotFound(code: "Profile.NotFound",description: "Profile was not found.");
        }

        return new ProfileDto
        {
            Name = profile.Name,
            Surname = profile.Surname,
            Email = email.Value,

            ProfileImageUrl =profile.ProfileImageStorageKey is null? null: _fileStorage.GetPublicUrl(profile.ProfileImageStorageKey)
        };
    }
}
