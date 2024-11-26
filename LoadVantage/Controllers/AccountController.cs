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
using LoadVantage.Core.Contracts;

namespace LoadVantage.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<Role> roleManager;

        public AccountController(IUserService _userService, SignInManager<User> _signInManager, RoleManager<Role> _roleManager)
		{
            userService = _userService;
            signInManager = _signInManager;
            roleManager = _roleManager;
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

            if (!ValidPositions.Contains(model.Position))
            {
                ModelState.AddModelError("Position", InvalidPositionSelected);
                return View(model);
            }

            var existingEmail = await userService.FindUserByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", EmailAlreadyExists);
                return View(model);
            }

            var existingUser = await userService.FindUserByUsernameAsync(model.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", UserNameAlreadyExists);
                return View(model);
            }

            Role? role = await roleManager.FindByNameAsync(model.Role);

			var defaultImageId = await userService.GetOrCreateDefaultImageAsync();

			User user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                CompanyName = model.Company,
                Position = model.Position,
                Email = model.Email,
                UserName = model.UserName,
                Role = role!,
                UserImageId = defaultImageId
			};

            var result = await userService.CreateUserAsync(user, model.Password);

            if (result.Succeeded)
            {
                await userService.AssignUserRoleAsync(user, UserRoleName);
                await userService.AddUserClaimAsync(user, new Claim("Position", user.Position ?? ""));
                await userService.AddUserClaimAsync(user, new Claim("FirstName", user.FirstName ?? ""));
                await userService.AddUserClaimAsync(user, new Claim("LastName", user.LastName ?? ""));
                await userService.AddUserClaimAsync(user, new Claim("UserName", user.UserName ?? ""));

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userService.FindUserByUsernameAsync(model.UserName);

            if (user != null)
            {

                var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    var claims = GetMissingClaims(User.Claims, user.FirstName, user.LastName, user.UserName, user.Position);

                    if (claims.Count != 0)
                    {
                        await userService.AddUserClaimsAsync(user, claims);
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

        [Authorize]
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
