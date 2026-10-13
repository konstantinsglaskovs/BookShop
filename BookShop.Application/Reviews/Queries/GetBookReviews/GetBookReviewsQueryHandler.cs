using BookShop.Application.Reviews.Dtos;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Application.Reviews.Queries.GetBookReviews;

public class GetBookReviewsQueryHandler : IRequestHandler<GetBookReviewsQuery, ErrorOr<List<BookReviewDto>>>
{
    private readonly IAppDbContext _db;

    public GetBookReviewsQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<List<BookReviewDto>>> Handle(
        GetBookReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await _db.Reviews.AsNoTracking()
            .Where(review =>review.BookId == request.BookId)
            .OrderByDescending(review =>review.CreatedAt)
            .Select(review => new BookReviewDto
            {
                Id = review.Id,

                AuthorName =review.UserProfile.Name +" " + review.UserProfile.Surname[0],

                Rating = review.Rating,

                Comment = review.Comment,

                CreatedAt = review.CreatedAt
            })
            .ToListAsync();

        return reviews;
    }
}
