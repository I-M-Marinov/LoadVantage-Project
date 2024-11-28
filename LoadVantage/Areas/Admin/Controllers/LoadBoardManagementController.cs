using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Extensions;
using LoadVantage.Filters;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc;

using static LoadVantage.Common.GeneralConstants.ActiveTabs;

namespace LoadVantage.Areas.Admin.Controllers
{
	[AdminOnly]
	[Area("Admin")]
	public class LoadBoardManagementController : Controller
	{

		private readonly IUserService userService;
		private readonly IProfileService profileService;
		private readonly IAdminProfileService adminProfileService;
		private readonly IAdminUserService adminUserService;
		private readonly IAdminLoadBoardService adminLoadBoardService;

		public LoadBoardManagementController(IUserService _userService,
			IProfileService _profileService,
			IAdminProfileService _adminProfileService,
			IAdminUserService _adminUserService,
			IAdminLoadBoardService _adminLoadBoardService)
		{
			userService = _userService;
			profileService = _profileService;
			adminProfileService = _adminProfileService;
			adminUserService = _adminUserService;
			adminLoadBoardService = _adminLoadBoardService;

		}

		[HttpGet]
		public async Task<IActionResult> LoadBoardManagement()
		{
			Administrator user = adminUserService.GetCurrentAdministratorAsync().Result;

			var loadBoard = await adminLoadBoardService.GetLoadBoardManager(user.Id);

			return View("~/Areas/Admin/Views/Admin/LoadBoardManagement/LoadBoardManagement.cshtml",loadBoard);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ReturnToLoadBoard()
		{
			return RedirectToAction(nameof(LoadBoardManagement));
		}
	}
}
