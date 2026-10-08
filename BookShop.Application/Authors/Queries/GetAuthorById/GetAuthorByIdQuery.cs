using BookShop.Application.Authors.Dtos;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Authors.Queries.GetAuthorById;

public class GetAuthorByIdQuery: IRequest<ErrorOr<AuthorDetailsDto>>
{
    public Guid Id { get; set; }
}
