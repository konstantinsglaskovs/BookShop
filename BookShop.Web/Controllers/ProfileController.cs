using BookShop.Application.Profiles.Commands.RemoveProfileImage;
using BookShop.Application.Profiles.Commands.UpdateProfile;
using BookShop.Application.Profiles.Commands.UpdateProfileImage;
using BookShop.Application.Profiles.Queries.GetProfile;
using BookShop.Web.Models.Profiles;
using BookShop.Web.Validators.Images;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ImageFileValidator _imageFileValidator;

        public ProfileController(IMediator mediator,ImageFileValidator imageFileValidator)
        {
            _mediator = mediator;
            _imageFileValidator = imageFileValidator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetProfileQuery());

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;

                return RedirectToAction("Index","Book");
            }

            return View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string name,string surname)
        {
            var result = await _mediator.Send(new UpdateProfileCommand
                {
                    Name = name,
                    Surname = surname
                });

            if (result.IsError)
            {
                TempData["Error"] =result.FirstError.Description;
            }
            else
            {
                TempData["Success"] ="Profile updated successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateImage(
    IFormFile image)
        {
            var validationResult = _imageFileValidator.Validate(image);

            if (validationResult.IsError)
            {
                return BadRequest(new
                {
                    error = validationResult.FirstError.Description
                });
            }

            await using var stream = image.OpenReadStream();

            var result = await _mediator.Send(new UpdateProfileImageCommand
                {
                    Stream = stream,
                    ContentType = validationResult.Value.ContentType,
                    FileExtension = validationResult.Value.Extension
                });

            if (result.IsError)
            {
                return BadRequest(new
                {
                    error = result.FirstError.Description
                });
            }

            return Ok(new
            {
                imageUrl = result.Value
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveImage()
        {
            var result = await _mediator.Send(new RemoveProfileImageCommand());

            if (result.IsError)
            {
                return BadRequest(new
                {
                    error = result.FirstError.Description
                });
            }

            return Ok();
        }
    }
}
