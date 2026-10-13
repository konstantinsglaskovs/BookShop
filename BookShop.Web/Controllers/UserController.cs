using BookShop.Application.UserManagement.Commands.ChangeUserRole;
using BookShop.Application.UserManagement.Commands.SetUserLock;
using BookShop.Application.UserManagement.Dtos;
using BookShop.Application.UserManagement.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var result = await _mediator.Send(new GetUsersQuery
            {
                Search = search
            });
            ViewBag.Search = search;
            if (result.IsError)
            {
                TempData["Error"] = result.FirstError.Description;

                return View(new List<UserListItemDto>());
            }

            

            return View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(Guid userId,string role)
        {
            var result = await _mediator.Send(new ChangeUserRoleCommand
            {
                UserId = userId,
                Role = role
            });

            if (result.IsError)
            {
                TempData["Error"] = result.FirstError.Description;
            }
            else
            {
                TempData["Success"] = "User role updated successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetLock(Guid userId,bool locked)
        {
            var result = await _mediator.Send(new SetUserLockCommand
            {
                UserId = userId,
                Locked = locked
            });

            if (result.IsError)
            {
                TempData["Error"] = result.FirstError.Description;
            }
            else
            {
                TempData["Success"] = locked
                    ? "User blocked successfully."
                    : "User unblocked successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
