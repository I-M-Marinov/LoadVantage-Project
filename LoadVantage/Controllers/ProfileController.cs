using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using LoadVantage.Extensions;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Core.Models.Image;
using LoadVantage.Infrastructure.Data.Models;

using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.GeneralConstants.SuccessMessages;
using static LoadVantage.Common.GeneralConstants.ActiveTabs;
using static LoadVantage.Common.GeneralConstants.UserRoles;




namespace LoadVantage.Controllers
{
	public class ProfileController(
		UserManager<User> userManager,
		IUserService userService,
		IProfileService profileService)
		: Controller
	{

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("ChangePassword")]

		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{

			User? user = await User.GetUserAsync(userManager);

			var profileModel = new ProfileViewModel { ChangePasswordViewModel = model };

			if (!ModelState.IsValid)
			{
				TempData.SetActiveTab(ProfileChangePasswordActiveTab); // navigate to the change password tab
				return RedirectToAction("Profile", "Broker", profileModel); // Redirect to the profile page and pass the Model for the password form  
			}

			var result = await profileService.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

			if (result.Succeeded)
			{
				TempData.SetSuccessMessage(PasswordUpdatedSuccessfully);
				TempData.SetActiveTab(ProfileActiveTab); // navigate to the profile tab

				if(user is Administrator)
					return RedirectToAction("AdminDashboard", "Admin", new { area = "Admin" });
				if (user is Dispatcher)
					return RedirectToAction("Profile", "Dispatcher", new { area = "Dispatcher" });
				if (user is Broker)
					return RedirectToAction("Profile", "Broker", new { area = "Broker" });
			}

			var errors = "";

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
				errors += error.Description;
			}

			TempData.SetErrorMessage(errors);
			TempData.SetActiveTab(ProfileChangePasswordActiveTab); // navigate to the change password tab
			return RedirectToAction("Profile", "Broker", profileModel); // Redirect to the profile page and pass the Model for the password form  

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("UpdateImage")]
		public async Task<IActionResult> UpdateProfileImage(ImageFileUploadModel imageModel)
		{
			Guid userId = User.GetUserId().Value;

			if (!ModelState.IsValid)
			{
				TempData.SetActiveTab(ProfileEditActiveTab);
				return RedirectToAction("Profile", "Broker", new { area = "Broker" });
			}

			try
			{
                await userService.UpdateUserImageAsync(userId, imageModel.FormFile);

                TempData.SetSuccessMessage(ImageUpdatedSuccessfully);
                TempData.SetActiveTab(ProfileEditActiveTab);

                return RedirectToAction("Profile", "Broker", new { area = "Broker" }); // Redirect to the profile page 
            }
			catch (Exception ex)
			{
				ModelState.AddModelError("Image", ErrorUpdatingImage + ex.Message);
				TempData.SetActiveTab(ProfileEditActiveTab);
				return RedirectToAction("Profile", "Broker", new { area = "Broker" }); // Redirect to the profile page 
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("DeleteImage")]

		public async Task<IActionResult> DeleteProfileImage()
		{
			User user = User.GetUserAsync(userManager).Result!;

			try
			{
				await userService.DeleteUserImageAsync(user.Id, user.UserImageId.Value);

				TempData.SetSuccessMessage(ImageRemoveSuccessfully);
				return RedirectToAction("Profile", "Broker", new { area = "Broker" });
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("Image", ErrorRemovingImage + ex.Message);
				TempData.SetErrorMessage(ErrorRemovingImage + ex.Message);
				return RedirectToAction("Profile", "Broker", new { area = "Broker" });
			}
		}

		private IActionResult RedirectToUserDashboard(User user)
		{
			if (user is Administrator)
				return RedirectToAction("AdminDashboard", "Admin", new { area = "Admin" });
			if (user is Dispatcher)
				return RedirectToAction("Profile", DispatcherPositionName, new { area = DispatcherPositionName });
			if (user is Broker)
				return RedirectToAction("Profile", BrokerPositionName, new { area = BrokerPositionName });

			// Fallback if user type is not identified
			return RedirectToAction("Index", "Home");
		}

		private IActionResult ViewUserProfile(User user, object viewModel)
		{
			if (user is Dispatcher)
				return RedirectToAction("Profile", DispatcherPositionName, viewModel);
			if (user is Broker)
				return RedirectToAction("Profile", BrokerPositionName, viewModel);

			// Fallback if user type is not identified
			return View("Error"); // Or another appropriate fallback view
		}


	}
}
