using LoadVantage.Core.Models.Account;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static LoadVantage.Extensions.TempDataExtension;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.GeneralConstants.TempMessages;
using static LoadVantage.Common.ValidationConstants;
using System.Security.Claims;

namespace LoadVantage.Controllers
{
	public class AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager)
        : Controller
    {

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
            
            existingUser = await userManager.FindByEmailAsync(model.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", UserNameAlreadyExists);
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
                await userManager.AddClaimAsync(user, new Claim("Position", user.Position ?? ""));

                TempData.SetSuccessMessage(LoginWithNewAccount);
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.UserName);

	        if (user != null)
            {

                var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
	               var claims =  GetMissingClaims(User.Claims, user.FirstName, user.LastName, user.UserName, user.Position);

	               if (claims.Count != 0)
	               {
		               await userManager.AddClaimsAsync(user, claims);
	               }

	               await signInManager.SignInAsync(user, isPersistent: false);

                    if (user is Administrator)
                        return RedirectToAction("AdminDashboard", "Admin"); // Redirect to admin dashboard
                    if (user is Dispatcher || user is Broker)
                        return RedirectToAction("Profile", "Profile"); // Redirect to Profile page
                }

                ModelState.AddModelError(string.Empty, InvalidUserNameOrPassword);
                return View(model);

            }

            ModelState.AddModelError(string.Empty, InvalidUserNameOrPassword);
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
	        await signInManager.SignOutAsync();
	        TempData.SetSuccessMessage(LoggedOutOfAccount);
	        return RedirectToAction("Login", "Account", new { area = "" });
        }

        private List<Claim> GetMissingClaims(IEnumerable<Claim> existingClaims, string firstName, string lastName, string userName, string userPosition)
        {
			var claims = new List<Claim>
			{
				new Claim("FirstName", firstName),
				new Claim("LastName", lastName),
				new Claim("UserName", userName),
				new Claim("Position", userPosition),
			};

			var missingClaims = claims.Where(claim =>
				!existingClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value)
			).ToList();

			return missingClaims; 
		}

	}
}
