using BookShop.Application.Authors.Commands.CreateAuthor;
using BookShop.Application.Authors.Commands.DeleteAuthor;
using BookShop.Application.Authors.Commands.UpdateAuthor;
using BookShop.Application.Authors.Queries.GetAuthorById;
using BookShop.Application.Authors.Queries.GetAuthors;
using BookShop.Web.Models.Authors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IMediator _mediator;

        public AuthorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search,int page = 1)
        {
            var result = await _mediator.Send(new GetAuthorsQuery
            {
                Search = search,
                Page = page,
            });

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateAuthorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAuthorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _mediator.Send(new CreateAuthorCommand
            {
                Name = model.Name,
                Bio = model.Bio
            });

            if (result.IsError)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty,error.Description);
                }

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _mediator.Send(new GetAuthorByIdQuery
                {
                    Id = id
                });

            if (result.IsError)
            {
                TempData["Error"] = result.FirstError.Description;

                return RedirectToAction(nameof(Index));
            }

            var model = new UpdateAuthorViewModel
            {
                Id = result.Value.Id,
                Name = result.Value.Name,
                Bio = result.Value.Bio
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateAuthorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _mediator.Send(new UpdateAuthorCommand
                {
                    Id = model.Id,
                    Name = model.Name,
                    Bio = model.Bio
                });

            if (result.IsError)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty,error.Description);
                }

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(
                new DeleteAuthorCommand
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
