using BookShop.Application.Categories.Dtos;
using BookShop.Application.Сommon.Models;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Categories.Queries.GetCategories;

public class GetCategoriesQuery:IRequest<PagedResult<CategoryDto>>
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
