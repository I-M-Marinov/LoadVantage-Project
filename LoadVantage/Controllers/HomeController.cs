using LoadVantage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LoadVantage.Core.Contracts;
using LoadVantage.Extensions;
using Microsoft.AspNetCore.Authorization;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Controllers
{
	public class HomeController : Controller
	{
		private readonly IUserService userService;

		public HomeController(IUserService _userService)
		{
			userService = _userService;
		}

		[AllowAnonymous]
        public async Task<IActionResult> Index()
		{
			if (!User.Identity!.IsAuthenticated)
			{
				return View(); 
			}

			var username = User.FindFirst("UserName")?.Value;

			if (string.IsNullOrEmpty(username))
			{
				TempData.SetErrorMessage(InvalidSession);
				return RedirectToAction("Login", "Account"); 
			}

            var user = await userService.FindUserByUsernameAsync(username);

            if (user == null)
            {
				TempData.SetErrorMessage(UserAccountNotFound);
	            return RedirectToAction("Login", "Account"); 
            }

            if (user is Administrator)
            {
	            return RedirectToAction("AdminProfile", "Admin", new { area = "Admin" }); // Redirect to Admin Profile in the Admin area

			}
			else if (user is Dispatcher || user is Broker)
            {
	            return RedirectToAction("Profile", "Profile");
            }

            TempData.SetErrorMessage(NoPermissionToView);
            return RedirectToAction("Register", "Account");
		}

        [AllowAnonymous]

        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
