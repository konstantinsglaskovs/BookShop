using BookShop.Application.Tags.Dtos;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Application.Сommon.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Tags.Queries.GetTags;

public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, PagedResult<TagDto>>
{
    private readonly IAppDbContext _db;

    public GetTagsQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<TagDto>> Handle(GetTagsQuery request,CancellationToken cancellationToken)
    {
        var query = _db.Tags.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(tag =>tag.Name.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var tags = await query.OrderBy(tag => tag.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(tag => new TagDto
            {
                Id = tag.Id,
                Name = tag.Name
            }).ToListAsync(cancellationToken);

        return new PagedResult<TagDto>
        {
            Items = tags,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
