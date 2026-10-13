using BookShop.Application.Cart.Queries.GetCartCount;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.ViewComponents
{
    public class CartIconViewComponent:ViewComponent
    {
        private readonly IMediator _mediator;

        public CartIconViewComponent(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return View(0);
            }

            var result = await _mediator.Send(new GetCartCountQuery());

            if (result.IsError)
            {
                return View(0);
            }

            return View(result.Value);
        }
    }
}
