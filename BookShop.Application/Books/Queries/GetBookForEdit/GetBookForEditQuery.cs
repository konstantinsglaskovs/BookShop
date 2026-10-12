using BookShop.Application.Books.Dtos;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Books.Queries.GetBookForEdit;

public class GetBookForEditQuery: IRequest<ErrorOr<BookForEditDto>>
{
    public Guid Id { get; set; }
}
