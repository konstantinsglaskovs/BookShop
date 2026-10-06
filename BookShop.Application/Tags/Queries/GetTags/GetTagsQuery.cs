using BookShop.Application.Tags.Dtos;
using BookShop.Application.Сommon.Models;
using MediatR;

namespace BookShop.Application.Tags.Queries.GetTags;

public class GetTagsQuery:IRequest<PagedResult<TagDto>>
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
