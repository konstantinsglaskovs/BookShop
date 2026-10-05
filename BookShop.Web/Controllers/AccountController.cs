using BookShop.Application.Auth.Commands.Login;
using BookShop.Application.Auth.Commands.Logout;
using BookShop.Application.Auth.Commands.Register;
using BookShop.Web.Models.Account;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers;

public class AccountController : Controller
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var command = model.Adapt<RegisterCommand>();

        var result = await _mediator.Send(command);

        if (result.IsError)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var command = model.Adapt<LoginCommand>();

        var result = await _mediator.Send(command);

        if (result.IsError)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutCommand());

        return RedirectToAction("Index", "Home");
    }
}
