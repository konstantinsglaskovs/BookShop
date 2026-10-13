using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Profiles.Commands.RemoveProfileImage;

public class RemoveProfileImageCommandHandler : IRequestHandler<RemoveProfileImageCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;
    private readonly IFileStorage _fileStorage;

    public RemoveProfileImageCommandHandler(IAppDbContext db, IIdentityService identityService, IFileStorage fileStorage)
    {
        _db = db;
        _identityService = identityService;
        _fileStorage = fileStorage;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveProfileImageCommand request,CancellationToken cancellationToken)
    {
        var profileResult =await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        var profile = await _db.UserProfiles
            .FirstOrDefaultAsync(profile =>profile.Id == profileResult.Value);

        if (profile is null)
        {
            return Error.NotFound(code: "Profile.NotFound",description: "Profile was not found.");
        }

        if (string.IsNullOrWhiteSpace(profile.ProfileImageStorageKey))
        {
            return Result.Success;
        }

        var storageKey =profile.ProfileImageStorageKey;

        profile.ProfileImageStorageKey = null;

        await _db.SaveChangesAsync();

        await _fileStorage.DeleteAsync(storageKey);

        return Result.Success;
    }
}
