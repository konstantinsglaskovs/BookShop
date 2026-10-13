using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Cart.Commands.RemoveCartItem;

public class RemoveCartItemCommandHandler:IRequestHandler<RemoveCartItemCommand,ErrorOr<Success>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;

    public RemoveCartItemCommandHandler(IAppDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveCartItemCommand request,CancellationToken cancellationToken)
    {
        var profileResult = await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }


        var cartItem = await _db.CartItems
            .FirstOrDefaultAsync(item => item.Id == request.CartItemId && item.UserProfileId == profileResult.Value);

        if (cartItem is null)
        {
            return Error.NotFound(code: "Cart.ItemNotFound",description: "Cart item was not found.");
        }

        _db.CartItems.Remove(cartItem);

        await _db.SaveChangesAsync();

        return Result.Success;
    }
}
