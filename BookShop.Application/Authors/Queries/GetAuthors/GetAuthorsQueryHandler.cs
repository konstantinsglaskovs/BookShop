using BookShop.Application.Authors.Dtos;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Application.Сommon.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Authors.Queries.GetAuthors;

public class GetAuthorsQueryHandler : IRequestHandler<GetAuthorsQuery, PagedResult<AuthorListItemDto>>
{
    private readonly IAppDbContext _db;

    public GetAuthorsQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<AuthorListItemDto>> Handle(GetAuthorsQuery request,CancellationToken cancellationToken)
    {
        var query = _db.Authors.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(author => author.Name.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var authors = await query.OrderBy(author => author.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(author => new AuthorListItemDto
            {
                Id = author.Id,
                Name = author.Name
            }).ToListAsync(cancellationToken);

        return new PagedResult<AuthorListItemDto>
        {
            Items = authors,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
