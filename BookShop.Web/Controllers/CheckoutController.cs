using BookShop.Application.Cart.Queries.GetCart;
using BookShop.Application.Orders.Users.Commands.CancelOrder;
using BookShop.Application.Orders.Users.Commands.ConfirmPayment;
using BookShop.Application.Orders.Users.Commands.CreateOrder;
using BookShop.Web.Models.Checkout;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IMediator _mediator;

        public CheckoutController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartResult = await _mediator.Send(new GetCartQuery());

            if (cartResult.IsError)
            {
                TempData["Error"] =cartResult.FirstError.Description;

                return RedirectToAction("Index","Cart");
            }

            if (cartResult.Value.Items.Count == 0)
            {
                return RedirectToAction("Index","Cart");
            }

            var model = new CheckoutViewModel
            {
                Cart = cartResult.Value
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsError)
            {
                TempData["Error"] = result.FirstError.Description;

                return RedirectToAction("Index","Cart");
            }

            return Redirect(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Success(string session_id)
        {
            var result = await _mediator.Send(new ConfirmPaymentCommand{SessionId = session_id});

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;

                return RedirectToAction("Index","Book");
            }

            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(Guid orderId)
        {
            var result = await _mediator.Send(new CancelOrderCommand{OrderId = orderId});

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;
            }

            return RedirectToAction("Index","Cart");
        }
    }
}
