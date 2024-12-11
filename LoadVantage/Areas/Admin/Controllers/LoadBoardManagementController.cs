using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Core.Contracts;
using LoadVantage.Extensions;
using LoadVantage.Filters;
using LoadVantage.Infrastructure.Data.Models;


namespace LoadVantage.Areas.Admin.Controllers
{
	[Authorize]
	[AdminOnly]
	[Area("Admin")]
	public class LoadBoardManagementController : Controller
	{

		private readonly IAdminUserService adminUserService;
		private readonly IAdminLoadBoardService adminLoadBoardService;

		public LoadBoardManagementController(
			IAdminUserService _adminUserService,
			IAdminLoadBoardService _adminLoadBoardService)
		{
		
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

		[HttpGet]
		public IActionResult GetPostedLoadsTableForAdmin()
		{
			var userId = User.GetAdminId().Value;

			var loads = adminLoadBoardService.GetAllPostedLoadsAsync(userId).Result;

			return PartialView("~/Areas/Admin/Views/Admin/LoadBoardManagement/_AdminPostedLoadsTablePartial.cshtml", loads);
		}
	}
}
