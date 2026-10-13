using ErrorOr;
using MediatR;

namespace BookShop.Application.Reviews.Commands.CreateReview;

public class CreateReviewCommand : IRequest<ErrorOr<Success>>
{
    public Guid BookId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
