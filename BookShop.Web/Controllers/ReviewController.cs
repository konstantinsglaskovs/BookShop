using BookShop.Application.Reviews.Commands.CreateReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IMediator _mediator;

        public ReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid bookId,int rating,string? comment)
        {
            var result = await _mediator.Send(new CreateReviewCommand
                {
                    BookId = bookId,
                    Rating = rating,
                    Comment = comment
                });

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;
            }
            else
            {
                TempData["Success"] ="Review added successfully.";
            }

            return RedirectToAction("Details","Book",new { id = bookId });
        }
    }
}
