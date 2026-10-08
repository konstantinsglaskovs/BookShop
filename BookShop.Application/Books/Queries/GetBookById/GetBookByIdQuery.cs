using BookShop.Application.Books.Dtos;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Books.Queries.GetBookById;

public class GetBookByIdQuery:IRequest<ErrorOr<BookDetailsDto>>
{
    public Guid Id { get; set; }
}
