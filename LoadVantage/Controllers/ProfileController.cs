using System.Security.Claims;
using LoadVantage.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using LoadVantage.Extensions;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Core.Models.Image;
using LoadVantage.Infrastructure.Data.Models;


using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.GeneralConstants.SuccessMessages;
using static LoadVantage.Common.GeneralConstants.ActiveTabs;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;




namespace LoadVantage.Controllers
{
	[Authorize]
	public class ProfileController : Controller
	{
		private readonly UserManager<User> userManager;
		private readonly IUserService userService;
		private readonly IProfileService profileService;
		private readonly ILoadBoardService loadBoardService;

		public ProfileController(UserManager<User> _userManager, IUserService _userService,
			IProfileService _profileService, ILoadBoardService _loadBoardService)
		{
			userManager = _userManager;
			userService = _userService;
			profileService = _profileService;
			loadBoardService = _loadBoardService;
		}


		[HttpGet]
        public async Task<IActionResult> Profile()
        {
	        var user = User.GetUserAsync(userManager).Result;


	        if (user is Broker)
	        {
		        ViewBag.CreatedLoadsCount = loadBoardService.GetCreatedLoadsCountForBrokerAsync(user.Id).Result;
		        ViewBag.PostedLoadsCount = loadBoardService.GetPostedLoadsCountForBrokerAsync(user.Id).Result;
		        ViewBag.BookedLoadsCount = loadBoardService.GetBookedLoadsCountForBrokerAsync(user.Id).Result;
		        ViewBag.DeliveredLoadsCount = loadBoardService.GetBilledLoadsCountForBrokerAsync(user.Id).Result;
			}
	        else // user is Dispatcher
	        {
		        ViewBag.BookedLoadsCount = loadBoardService.GetBookedLoadsCountForDispatcherAsync(user.Id).Result;
		        ViewBag.DeliveredLoadsCount = loadBoardService.GetBilledLoadsCountForDispatcherAsync(user.Id).Result;
			}

			ProfileViewModel? userProfileViewModel = await profileService.GetUserInformation(user.Id); // Fetch user information for the logged-in user only
            return View(userProfileViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Profile(ProfileViewModel model)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);  // or any custom claim name you use for userId

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = Guid.Parse(userIdClaim.Value); 
            ProfileViewModel? user = await profileService.GetUserInformation(userId);
            model.UserImageUrl = user!.UserImageUrl;


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isUsernameTaken = await profileService.IsUsernameTakenAsync(model.Username, userId);
            if (isUsernameTaken)
            {
                TempData.SetActiveTab(ProfileEditActiveTab);
                ModelState.AddModelError("Username", UserNameIsAlreadyTaken);
                return View(model);
            }

            try
            {
                var updatedModel = await profileService.UpdateProfileInformation(model, userId);

                if (updatedModel.Id != userId.ToString()) // If the returned model from the method is with a different Id or Position return the View and add some messages 
                {
                    TempData.SetActiveTab(ProfileActiveTab);
                    ModelState.AddModelError(string.Empty, NoChangesMadeToProfile);

                    return View(updatedModel);
                }

                TempData.SetSuccessMessage(ProfileUpdatedSuccessfully);
                TempData.SetActiveTab(ProfileActiveTab);
                return View(updatedModel);
            }
            catch (InvalidOperationException ex)
            {
                TempData.SetActiveTab(ProfileActiveTab);
                TempData.SetSuccessMessage(ex.Message);
                return View(model);
            }
            catch (InvalidDataException ex)
            {
	            TempData.SetActiveTab(ProfileActiveTab);
	            TempData.SetErrorMessage(ex.Message);
	            return View(model);
            }
		}

        [HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{

			User? user = await User.GetUserAsync(userManager);
            ProfileViewModel? profileModel = await profileService.GetUserInformation(user.Id);

            profileModel.ChangePasswordViewModel = model;

			if (!ModelState.IsValid)
			{
				TempData.SetActiveTab(ProfileChangePasswordActiveTab); // navigate to the change password tab
				return View("Profile", profileModel); // Redirect to the profile page and pass the Model for the password form  
			}

			var result = await profileService.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

			if (result.Succeeded)
			{
				TempData.SetSuccessMessage(PasswordUpdatedSuccessfully);
				TempData.SetActiveTab(ProfileActiveTab); // navigate to the profile tab

                return View("Profile", profileModel);
            }

			var errors = "";

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
				errors += error.Description;
			}

			TempData.SetErrorMessage(errors);
			TempData.SetActiveTab(ProfileChangePasswordActiveTab); // navigate to the change password tab
			return View("Profile",  profileModel); // Redirect to the profile page and pass the Model for the password form  

		}

		[HttpGet]
        public async Task<IActionResult> UpdateProfileImage()
		{
			Guid userId = User.GetUserId()!.Value;

			try
			{
				var profile = await userService.GetUserInformation(userId);

				profile.ImageFileUploadModel = new ImageFileUploadModel();

                return RedirectToAction("Profile", profile);
			}
            catch (Exception ex)
            { 
				TempData.SetErrorMessage(ex.Message);
                return View("Profile");

            }
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateProfileImage(ProfileViewModel model)
		{
			Guid userId = User.GetUserId()!.Value;

			ModelState.Clear(); // Clear any other model state errors found 
			TryValidateModel(model.ImageFileUploadModel); // Validate just the image uploaded 

			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.ToList();

				TempData.SetErrorMessage(string.Join(", ", errors));
				TempData.SetActiveTab(ProfileEditActiveTab);
				return RedirectToAction("Profile", model);
			}

			try
			{
                await userService.UpdateUserImageAsync(userId, model.ImageFileUploadModel.FormFile);

				TempData.SetSuccessMessage(ImageUpdatedSuccessfully);
                TempData.SetActiveTab(ProfileChangePictureActiveTab);

                return RedirectToAction("Profile");
            }
            catch (Exception ex)
			{
				ModelState.AddModelError("Image", ErrorUpdatingImage + ex.Message);
				TempData.SetActiveTab(ProfileEditActiveTab);
                return RedirectToAction("Profile", model);
			}
        }

		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> DeleteProfileImage()
		{
			User user = User.GetUserAsync(userManager).Result!;

			try
			{
				await userService.DeleteUserImageAsync(user.Id, user.UserImageId!.Value);

				TempData.SetSuccessMessage(ImageRemoveSuccessfully);
                return RedirectToAction("Profile");
            }
			catch (Exception ex)
			{
				ModelState.AddModelError("Image", ErrorRemovingImage + ex.Message);
				TempData.SetErrorMessage(ErrorRemovingImage + ex.Message);
                return View("Profile");
            }
		}


		[HttpGet]
		public IActionResult RefreshEditProfile(ProfileViewModel model)
		{
			TempData.SetActiveTab(ProfileEditActiveTab);
			return View(nameof(Profile), model);
		}


	}
}
