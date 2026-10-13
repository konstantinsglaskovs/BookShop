using BookShop.Application.Сommon.Interfaces;
using BookShop.Domain.Entities;
using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler: IRequestHandler<CreateReviewCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _db;
    private readonly IIdentityService _identityService;

    public CreateReviewCommandHandler(IAppDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<ErrorOr<Success>> Handle(CreateReviewCommand request,CancellationToken cancellationToken)
    {
        if (request.Rating < 1 || request.Rating > 5)
        {
            return Error.Validation(code: "Review.InvalidRating",description: "Rating must be between 1 and 5.");
        }

        var profileResult =await _identityService.GetCurrentUserProfileIdAsync();

        if (profileResult.IsError)
        {
            return profileResult.Errors;
        }

        var userProfileId = profileResult.Value;

        var bookExists = await _db.Books.AsNoTracking()
            .AnyAsync(book =>book.Id == request.BookId &&book.Status == BookStatus.Active);

        if (!bookExists)
        {
            return Error.NotFound(code: "Review.BookNotFound", description: "Book was not found.");
        }

        var reviewExists = await _db.Reviews
            .AsNoTracking()
            .AnyAsync(review =>review.BookId == request.BookId &&review.UserProfileId == userProfileId);

        if (reviewExists)
        {
            return Error.Validation(code: "Review.AlreadyExists",description: "You have already reviewed this book.");
        }

        _db.Reviews.Add(new Review
            {
                BookId = request.BookId,
                UserProfileId = userProfileId,
                Rating = request.Rating,
                Comment = request.Comment
            });

        await _db.SaveChangesAsync();

        return Result.Success;
    }
}
