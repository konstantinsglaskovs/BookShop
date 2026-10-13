using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Profiles.Commands.UpdateProfileImage;

public class UpdateProfileImageCommandHandler : IRequestHandler<UpdateProfileImageCommand, ErrorOr<string>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;
    private readonly IFileStorage _fileStorage;

    public UpdateProfileImageCommandHandler(IAppDbContext db,
        IIdentityService identityService, IFileStorage fileStorage)
    {
        _db = db;
        _identityService = identityService;
        _fileStorage = fileStorage;
    }

    
    public async Task<ErrorOr<string>> Handle(UpdateProfileImageCommand request,CancellationToken cancellationToken)
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

        var oldStorageKey =profile.ProfileImageStorageKey;

        var newStorageKey =$"profiles/{profile.Id}/{Guid.NewGuid()}{request.FileExtension}";

        var uploadResult =await _fileStorage.UploadAsync(request.Stream,newStorageKey,request.ContentType);

        if (uploadResult.IsError)
        {
            return uploadResult.Errors;
        }

        profile.ProfileImageStorageKey =newStorageKey;

        await _db.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(oldStorageKey))
        {
            await _fileStorage.DeleteAsync(oldStorageKey);
        }

        return  _fileStorage.GetPublicUrl(newStorageKey);
    }
}
