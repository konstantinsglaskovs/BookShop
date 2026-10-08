using BookShop.Application.Books.Dtos;
using BookShop.Application.Сommon.Models;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Books.Queries.GetBooks;

public class GetBooksQuery:IRequest<PagedResult<BookListItemDto>>
{
    public string? Search { get; set; }
    public List<Guid> CategoryIds { get; set; } = [];
    public List<Guid> PublisherIds { get; set; } = [];
    public List<Guid> AuthorIds { get; set; } = [];
    public List<Guid> TagIds { get; set; } = [];
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
