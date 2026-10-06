using BookShop.Application.Categories.Dtos;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Application.Сommon.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryDto>>
{   
    private readonly IAppDbContext _db;

    public GetCategoriesQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<CategoryDto>> Handle(GetCategoriesQuery request,CancellationToken cancellationToken)
    {
        var query = _db.Categories.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(category =>category.Name.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var categories = await query.OrderBy(category => category.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            }).ToListAsync();

        return new PagedResult<CategoryDto>
        {
            Items = categories,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
