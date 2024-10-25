using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

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
		public IActionResult Home()
		{
			return View();
		}
	}
}
