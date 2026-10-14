using BookShop.Application.Books.Commands.CreateBook;
using BookShop.Application.Books.Commands.DeleteBook;
using BookShop.Application.Books.Commands.UpdateBook;
using BookShop.Application.Books.Dtos;
using BookShop.Application.Books.Queries.GetAdminBooks;
using BookShop.Application.Books.Queries.GetBookFilterOptions;
using BookShop.Application.Books.Queries.GetBookForEdit;
using BookShop.Application.Сommon.Models;
using BookShop.Web.Models.Books;
using BookShop.Web.Validators.Images;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBookController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ImageFileValidator _imageFileValidator;

        public AdminBookController(IMediator meadiator, ImageFileValidator imageFileValidator)
        {
            _mediator = meadiator;
            _imageFileValidator = imageFileValidator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search,int page = 1)
        {
            var result = await _mediator.Send(new GetAdminBooksQuery
                {
                    Search = search,
                    Page = page,
                });

            var model = new AdminBooksIndexViewModel
            {
                Search = search,
                Page = result.Page,
                TotalPages = result.TotalPages,
                TotalCount = result.TotalCount,

                Books = result.Items
                    .Select(book => new AdminBookListItemViewModel
                    {
                        Id = book.Id,
                        Title = book.Title,
                        Authors = book.Authors,
                        Status = book.Status
                    }).ToList()
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadBookOptions();

            return View(new CreateBookViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookViewModel model)
        {
            if (model.Images.Count == 0)
            {
                ModelState.AddModelError(nameof(model.Images), "At least one image is required.");
            }

            if (!ModelState.IsValid)
            {
                await LoadBookOptions();

                return View(model);
            }

            var images = new List<BookImageInput>();

            foreach (var file in model.Images)
            {
                var validationResult = _imageFileValidator.Validate(file);

                if (validationResult.IsError)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError(nameof(model.Images), error.Description);
                    }

                    await LoadBookOptions();

                    return View(model);
                }

                var validatedImage = validationResult.Value;

                images.Add(new BookImageInput
                {
                    Stream = file.OpenReadStream(),
                    ContentType = validatedImage.ContentType,
                    FileExtension = validatedImage.Extension
                });
            }

            var result = await _mediator.Send(
                new CreateBookCommand
                {
                    Title = model.Title,
                    Description = model.Description,
                    ISBN = model.ISBN,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,

                    CategoryId = model.CategoryId,
                    PublisherId = model.PublisherId,

                    AuthorIds = model.AuthorIds,
                    TagIds = model.TagIds,

                    Images = images
                });

            if (result.IsError)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await LoadBookOptions();

                return View(model);
            }

            return RedirectToAction("Details","Book", new { id = result.Value });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _mediator.Send(new GetBookForEditQuery
                {
                    Id = id
                });

            if (result.IsError)
            {
                TempData["Error"] = result.FirstError.Description;

                return RedirectToAction(nameof(Index));
            }

            await LoadBookOptions();

            var book = result.Value;

            var model = new UpdateBookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                ISBN = book.ISBN,
                Price = book.Price,
                StockQuantity = book.StockQuantity,
                CategoryId = book.CategoryId,
                PublisherId = book.PublisherId,
                AuthorIds = book.AuthorIds,
                TagIds = book.TagIds
            };

            for (var i = 0; i < 5; i++)
            {
                var image = book.Images.ElementAtOrDefault(i);

                model.Images.Add(new BookEditImageViewModel
                {
                    ExistingImageId = image?.Id ?? Guid.Empty,
                    ImageUrl = image?.ImageUrl
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateBookViewModel model)
        {
            if (model.Images.Count == 0 ||(model.Images[0].ExistingImageId == Guid.Empty && model.Images[0].NewImage is null))
            {
                ModelState.AddModelError(nameof(model.Images),"The first image is required.");
            }

            var images = new List<UpdateBookImageInput>();

            if (ModelState.IsValid)
            {
                foreach (var slot in model.Images)
                {
                    BookImageInput? newImage = null;

                    if (slot.NewImage is not null)
                    {
                        var validationResult =_imageFileValidator.Validate(slot.NewImage);

                        if (validationResult.IsError)
                        {
                            foreach (var error in validationResult.Errors)
                            {
                                ModelState.AddModelError(nameof(model.Images),error.Description);
                            }

                            break;
                        }

                        newImage = new BookImageInput
                        {
                            Stream = slot.NewImage.OpenReadStream(),
                            ContentType = validationResult.Value.ContentType,
                            FileExtension = validationResult.Value.Extension
                        };
                    }

                    images.Add(new UpdateBookImageInput
                    {
                        ExistingImageId = slot.ExistingImageId,
                        NewImage = newImage
                    });
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadEditData(model);

                return View(model);
            }

            var result = await _mediator.Send(new UpdateBookCommand
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    ISBN = model.ISBN,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                    CategoryId = model.CategoryId,
                    PublisherId = model.PublisherId,
                    AuthorIds = model.AuthorIds,
                    TagIds = model.TagIds,
                    Images = images
                });

            if (result.IsError)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty,error.Description);
                }

                await LoadEditData(model);

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteBookCommand{Id = id});

            if (result.IsError)
            {
                TempData["Error"] = result.FirstError.Description;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadBookOptions()
        {
            var options = await _mediator.Send(new GetBookFilterOptionsQuery());

            ViewBag.Categories = options.Categories;
            ViewBag.Publishers = options.Publishers;
            ViewBag.Authors = options.Authors;
            ViewBag.Tags = options.Tags;
        }

        private async Task LoadEditData(UpdateBookViewModel model)
        {
            await LoadBookOptions();

            var result = await _mediator.Send(new GetBookForEditQuery
                {
                    Id = model.Id
                });

            if (result.IsError)
            {
                return;
            }

            foreach (var slot in model.Images)
            {
                if (slot.ExistingImageId == Guid.Empty)
                {
                    continue;
                }

                var image = result.Value.Images.FirstOrDefault(image =>image.Id == slot.ExistingImageId);

                if (image is not null)
                {
                    slot.ImageUrl = image.ImageUrl;
                }
            }
        }
    }
}
