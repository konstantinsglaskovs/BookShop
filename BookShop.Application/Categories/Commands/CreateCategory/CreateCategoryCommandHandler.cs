using BookShop.Application.Сommon.Interfaces;
using Microsoft.EntityFrameworkCore;
using BookShop.Domain.Entities;
using ErrorOr;
using MediatR;
using BookShop.Application.Сommon.Extensions;

namespace BookShop.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _dbContext;

    public CreateCategoryCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Error.Validation(code: "Category.NameRequired",description: "Category name is required.");
        }

        var normalizedName = request.Name.ToNormalizedTitleCase();

        var categoryExists = await _dbContext.Categories.AnyAsync(category => category.Name == normalizedName);

        if (categoryExists)
        {
            return Error.Conflict(code: "Category.AlreadyExists",description: "Category with this name already exists.");
        }

        var category = new Category
        {
            Name = normalizedName
        };

        _dbContext.Categories.Add(category);

        await _dbContext.SaveChangesAsync();

        return Result.Success;
    }
}
