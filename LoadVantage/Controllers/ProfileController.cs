using LoadVantage.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
		private readonly IUserService userService;
		private readonly IProfileService profileService;
		private readonly ILoadBoardService loadBoardService;
		private readonly ITruckService truckService;
		private readonly IDriverService driverService;

		public ProfileController(
			IUserService _userService,
			IProfileService _profileService, 
			ILoadBoardService _loadBoardService,
			ITruckService _truckService,
			IDriverService _driverService)
		{
			userService = _userService;
			profileService = _profileService;
			loadBoardService = _loadBoardService;
			truckService = _truckService;
			driverService = _driverService;
		}


		[HttpGet]
        public async Task<IActionResult> Profile()
        {
	        var user = await userService.GetCurrentUserAsync();

			await GetLoadCounts(user);

			ProfileViewModel? userProfileViewModel = await profileService.GetUserInformation(user.Id);

			try
			{
				if (user.Position is nameof(Dispatcher))
				{
					await GetTrucksAndDriversCounts(user);
				}
			}
			catch (ArgumentException e)
			{
				TempData.SetErrorMessage(e.Message);
				return View(userProfileViewModel);
			}

            return View(userProfileViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
	        var user = await userService.GetCurrentUserAsync();
	        await GetLoadCounts(user);

			ProfileViewModel? userProfileViewModel = await profileService.GetUserInformation(user.Id);
            model.UserImageUrl = userProfileViewModel!.UserImageUrl;


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var updatedModel = await profileService.UpdateProfileInformation(model, user.Id);

                if (updatedModel.Id != user.Id.ToString()) 
                {
                    TempData.SetActiveTab(ProfileActiveTab);
                    ModelState.AddModelError(string.Empty, NoChangesMadeToProfile); // tell the user no changes were made 

                    return View(updatedModel);
                }

                TempData.SetActiveTab(ProfileActiveTab);
                TempData.SetSuccessMessage(ProfileUpdatedSuccessfully);
                return View(updatedModel);
            }
			catch (Exception ex) when (ex is InvalidDataException || ex is InvalidOperationException || ex is ArgumentException)
			{
				TempData.SetActiveTab(ProfileEditActiveTab);

				if (ex is InvalidDataException) // Invalid username
				{
					ModelState.AddModelError("username", ex.Message); 
				}
				else if (ex is InvalidOperationException) // Invalid email
				{
					ModelState.AddModelError("email", ex.Message); 
				}
				else if (ex is ArgumentException) // Invalid user type 
				{
					TempData.SetErrorMessage(ex.Message); // message bubbling up from the GetLoadCountsForUserAsync method in the loadBoardService
					return View(model);
				}

				return View(model);
			}
			catch (Exception ex)
            {
                TempData.SetActiveTab(ProfileActiveTab);
                TempData.SetSuccessMessage(NoChangesMadeToProfile); // tell the user no changes were made 
				return View(model);
            }
           
		}

        [HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			var user = await userService.GetCurrentUserAsync();

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

			await GetLoadCounts(user);
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
				var profile = await profileService.GetUserInformation(userId);

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
				TempData.SetActiveTab(ProfileChangePictureActiveTab);
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
				TempData.SetActiveTab(ProfileChangePictureActiveTab);
                return RedirectToAction("Profile", model);
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
		public IActionResult RefreshEditProfile()
		{
			TempData.SetActiveTab(ProfileEditActiveTab);
			return RedirectToAction(nameof(Profile));
		}

		private async Task GetLoadCounts(BaseUser user)
		{
			var loadCounts = new Dictionary<string, Dictionary<LoadStatus, int>>();

			try
			{
				loadCounts = await loadBoardService.GetLoadCountsForUserAsync(user.Id, user.Position);
			}
			catch (ArgumentException e)
			{
				throw new ArgumentException(e.Message);
			}

			if (user is Broker)
			{
				var brokerLoadCounts = loadCounts[nameof(Broker)];

				ViewBag.CreatedLoadsCount = brokerLoadCounts.GetValueOrDefault(LoadStatus.Created, 0);
				ViewBag.PostedLoadsCount = brokerLoadCounts.GetValueOrDefault(LoadStatus.Available, 0);
				ViewBag.BookedLoadsCount = brokerLoadCounts.GetValueOrDefault(LoadStatus.Booked, 0);
				ViewBag.DeliveredLoadsCount = brokerLoadCounts.GetValueOrDefault(LoadStatus.Delivered, 0);
			}
			else // user is Dispatcher
			{
				var dispatcherLoadCounts = loadCounts[nameof(Dispatcher)];

				ViewBag.BookedLoadsCount = dispatcherLoadCounts.GetValueOrDefault(LoadStatus.Booked, 0);
				ViewBag.DeliveredLoadsCount = dispatcherLoadCounts.GetValueOrDefault(LoadStatus.Delivered, 0);
			}
		}

        private async Task GetTrucksAndDriversCounts(BaseUser user)
        {
            try
            {
				var truckCount = await truckService.GetTruckCount(user.Id);
				var driversCount = await driverService.GetDriverCount(user.Id);

				ViewBag.TrucksCount = truckCount > 0 ? truckCount : 0;
				ViewBag.DriversCount = driversCount > 0 ? driversCount : 0;

			}
            catch (Exception e)
            {
	            ViewBag.TrucksCount = 0;
	            ViewBag.DriversCount = 0;

				throw new ArgumentException(ErrorRetrievingTruckAndDriverCount);
            }

        }
    }
}
