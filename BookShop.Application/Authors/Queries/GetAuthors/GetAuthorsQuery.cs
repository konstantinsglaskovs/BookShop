using BookShop.Application.Authors.Dtos;
using BookShop.Application.Сommon.Models;
using MediatR;

namespace BookShop.Application.Authors.Queries.GetAuthors;

public class GetAuthorsQuery: IRequest<PagedResult<AuthorListItemDto>>
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
