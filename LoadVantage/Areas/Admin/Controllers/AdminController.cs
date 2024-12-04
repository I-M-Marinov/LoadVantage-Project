using Microsoft.AspNetCore.Mvc;

using LoadVantage.Filters;
using LoadVantage.Extensions;
using LoadVantage.Core.Contracts;
using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.GeneralConstants.ActiveTabs;
using static LoadVantage.Common.GeneralConstants.SuccessMessages;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Areas.Admin.Controllers
{
	[Authorize]
    [AdminOnly]
	[Area("Admin")]

	public class AdminController : Controller
	{
		private readonly IUserService userService;
		private readonly IAdminProfileService adminProfileService;
		private readonly IAdminUserService adminUserService;
		private readonly IStatisticsService statisticsService;

		public AdminController(IUserService _userService,
			IAdminProfileService _adminProfileService,
			IAdminUserService _adminUserService, 
			IStatisticsService _statisticsService)
		{
			userService = _userService;
			adminProfileService = _adminProfileService;
			adminUserService = _adminUserService;
			statisticsService = _statisticsService;
		}

		[HttpGet]
		[Route("AdminProfile")]
		public async Task<IActionResult> AdminProfile()
		{
			var user = await userService.GetCurrentUserAsync();

			ViewBag.TotalRevenue = await statisticsService.GetTotalRevenuesAsync();
			ViewBag.UsersCount = await statisticsService.GetTotalUserCountAsync();
			ViewBag.LoadsCount = await statisticsService.GetTotalLoadCountAsync();

			AdminProfileViewModel ? adminProfileInformation = await adminProfileService.GetAdminInformation(user.Id);

			return View("~/Areas/Admin/Views/Admin/Profile/AdminProfile.cshtml", adminProfileInformation);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> UpdateProfile(AdminProfileViewModel model)
		{
			var user = await adminUserService.GetCurrentAdministratorAsync();

			AdminProfileViewModel? userProfileViewModel = await adminProfileService.GetAdminInformation(user.Id);
			model.UserImageUrl = userProfileViewModel!.UserImageUrl;


			if (!ModelState.IsValid)
			{
				return View("~/Areas/Admin/Views/Admin/Profile/AdminProfile.cshtml", model);
			}

			try
			{
				var updatedModel = await adminProfileService.UpdateProfileInformation(model, user.Id);

				if (updatedModel.Id != user.Id.ToString())
				{
					TempData.SetActiveTab(ProfileActiveTab);
					ModelState.AddModelError(string.Empty, NoChangesMadeToProfile);

					return View("~/Areas/Admin/Views/Admin/Profile/AdminProfile.cshtml", updatedModel);
				}

				TempData.SetActiveTab(ProfileActiveTab);
				TempData.SetSuccessMessage(ProfileUpdatedSuccessfully);
				return View("~/Areas/Admin/Views/Admin/Profile/AdminProfile.cshtml", updatedModel);
			}
			catch (Exception ex) when (ex is InvalidDataException || ex is InvalidOperationException)
			{
				TempData.SetActiveTab(ProfileEditActiveTab);

				if (ex is InvalidDataException)
				{
					ModelState.AddModelError("username", ex.Message); // Invalid username
				}
				else if (ex is InvalidOperationException)
				{
					ModelState.AddModelError("email", ex.Message); // Invalid email
				}

				return View("~/Areas/Admin/Views/Admin/Profile/AdminProfile.cshtml", model);
			}
			catch (Exception ex)
			{
				TempData.SetActiveTab(ProfileActiveTab);
				TempData.SetSuccessMessage(NoChangesMadeToProfile); // tell the user no changes were made 
				return View("~/Areas/Admin/Views/Admin/Profile/AdminProfile.cshtml", model);
			}

		}

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
			var user = await adminUserService.GetCurrentAdministratorAsync();

			try
			{
				await adminUserService.DeleteUserImageAsync(user.Id, user.UserImageId!.Value);

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

		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> ChangePassword(AdminChangePasswordViewModel model)
		{
			var user = await adminUserService.GetCurrentAdministratorAsync();

			AdminProfileViewModel? profileModel = await adminProfileService.GetAdminInformation(user.Id);

			profileModel.AdminChangePasswordViewModel = model;

			if (!ModelState.IsValid)
			{
				TempData.SetActiveTab(ProfileChangePasswordActiveTab); // navigate to the change password tab
				return View("~/Areas/Admin/Views/Admin/Profile/AdminProfile.cshtml", profileModel); // Redirect to the profile page and pass the Model for the password form  
			}

			var result = await adminProfileService.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

			if (result.Succeeded)
			{
				TempData.SetSuccessMessage(PasswordUpdatedSuccessfully);
				TempData.SetActiveTab(ProfileActiveTab); // navigate to the profile tab

				return View("~/Areas/Admin/Views/Admin/Profile/AdminProfile.cshtml", profileModel);
			}

			var errors = "";

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
				errors += error.Description;
			}

			TempData.SetErrorMessage(errors);
			TempData.SetActiveTab(ProfileChangePasswordActiveTab); // navigate to the change password tab
			return View("~/Areas/Admin/Views/Admin/Profile/AdminProfile.cshtml", profileModel); // Redirect to the profile page and pass the Model for the password form  

		}

		[HttpGet]
		public IActionResult RefreshEditProfile()
		{
			TempData.SetActiveTab(ProfileEditActiveTab);

			return RedirectToAction(nameof(AdminProfile));
		}
	}

	
}

