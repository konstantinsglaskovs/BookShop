using BookShop.Application.Publishers.Commands.CreatePublisher;
using BookShop.Application.Publishers.Commands.DeletePublisher;
using BookShop.Application.Publishers.Commands.UpdatePublisher;
using BookShop.Application.Publishers.Queries.GetPublishers;
using BookShop.Web.Models.Publishers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PublisherController : Controller
    {
        private readonly IMediator _mediator;

        public PublisherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search,int page = 1)
        {
            var result = await _mediator.Send(new GetPublishersQuery
            {
                Search = search,
                Page = page,
            });

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePublisherViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var publishers = await _mediator.Send(new GetPublishersQuery());

                return View("Index", publishers);
            }

            var result = await _mediator.Send(new CreatePublisherCommand
            {
                Name = model.Name
            });

            if (result.IsError)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(nameof(model.Name),error.Description);
                }

                var publishers = await _mediator.Send(new GetPublishersQuery());

                return View("Index", publishers);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdatePublisherViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = ModelState
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .FirstOrDefault();

                return RedirectToAction(nameof(Index));
            }

            var result = await _mediator.Send(new UpdatePublisherCommand
            {
                Id = model.Id,
                Name = model.Name
            });

            if (result.IsError)
            {
                TempData["Error"] = result.FirstError.Description;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeletePublisherCommand
            {
                Id = id
            });

            if (result.IsError)
            {
                TempData["Error"] = result.FirstError.Description;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
