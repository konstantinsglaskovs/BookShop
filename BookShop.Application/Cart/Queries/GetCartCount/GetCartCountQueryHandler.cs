using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Cart.Queries.GetCartCount;

public class GetCartCountQueryHandler:IRequestHandler<GetCartCountQuery,ErrorOr<int>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;

    public GetCartCountQueryHandler(IAppDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<ErrorOr<int>> Handle(GetCartCountQuery request,CancellationToken cancellationToken)
    {
        var profileResult =await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        var userProfileId = profileResult.Value;

        var count = await _db.CartItems.Where(item =>item.UserProfileId == userProfileId).CountAsync();

        return count;
    }
}
