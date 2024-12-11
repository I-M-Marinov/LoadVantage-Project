using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.Load;
using LoadVantage.Extensions;
using LoadVantage.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LoadVantage.Common.GeneralConstants.SuccessMessages;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Areas.Admin.Controllers
{
	[Authorize]
	[AdminOnly]
	[Area("Admin")]
	public class AdminLoadController : Controller
	{
		private readonly IAdminProfileService adminProfileService;
		private readonly IAdminUserService adminUserService;
		private readonly IAdminLoadStatusService adminLoadStatusService;

		public AdminLoadController(
			IAdminProfileService _adminProfileService,
			IAdminUserService _adminUserService,
			IAdminLoadStatusService _adminLoadStatusService)
		{
			
			adminProfileService = _adminProfileService;
			adminUserService = _adminUserService;
			adminLoadStatusService = _adminLoadStatusService;
		}

		[HttpGet]
		[Route("LoadInfo")]

		public async Task<IActionResult> GetLoadInfo(Guid loadId)
		{

			var admin = await adminUserService.GetCurrentAdminAsync();

			if (admin == null)
			{
				return RedirectToAction("Login", "Account");
			}

			try
			{
				var loadToShow = await adminLoadStatusService.GetLoadInformation(loadId, admin.Id);

				if (loadToShow == null)
				{
					return NotFound(NoPermissionToSeeTheLoad);
				}

				return View("~/Areas/Admin/Views/Admin/Load/LoadInformation.cshtml", loadToShow);
			}
			catch (Exception e)
			{
				TempData.SetErrorMessage(e.Message);
				return RedirectToAction("GetLoadInfo", new { loadId }); 
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> UpdateLoad(AdminLoadViewModel model, bool isEditing, Guid loadId)
		{

			Guid userId = User.GetUserId()!.Value;
			var profile = await adminProfileService.GetAdminInformation(userId);

			model.AdminProfile = profile; // pass the user profile information in the view 

			if (isEditing)
			{
				
				if (!ModelState.IsValid)
				{
					return View("~/Areas/Admin/Views/Admin/Load/LoadInformation.cshtml", model);
				}

				try
				{
					var result = await adminLoadStatusService.EditLoadAsync(loadId, model);

					if (result)
					{
						TempData["isEditing"] = false;
						TempData.SetSuccessMessage(LoadUpdatedSuccessfully);
					}
					else
					{
						TempData["isEditing"] = false;
						TempData.SetErrorMessage(LoadWasNotUpdated);
					}

				}
				catch (Exception e)
				{
					ModelState.AddModelError(string.Empty, ErrorUpdatingLoad + e.Message);
					TempData.SetErrorMessage(ErrorUpdatingLoad + e.Message);
				}

				return RedirectToAction("GetLoadInfo", new { loadId = model.Id });
			}

			return RedirectToAction("GetLoadInfo", new { loadId = model.Id });

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RestoreLoad(Guid loadId)
		{
			var result = await adminLoadStatusService.RestoreLoadAsync(loadId);

			if (!result)
			{
				TempData.SetErrorMessage(LoadCouldNotBeRetrieved);
				return RedirectToAction("GetLoadInfo", new { loadId = loadId });
			}

			TempData.SetSuccessMessage(LoadWasRestoredSuccessfully);
			return RedirectToAction("GetLoadInfo", new { loadId = loadId });
		}


	}
}
