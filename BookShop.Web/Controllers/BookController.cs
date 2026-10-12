using BookShop.Application.Books.Commands.CreateBook;
using BookShop.Application.Books.Queries.GetBookById;
using BookShop.Application.Books.Queries.GetBookFilterOptions;
using BookShop.Application.Books.Queries.GetBooks;
using BookShop.Application.Сommon.Models;
using BookShop.Web.Models.Books;
using BookShop.Web.Validators.Images;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers;

public class BookController : Controller
{
    private readonly IMediator _mediator;
    

    public BookController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(BooksIndexViewModel model)
    {
        var books = await _mediator.Send(new GetBooksQuery
        {
            Search = model.Search,
            CategoryIds = model.CategoryIds,
            PublisherIds = model.PublisherIds,
            AuthorIds = model.AuthorIds,
            TagIds = model.TagIds,
            Page = model.Page
        });

        await LoadBookOptions();

        model.Books = books.Items
            .Select(book => new BookListItemViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Price = book.Price,
                Authors = book.Authors,
                ImageUrl = book.ImageUrl
            }).ToList();

        model.TotalCount = books.TotalCount;
        model.TotalPages = books.TotalPages;
        model.Page = books.Page;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _mediator.Send(
            new GetBookByIdQuery
            {
                Id = id
            });

        if (result.IsError)
        {
            TempData["Error"] = result.FirstError.Description;

            return RedirectToAction(nameof(Index));
        }

        var book = result.Value;

        var model = new BookDetailsViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            ISBN = book.ISBN,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            CategoryName = book.CategoryName,
            PublisherName = book.PublisherName,
            Authors = book.Authors,
            Tags = book.Tags,
            CategoryId = book.CategoryId,

            Images = book.Images
                .Select(image => new BookDetailsImageViewModel
                {
                    ImageUrl = image.ImageUrl,
                    DisplayOrder = image.DisplayOrder
                })
                .ToList()
        };

        return View(model);
    }

    private async Task LoadBookOptions()
    {
        var options = await _mediator.Send(new GetBookFilterOptionsQuery());

        ViewBag.Categories = options.Categories;
        ViewBag.Publishers = options.Publishers;
        ViewBag.Authors = options.Authors;
        ViewBag.Tags = options.Tags;
    }
}
