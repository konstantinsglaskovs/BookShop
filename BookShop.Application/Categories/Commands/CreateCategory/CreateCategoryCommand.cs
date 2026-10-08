using ErrorOr;
using MediatR;

namespace BookShop.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommand:IRequest<ErrorOr<Success>>
{
    public string Name { get; set; } = null!;
}
