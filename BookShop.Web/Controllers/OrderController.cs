using BookShop.Application.Orders.Users.Queries.GetOrderById;
using BookShop.Application.Orders.Users.Queries.GetUserOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    public class OrderController : Controller
    {

        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetUserOrdersQuery());

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;

                return RedirectToAction("Index","Book");
            }

            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _mediator.Send(new GetUserOrderByIdQuery{Id = id});

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;

                return RedirectToAction(nameof(Index));
            }

            return View(result.Value);
        }

    }
}
