using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models;
using LoadVantage.Areas.Admin.Services;
using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Image;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Core.Services;
using LoadVantage.Extensions;
using LoadVantage.Filters;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.GeneralConstants.ActiveTabs;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.ValidationConstants.UserImageValidations;
using static LoadVantage.Common.ValidationConstants.UserValidations;

namespace LoadVantage.Areas.Admin.Controllers
{
	[AdminOnly]
	[Area("Admin")]

	public class AdminController : Controller
	{
		private readonly IUserService userService;
		private readonly IAdminProfileService adminProfileService;
		private readonly ILoadBoardService loadBoardService;

		public AdminController(IUserService _userService,
			IAdminProfileService _adminProfileService, ILoadBoardService _loadBoardService)
		{
			userService = _userService;
			adminProfileService = _adminProfileService;
			loadBoardService = _loadBoardService;
		}

		[HttpGet]
		[Route("AdminProfile")]
		public async Task<IActionResult> AdminProfile()
		{
			var user = await userService.GetCurrentUserAsync();

			AdminProfileViewModel? adminProfileInformation = await adminProfileService.GetAdminInformation(user.Id);

			return View("~/Areas/Admin/Views/Admin/Profile/AdminProfile.cshtml", adminProfileInformation);
		}


		//[HttpPost]
		//[ValidateAntiForgeryToken]

		//public async Task<IActionResult> ChangePassword(AdminChangePasswordViewModel model)
		//{
		//	var user = await userService.GetCurrentUserAsync();

		//	ProfileViewModel? profileModel = await profileService.GetUserInformation(user.Id);

		//	profileModel.ChangePasswordViewModel = model;

		//	if (!ModelState.IsValid)
		//	{
		//		TempData.SetActiveTab(ProfileChangePasswordActiveTab); // navigate to the change password tab
		//		return View("Profile", profileModel); // Redirect to the profile page and pass the Model for the password form  
		//	}

		//	var result = await profileService.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

		//	if (result.Succeeded)
		//	{
		//		TempData.SetSuccessMessage(PasswordUpdatedSuccessfully);
		//		TempData.SetActiveTab(ProfileActiveTab); // navigate to the profile tab

		//		return View("Profile", profileModel);
		//	}

		//	var errors = "";

		//	foreach (var error in result.Errors)
		//	{
		//		ModelState.AddModelError(string.Empty, error.Description);
		//		errors += error.Description;
		//	}

		//	await GetLoadCounts(user);
		//	TempData.SetErrorMessage(errors);
		//	TempData.SetActiveTab(ProfileChangePasswordActiveTab); // navigate to the change password tab
		//	return View("Profile", profileModel); // Redirect to the profile page and pass the Model for the password form  

		//}

		[HttpGet]
		public async Task<IActionResult> UpdateProfileImage()
		{
			Guid userId = User.GetAdminId()!.Value;

			try
			{
				var profile = await adminProfileService.GetAdminInformation(userId);

				profile.AdminImageFileUploadModel = new AdminImageFileUploadModel();

				return RedirectToAction("AdminProfile", profile);
			}
			catch (Exception ex)
			{
				TempData.SetErrorMessage(ex.Message);
				return View("~/Areas/Admin/Views/Admin/Profile/AdminProfile.cshtml");

			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateProfileImage(AdminProfileViewModel model)
		{
			Guid userId = User.GetAdminId()!.Value;

			ModelState.Clear(); // Clear any other model state errors found 
			TryValidateModel(model.AdminImageFileUploadModel); // Validate just the image uploaded 

			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.ToList();

				TempData.SetErrorMessage(string.Join(", ", errors));
				TempData.SetActiveTab(ProfileChangePictureActiveTab);
				return RedirectToAction("AdminProfile", model);
			}

			try
			{
				await userService.UpdateUserImageAsync(userId, model.AdminImageFileUploadModel.FormFile);

				TempData.SetSuccessMessage(ImageUpdatedSuccessfully);
				TempData.SetActiveTab(ProfileChangePictureActiveTab);

				return RedirectToAction("AdminProfile");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("Image", ErrorUpdatingImage + ex.Message);
				TempData.SetActiveTab(ProfileChangePictureActiveTab);
				return RedirectToAction("AdminProfile", model);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> DeleteProfileImage()
		{
			var user = await userService.GetCurrentUserAsync();

			try
			{
				await userService.DeleteUserImageAsync(user.Id, user.UserImageId!.Value);

				TempData.SetSuccessMessage(ImageRemoveSuccessfully);
				return RedirectToAction("AdminProfile");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("Image", ErrorRemovingImage + ex.Message);
				TempData.SetErrorMessage(ErrorRemovingImage + ex.Message);
				return View("~/Areas/Admin/Views/Admin/Profile/AdminProfile.cshtml");
			}
		}
	}

	//[HttpGet]
	//public IActionResult RefreshEditProfile()
	//{
	//	TempData.SetActiveTab(ProfileEditActiveTab);
	//	return RedirectToAction(nameof(Profile));
	//}

	//private async Task GetLoadCounts(User user)
	//{
	//	var loadCounts = await loadBoardService.GetLoadCountsForUserAsync(user.Id, user.Position);

	//	if (user is Broker)
	//	{
	//		var brokerLoadCounts = loadCounts[nameof(Broker)];

	//		ViewBag.CreatedLoadsCount = brokerLoadCounts.GetValueOrDefault(LoadStatus.Created, 0);
	//		ViewBag.PostedLoadsCount = brokerLoadCounts.GetValueOrDefault(LoadStatus.Available, 0);
	//		ViewBag.BookedLoadsCount = brokerLoadCounts.GetValueOrDefault(LoadStatus.Booked, 0);
	//		ViewBag.DeliveredLoadsCount = brokerLoadCounts.GetValueOrDefault(LoadStatus.Delivered, 0);
	//	}
	//	else // user is Dispatcher
	//	{
	//		var dispatcherLoadCounts = loadCounts[nameof(Dispatcher)];

	//		ViewBag.BookedLoadsCount = dispatcherLoadCounts.GetValueOrDefault(LoadStatus.Booked, 0);
	//		ViewBag.DeliveredLoadsCount = dispatcherLoadCounts.GetValueOrDefault(LoadStatus.Delivered, 0);
	//	}
	//}
}

