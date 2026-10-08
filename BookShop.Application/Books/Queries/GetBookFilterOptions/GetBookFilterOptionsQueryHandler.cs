using BookShop.Application.Books.Dtos;
using BookShop.Application.Сommon.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Books.Queries.GetBookFilterOptions;

public class GetBookFilterOptionsQueryHandler : IRequestHandler<GetBookFilterOptionsQuery, BookFilterOptionsDto>
{
    private readonly IAppDbContext _db;

    public GetBookFilterOptionsQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<BookFilterOptionsDto> Handle(GetBookFilterOptionsQuery request,CancellationToken cancellationToken)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new FilterOptionDto
            {
                Id = category.Id,
                Name = category.Name
            })
            .ToListAsync();

        var publishers = await _db.Publishers
            .AsNoTracking()
            .OrderBy(publisher => publisher.Name)
            .Select(publisher => new FilterOptionDto
            {
                Id = publisher.Id,
                Name = publisher.Name
            })
            .ToListAsync();

        var authors = await _db.Authors
            .AsNoTracking()
            .OrderBy(author => author.Name)
            .Select(author => new FilterOptionDto
            {
                Id = author.Id,
                Name = author.Name
            })
            .ToListAsync();

        var tags = await _db.Tags
            .AsNoTracking()
            .OrderBy(tag => tag.Name)
            .Select(tag => new FilterOptionDto
            {
                Id = tag.Id,
                Name = tag.Name
            })
            .ToListAsync();

        return new BookFilterOptionsDto
        {
            Categories = categories,
            Publishers = publishers,
            Authors = authors,
            Tags = tags
        };
    }
}
