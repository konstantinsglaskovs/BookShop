using BookShop.Application.Books.Dtos;
using MediatR;

namespace BookShop.Application.Books.Queries.GetBookFilterOptions;

public class GetBookFilterOptionsQuery : IRequest<BookFilterOptionsDto>
{

}
