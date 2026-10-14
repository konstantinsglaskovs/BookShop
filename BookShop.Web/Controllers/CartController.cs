using BookShop.Application.Cart.Commands.AddToCart;
using BookShop.Application.Cart.Commands.ChangeCartItemQuantity;
using BookShop.Application.Cart.Commands.RemoveCartItem;
using BookShop.Application.Cart.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetCartQuery());

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;

                return RedirectToAction("Index","Book");
            }

            return View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Guid bookId,int quantity = 1, string? returnUrl = null)
        {
            var result = await _mediator.Send(new AddToCartCommand
                {
                    BookId = bookId,
                    Quantity = quantity
                });

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index","Book");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeQuantity(Guid cartItemId,int change)
        {
            var result = await _mediator.Send(new ChangeCartItemQuantityCommand
                {
                    CartItemId = cartItemId,
                    QuantityChange = change
                });

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid cartItemId)
        {
            var result = await _mediator.Send(new RemoveCartItemCommand{CartItemId = cartItemId});

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
