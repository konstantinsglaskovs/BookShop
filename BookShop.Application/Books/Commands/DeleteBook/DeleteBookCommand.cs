using ErrorOr;
using MediatR;

namespace BookShop.Application.Books.Commands.DeleteBook;

public class DeleteBookCommand:IRequest<ErrorOr<Success>>
{
    public Guid Id { get; set; }
}
