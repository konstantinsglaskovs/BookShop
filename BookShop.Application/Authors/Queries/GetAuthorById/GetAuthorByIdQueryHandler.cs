using BookShop.Application.Authors.Dtos;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Authors.Queries.GetAuthorById;

public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, ErrorOr<AuthorDetailsDto>>
{
    private readonly IAppDbContext _db;

    public GetAuthorByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<AuthorDetailsDto>> Handle(GetAuthorByIdQuery request,CancellationToken cancellationToken)
    {
        var author = await _db.Authors.AsNoTracking()
            .Where(author => author.Id == request.Id)
            .Select(author => new AuthorDetailsDto
            {
                Id = author.Id,
                Name = author.Name,
                Bio = author.Bio
            }).FirstOrDefaultAsync();

        if (author is null)
        {
            return Error.NotFound(code: "Author.NotFound",description: "Author was not found.");
        }

        return author;
    }
}
