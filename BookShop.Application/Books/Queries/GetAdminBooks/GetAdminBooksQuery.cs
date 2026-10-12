using BookShop.Application.Books.Dtos;
using BookShop.Application.Сommon.Models;
using MediatR;

namespace BookShop.Application.Books.Queries.GetAdminBooks;

public class GetAdminBooksQuery:IRequest<PagedResult<AdminBookListItemDto>>
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
