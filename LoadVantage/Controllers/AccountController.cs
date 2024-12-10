using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using LoadVantage.Extensions;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Account;
using LoadVantage.Infrastructure.Data.Models;

using static LoadVantage.Common.GeneralConstants.TempMessages;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;


namespace LoadVantage.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IAccountService accountService;

        public AccountController(IUserService _userService, IAccountService _accountService)
        {
	        userService = _userService;
            accountService = _accountService;
		}

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
	        if (!ModelState.IsValid)
	        {
		        return View(model);
	        }

	        var (result, errorKey) = await accountService.RegisterUserAsync(model);

	        if (result.Succeeded)
	        {
		        TempData.SetSuccessMessage(LoginWithNewAccount);
		        return RedirectToAction(nameof(Login));
	        }

	        if (errorKey != null)
	        {
		        ModelState.AddModelError(errorKey, result.Errors.FirstOrDefault()?.Description ?? "An error occurred.");
	        }
	        else
	        {
		        foreach (var error in result.Errors)
		        {
			        ModelState.AddModelError(string.Empty, error.Description);
		        }
	        }

	        return View(model);
        }

		[HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var (result, message) = await accountService.LoginAsync(model.UserName, model.Password);

			if (result == SignInResult.Success)
			{
				return await RedirectToProfile();
			}

			if (!string.IsNullOrEmpty(message))
			{
				ModelState.AddModelError(string.Empty, message);
			}

			return View(model);

        }

        [Authorize]
        [HttpGet]
		public async Task<IActionResult> Logout()
        {
	        await accountService.LogOutAsync();

	        TempData.SetSuccessMessage(LoggedOutOfAccount);
	        return RedirectToAction("Login", "Account");
        }

        private async Task<IActionResult> RedirectToProfile()
        {
			var user = await userService.GetCurrentUserAsync(); 

			if (user is Administrator)
	        {
		        return RedirectToAction("AdminProfile", "Admin", new { area = "Admin" }); // Redirect to Admin Profile in the Admin area
			}

	        if (user is Dispatcher || user is Broker)
	        {
		        return RedirectToAction("Profile", "Profile"); // Redirect to Profile page
			}

			return RedirectToAction("Index", "Home"); // Redirect to the Home page alternatively
        }

	}
}
