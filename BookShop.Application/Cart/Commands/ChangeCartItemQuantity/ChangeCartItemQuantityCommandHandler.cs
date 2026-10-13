using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Cart.Commands.ChangeCartItemQuantity;

public class ChangeCartItemQuantityCommandHandler:IRequestHandler<ChangeCartItemQuantityCommand,ErrorOr<Success>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;

    public ChangeCartItemQuantityCommandHandler(IAppDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<ErrorOr<Success>> Handle(ChangeCartItemQuantityCommand request,CancellationToken cancellationToken)
    {
        if (request.QuantityChange != 1 && request.QuantityChange != -1)
        {
            return Error.Validation(code: "Cart.InvalidQuantityChange",description: "Quantity can only be increased or decreased by one.");
        }

        var profileResult = await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        var userProfileId = profileResult.Value;

        var cartItem = await _db.CartItems
            .Include(item => item.Book)
            .FirstOrDefaultAsync(item =>item.Id == request.CartItemId &&item.UserProfileId == userProfileId);

        if (cartItem is null)
        {
            return Error.NotFound(code: "Cart.ItemNotFound",description: "Cart item was not found.");
        }

        var newQuantity =cartItem.Quantity + request.QuantityChange;

        if (newQuantity <= 0)
        {
            _db.CartItems.Remove(cartItem);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }

        if (request.QuantityChange == 1)
        {
            if (cartItem.Book.Status != BookStatus.Active)
            {
                return Error.Validation(code: "Cart.BookUnavailable",description: "This book is no longer available.");
            }

            if (newQuantity > cartItem.Book.StockQuantity)
            {
                return Error.Validation(code: "Cart.NotEnoughStock",description: "Not enough books in stock.");
            }
        }

        cartItem.Quantity = newQuantity;

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
