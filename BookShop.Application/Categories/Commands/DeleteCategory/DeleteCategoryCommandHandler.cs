using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler:IRequestHandler<DeleteCategoryCommand,ErrorOr<Success>>
{
    private readonly IAppDbContext _db;

    public DeleteCategoryCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteCategoryCommand request,CancellationToken cancellationToken)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(category => category.Id == request.Id);
                

        if (category is null)
        {
            return Error.NotFound(code: "Category.NotFound",description: "Category was not found.");
        }

        var isUsedByBooks = await _db.Books.AnyAsync(book => book.CategoryId == request.Id);

        if (isUsedByBooks)
        {
            return Error.Conflict(code: "Category.InUse",description: "Category cannot be deleted because it is used by one or more books.");
        }

        _db.Categories.Remove(category);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
