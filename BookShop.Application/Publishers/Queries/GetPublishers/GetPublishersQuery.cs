using BookShop.Application.Publishers.Dtos;
using BookShop.Application.Сommon.Models;
using MediatR;

namespace BookShop.Application.Publishers.Queries.GetPublishers;

public class GetPublishersQuery:IRequest<PagedResult<PublisherDto>>
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
