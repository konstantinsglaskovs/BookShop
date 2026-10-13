using BookShop.Application.Reviews.Dtos;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Reviews.Queries.GetBookReviews
{
    public class GetBookReviewsQuery: IRequest<ErrorOr<List<BookReviewDto>>>
    {
        public Guid BookId { get; set; }
    }
}
