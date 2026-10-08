using BookShop.Application.Сommon.Extensions;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _db;

    public UpdateCategoryCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(category => category.Id == request.Id);

        if (category is null)
        {
            return Error.NotFound(code: "Category.NotFound", description: "Category was not found.");
        }

        var normalizedName = request.Name.ToNormalizedTitleCase();

        var categoryExists = await _db.Categories.AnyAsync(otherCategory =>
                    otherCategory.Id != request.Id && otherCategory.Name == normalizedName);

        if (categoryExists)
        {
            return Error.Conflict(code: "Category.AlreadyExists", description: "Category with this name already exists.");
        }

        category.Name = normalizedName;

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
