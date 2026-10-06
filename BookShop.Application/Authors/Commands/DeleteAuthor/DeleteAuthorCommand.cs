using ErrorOr;
using MediatR;

namespace BookShop.Application.Authors.Commands.DeleteAuthor;

public class DeleteAuthorCommand : IRequest<ErrorOr<Success>>
{
    public Guid Id { get; set; }
}
