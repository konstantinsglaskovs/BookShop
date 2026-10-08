using BookShop.Application.Authors.Queries.GetAuthors;
using BookShop.Application.Books.Queries.GetBookFilterOptions;
using BookShop.Application.Books.Queries.GetBooks;
using BookShop.Application.Categories.Queries.GetCategories;
using BookShop.Application.Publishers.Queries.GetPublishers;
using BookShop.Application.Tags.Queries.GetTags;
using BookShop.Web.Models.Books;
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
        var booksResult = await _mediator.Send(new GetBooksQuery
        {
            Search = model.Search,
            CategoryIds = model.CategoryIds,
            PublisherIds = model.PublisherIds,
            AuthorIds = model.AuthorIds,
            TagIds = model.TagIds,
            Page = model.Page,
            PageSize = 6
        });

        var filters = await _mediator.Send(new GetBookFilterOptionsQuery());

        model.Books = booksResult.Items
            .Select(book => new BookListItemViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Price = book.Price,
                Authors = book.Authors,
                ImageUrl = book.ImageUrl
            })
            .ToList();

        model.Categories = filters.Categories
            .Select(x => new BookFilterOptionViewModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

        model.Publishers = filters.Publishers
            .Select(x => new BookFilterOptionViewModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

        model.Authors = filters.Authors
            .Select(x => new BookFilterOptionViewModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

        model.Tags = filters.Tags
            .Select(x => new BookFilterOptionViewModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

        model.TotalCount = booksResult.TotalCount;
        model.TotalPages = booksResult.TotalPages;

        return View(model);
    }
}
