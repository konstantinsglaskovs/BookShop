using ErrorOr;
using MediatR;

namespace BookShop.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand:IRequest<ErrorOr<Success>>
{
    public Guid Id { get; set; }
}
