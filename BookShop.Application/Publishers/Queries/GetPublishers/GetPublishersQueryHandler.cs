using BookShop.Application.Publishers.Dtos;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Application.Сommon.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Publishers.Queries.GetPublishers;

public class GetPublishersQueryHandler:IRequestHandler<GetPublishersQuery, PagedResult<PublisherDto>>
{
    private readonly IAppDbContext _db;

    public GetPublishersQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<PublisherDto>> Handle(GetPublishersQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Publishers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(publisher => publisher.Name.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var publishers = await query.OrderBy(publisher => publisher.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(publisher => new PublisherDto
            {
                Id = publisher.Id,
                Name = publisher.Name
            }).ToListAsync(cancellationToken);

        return new PagedResult<PublisherDto>
        {
            Items = publishers,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
