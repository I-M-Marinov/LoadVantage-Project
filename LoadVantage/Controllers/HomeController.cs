using LoadVantage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> logger;
		private readonly UserManager<User> userManager;

		public HomeController(ILogger<HomeController> _logger, UserManager<User> _userManager)
		{
			logger = _logger;
			userManager = _userManager;
		}

		[AllowAnonymous]
        public async Task<IActionResult> Index()
		{
            var username = User.FindFirst("UserName")?.Value;

            if (username != null)
            {
                var user = await userManager.FindByNameAsync(username);

                if (User.Identity!.IsAuthenticated)
                {
					if (user is Administrator)
						return RedirectToAction("AdminDashboard", "Admin"); // Redirect to admin dashboard
					if (user is Dispatcher || user is Broker)
						return RedirectToAction("Profile", "Profile"); // Redirect to Profile page
				}
            }

			return View();
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
