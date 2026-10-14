using BookShop.Application.Orders.Admins.Commands.UpdateOrder;
using BookShop.Application.Orders.Admins.Queries.GetAdminOrderById;
using BookShop.Application.Orders.Admins.Queries.GetAdminOrders;
using BookShop.Domain.Enums;
using BookShop.Web.Models.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly IMediator _mediator;

        public AdminOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search,OrderStatus? orderStatus,PaymentStatus? paymentStatus,int page = 1)
        {
            var result = await _mediator.Send(new GetAdminOrdersQuery
                {
                    Search = search,
                    OrderStatus = orderStatus,
                    PaymentStatus = paymentStatus,
                    Page = page,
                    PageSize = 10
                });

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;

                return View(new AdminOrdersViewModel());
            }

            ViewBag.Search = search;
            ViewBag.OrderStatus = orderStatus;
            ViewBag.PaymentStatus = paymentStatus;

            var model = new AdminOrdersViewModel
            {
                Page = result.Value.Page,
                TotalPages = result.Value.TotalPages,
                TotalCount = result.Value.TotalCount,

                Orders = result.Value.Items
                    .Select(order => new AdminOrderListItemViewModel
                    {
                        Id = order.Id,
                        RecipientName = order.RecipientName,
                        RecipientSurname = order.RecipientSurname,
                        OrderDate = order.OrderDate,
                        Total = order.Total,
                        OrderStatus = order.OrderStatus,
                        PaymentStatus = order.PaymentStatus,
                        ItemsCount = order.ItemsCount
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _mediator.Send(new GetAdminOrderByIdQuery{Id = id});

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;

                return RedirectToAction(nameof(Index));
            }

            var order = result.Value;
            ViewBag.Order = order;

            var model = new UpdateOrderViewModel
            {
                Id = order.Id,
                OrderStatus = order.OrderStatus,
                TrackingNumber = order.TrackingNumber,
                Carrier = order.Carrier,
            };


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var orderResult = await _mediator.Send(new GetAdminOrderByIdQuery{Id = model.Id});

                if (!orderResult.IsError)
                {
                    ViewBag.Order = orderResult.Value;
                }

                return View(model);
            }

            var result = await _mediator.Send(new UpdateOrderCommand
                {
                    Id = model.Id,
                    OrderStatus = model.OrderStatus,
                    TrackingNumber = model.TrackingNumber,
                    Carrier = model.Carrier
                });

            if (result.IsError)
            {
                ModelState.AddModelError(string.Empty,result.FirstError.Description);

                var orderResult = await _mediator.Send(new GetAdminOrderByIdQuery { Id = model.Id });

                if (!orderResult.IsError)
                {
                    ViewBag.Order = orderResult.Value;
                }

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
