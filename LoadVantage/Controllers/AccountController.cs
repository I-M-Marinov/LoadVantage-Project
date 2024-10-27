using LoadVantage.Core.Models.Account;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static LoadVantage.Extensions.TempDataExtension;
using static LoadVantage.Common.GeneralConstants.TempMessages;

namespace LoadVantage.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<User> userManager;
		private readonly SignInManager<User> signInManager;
		private readonly RoleManager<Role> roleManager;
		private readonly LoadVantageDbContext context;

		public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
			RoleManager<Role> roleManager, LoadVantageDbContext context)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.roleManager = roleManager;
			this.context = context;
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
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
	            ModelState.AddModelError("Email", "An account with this email already exists.");
	            return View(model);
            }

			User user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                CompanyName = model.Company,
                Role = model.Position,
                Email = model.Email,
                UserName = model.UserName
            };

            var result = await userManager.CreateAsync(user, model.Password);


            if (result.Succeeded)
            {
	            await userManager.AddToRoleAsync(user, model.Position);
				TempData.SetMessage(LoginWithNewAccount);
				return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
	            ModelState.AddModelError(string.Empty, error.Description);
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
	        var user = await userManager.FindByNameAsync(model.UserName);
	        if (user != null)
	        {
		        var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

		        if (result.Succeeded)
		        {
			        return RedirectToAction("Index", "Home");
		        }
	        }

	        ModelState.AddModelError(string.Empty, "Invalid username or password");
	        return View(model);
        }
        public async Task<IActionResult> Logout()
        {
	        await signInManager.SignOutAsync();
	        TempData.SetMessage(LoggedOutOfAccount);
			return RedirectToAction(nameof(Login));
        }
	}
}
