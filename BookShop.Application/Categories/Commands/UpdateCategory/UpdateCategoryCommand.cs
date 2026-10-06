using ErrorOr;
using MediatR;

namespace BookShop.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand:IRequest<ErrorOr<Success>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
