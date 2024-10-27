using LoadVantage.Core.Models.Account;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static LoadVantage.Extensions.TempDataExtension;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.GeneralConstants.TempMessages;
using static LoadVantage.Common.ValidationConstants;

namespace LoadVantage.Controllers
{
	public class AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager,
        LoadVantageDbContext context)
        : Controller
    {
        private readonly LoadVantageDbContext context = context;

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

            if (!ValidPositions.Contains(model.Position))
            {
	            ModelState.AddModelError("Position", InvalidPositionSelected);
	            return View(model);
            }

			var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
	            ModelState.AddModelError("Email", EmailAlreadyExists);
	            return View(model);
            }

            Role? role = await roleManager.FindByNameAsync(model.Role);

            User user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                CompanyName = model.Company,
                Position = model.Position,
                Email = model.Email,
                UserName = model.UserName,
                Role = role!
            };
            

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
	            await userManager.AddToRoleAsync(user, UserRoleName);
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
                    if (await userManager.IsInRoleAsync(user, AdminRoleName))
                    {
                        return RedirectToAction("AdminDashboard", "Admin", new { area = "Admin" }); // Redirect to admin dashboard
                    }
                    //else if (user is Dispatcher)
                    //{
                    //    return RedirectToAction("DispatcherDashboard", "Dispatcher"); // Redirect to Dispatcher dashboard
                    //}
                    //else if (user is Broker)
                    //{
                    //    return RedirectToAction("BrokerDashboard", "Broker"); // Redirect to Broker dashboard
                    //}
                    //else
                    //{
                    //    // Redirect to a general view if the role is unclear
                    //    return RedirectToAction("Index", "Home");
                    //}
                }
	        }

	        ModelState.AddModelError(string.Empty, InvalidUserNameOrPassword);
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
