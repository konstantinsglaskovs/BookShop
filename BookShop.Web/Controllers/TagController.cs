using BookShop.Application.Tags.Commands.CreateTag;
using BookShop.Application.Tags.Commands.DeleteTag;
using BookShop.Application.Tags.Commands.UpdateTag;
using BookShop.Application.Tags.Queries.GetTags;
using BookShop.Web.Models.Tags;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    public class TagController : Controller
    {
        private readonly IMediator _mediator;

        public TagController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search,int page = 1)
        {
            var result = await _mediator.Send(new GetTagsQuery
            {
                Search = search,
                Page = page,
            });

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTagViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var tags = await _mediator.Send(new GetTagsQuery());

                return View("Index", tags);
            }

            var result = await _mediator.Send(new CreateTagCommand
            {
                Name = model.Name
            });

            if (result.IsError)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(nameof(model.Name),error.Description);
                }

                var tags = await _mediator.Send(new GetTagsQuery());

                return View("Index", tags);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateTagViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = ModelState
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .FirstOrDefault();

                return RedirectToAction(nameof(Index));
            }

            var result = await _mediator.Send(new UpdateTagCommand
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
            var result = await _mediator.Send(new DeleteTagCommand
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
