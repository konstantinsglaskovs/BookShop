using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Entities;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Cart.Commands.AddToCart;

public class AddToCartCommandHandler:IRequestHandler<AddToCartCommand,ErrorOr<Success>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;

    public AddToCartCommandHandler(IAppDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<ErrorOr<Success>> Handle(AddToCartCommand request,CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            return Error.Validation(code: "Cart.InvalidQuantity",description: "Quantity must be greater than zero.");
        }

        var profileResult =await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        var userProfileId = profileResult.Value;

        var book = await _db.Books.AsNoTracking()
            .Where(book =>book.Id == request.BookId &&book.Status == BookStatus.Active)
            .Select(book => new
            {
                book.Id,
                book.StockQuantity
            }).FirstOrDefaultAsync();

        if (book is null)
        {
            return Error.NotFound(code: "Cart.BookNotFound",description: "Book was not found.");
        }

        var cartItem = await _db.CartItems.FirstOrDefaultAsync(item =>item.UserProfileId == userProfileId && item.BookId == request.BookId);

        if (cartItem is null)
        {
            if (request.Quantity > book.StockQuantity)
            {
                return Error.Validation(code: "Cart.NotEnoughStock",description: "Not enough books in stock.");
            }

            _db.CartItems.Add(new CartItem
            {
                UserProfileId = userProfileId,
                BookId = request.BookId,
                Quantity = request.Quantity
            });
        }
        else
        {
            var newQuantity = cartItem.Quantity + request.Quantity;

            if (newQuantity > book.StockQuantity)
            {
                return Error.Validation(code: "Cart.NotEnoughStock",description: "Not enough books in stock.");
            }

            cartItem.Quantity = newQuantity;
        }

        await _db.SaveChangesAsync();

        return Result.Success;
    }
}
