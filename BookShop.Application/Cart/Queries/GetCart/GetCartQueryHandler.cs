using BookShop.Application.Cart.Dtos;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Cart.Queries.GetCart;

public class GetCartQueryHandler:IRequestHandler<GetCartQuery,ErrorOr<CartDto>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;
    private readonly IFileStorage _fileStorage;

    public GetCartQueryHandler(IAppDbContext db, IIdentityService identityService, IFileStorage fileStorage)
    {
        _db = db;
        _identityService = identityService;
        _fileStorage = fileStorage;
    }

    public async Task<ErrorOr<CartDto>> Handle(GetCartQuery request,CancellationToken cancellationToken)
    {
        var profileResult = await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        var userProfileId = profileResult.Value;

        var items = await _db.CartItems.AsNoTracking()
            .Where(item => item.UserProfileId == userProfileId)
            .OrderBy(item => item.Book.Title)
            .Select(item => new
            {
                item.Id,
                item.BookId,
                item.Book.Title,
                item.Book.Price,
                item.Quantity,
                item.Book.StockQuantity,

                ImageStorageKey = item.Book.Images
                    .OrderBy(image => image.DisplayOrder)
                    .Select(image => image.StorageKey)
                    .FirstOrDefault()
            })
            .ToListAsync();

        var cartItems = items.Select(item => new CartItemDto
            {
                Id = item.Id,
                BookId = item.BookId,
                Title = item.Title,
                Price = item.Price,
                Quantity = item.Quantity,
                StockQuantity = item.StockQuantity,

                ImageUrl = item.ImageStorageKey is null? null: _fileStorage.GetPublicUrl(item.ImageStorageKey),

                Subtotal = item.Price * item.Quantity
            }).ToList();

        return new CartDto
        {
            Items = cartItems,
            Total = cartItems.Sum(item => item.Subtotal)
        };
    }
}
