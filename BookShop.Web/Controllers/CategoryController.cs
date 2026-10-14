using BookShop.Application.Categories.Commands.CreateCategory;
using BookShop.Application.Categories.Commands.DeleteCategory;
using BookShop.Application.Categories.Commands.UpdateCategory;
using BookShop.Application.Categories.Dtos;
using BookShop.Application.Categories.Queries.GetCategories;
using BookShop.Application.Сommon.Models;
using BookShop.Web.Models.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search,int page = 1)
        {
            var result = await _mediator.Send(new GetCategoriesQuery
            {
                Search = search,
                Page = page
            });

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _mediator.Send(new GetCategoriesQuery());

                return View("Index", categories);
            }

            var result = await _mediator.Send(new CreateCategoryCommand
                {
                    Name = model.Name
                });

            if (result.IsError)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(nameof(model.Name),error.Description);
                }

                var categories = await _mediator.Send(new GetCategoriesQuery());

                return View("Index", categories);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = ModelState
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .FirstOrDefault();

                return RedirectToAction(nameof(Index));
            }

            var result = await _mediator.Send(new UpdateCategoryCommand
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
            var result = await _mediator.Send(new DeleteCategoryCommand{Id = id});

            if (result.IsError)
            {
                TempData["Error"] = result.FirstError.Description;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
