using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using LoadVantage.Areas.Broker.Contracts;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Filters;
using LoadVantage.Common.Enums;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Extensions;
using LoadVantage.Infrastructure.Data.Models;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using static LoadVantage.Common.GeneralConstants.SuccessMessages;
using static LoadVantage.Common.GeneralConstants.ActiveTabs;

namespace LoadVantage.Areas.Broker.Controllers
{
    [BrokerOnly]
    [Area("Broker")]
    [Route("Broker")]
    public class BrokerController(
        UserManager<User> userManager, 
        IBrokerLoadBoardService brokerLoadBoardService, 
        ILoadStatusService loadService, 
        ILogger<BrokerController> logger, 
        IProfileService profileService) 
        : Controller
    {
        [HttpGet]
        [Route("Profile")]
        public async Task<IActionResult> Profile()
        {
			var userGuidId = User.GetUserId();

            if (userGuidId == null)
            {
                return NotFound(UserNotFound); // User was not found
            }

            try
            {
                ProfileViewModel? broker = await profileService.GetUserInformation(userGuidId.Value);

                if (broker == null)
                {
                    return NotFound(BrokerInformationNotRetrieved);  // Broker was not found
                }
                
                return View(broker);
            }
            catch (Exception)
            {
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Profile")]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
	            TempData.SetActiveTab(ProfileEditActiveTab);
                return View("Profile", model);
            }

            var userGuidId = User.GetUserId();

            if (userGuidId == Guid.Empty)
            {
                return NotFound(UserNotFound); // User was not found
            }

            var isUsernameTaken = await profileService.IsUsernameTakenAsync(model.Username, userGuidId.Value);
            if (isUsernameTaken)
            {
                TempData.SetActiveTab(ProfileEditActiveTab);
                ModelState.AddModelError("Username", UserNameIsAlreadyTaken);
                return View(model);
            }

            try
            {
                var updatedModel = await profileService.UpdateProfileInformation(model, userGuidId.Value);

                TempData.SetSuccessMessage(ProfileUpdatedSuccessfully);
                TempData.SetActiveTab(ProfileEditActiveTab);
                return View(updatedModel);
            }
            catch (Exception ex)
            {
                TempData.SetActiveTab(ProfileActiveTab);
                TempData.SetSuccessMessage(ex.Message);
                return View("Profile", model);
            }
        }

        [HttpGet]
        [Route("LoadBoard")]
        public async Task<IActionResult> LoadBoard()
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var loadBoardInfo = await brokerLoadBoardService.GetBrokerLoadBoardAsync(userId);

            return View(loadBoardInfo);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("BackToLoadBoard")]
		public IActionResult ReturnToLoadBoard()
        {
            return RedirectToAction("LoadBoard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("BackToEditProfile")]
        public IActionResult GoToEditProfileTab()
        {
            TempData.SetActiveTab(ProfileEditActiveTab);
            return RedirectToAction("Profile");
        }

        [HttpGet]
        [Route("Load")]
        public async Task<IActionResult> LoadDetails(Guid loadId)
        {
            try
            {
                var loadToShow = await loadService.GetLoadDetailsAsync(loadId);

                if (loadToShow == null)
                {
                    TempData.SetErrorMessage(LoadInformationCouldNotBeRetrieved);
                    return RedirectToAction("LoadDetails", new { loadId });
                }

                return View(loadToShow);
            }
            catch (Exception e)
            {
                TempData.SetErrorMessage(ErrorRetrievingDetailsForLoad + " " + e.Message);
				return RedirectToAction("LoadDetails", new { loadId });
            }
        }

        [HttpGet]
        [Route("Create")]

        public IActionResult CreateLoad(Guid brokerId)
        {
            var model = new LoadViewModel
            {
                BrokerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]

        public async Task<IActionResult> CreateLoad(LoadViewModel model, Guid brokerId)
        {
            model.Status = LoadStatus.Created.ToString();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var loadId = await loadService.CreateLoadAsync(model, brokerId);

                TempData.SetSuccessMessage(LoadCreatedSuccessfully);

				return RedirectToAction("LoadDetails", new { loadId });
			}
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ErrorCreatingLoad + ex.Message);
                return View("LoadDetails", model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")] 

        public async Task<IActionResult> EditLoad(LoadViewModel model, bool isEditing, Guid loadId)
        {

            if (isEditing)
            {
                if (!ModelState.IsValid)
                {
					return View("LoadDetails", model);
                }

                try
                {
                    await loadService.EditLoadAsync(loadId, model);

                    TempData["isEditing"] = false;
                    TempData.SetSuccessMessage(LoadUpdatedSuccessfully);

                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorUpdatingLoad + e.Message);
                    TempData.SetErrorMessage(ErrorUpdatingLoad + e.Message);
                }

                return RedirectToAction("LoadDetails", new { loadId = model.Id });
            }

            return RedirectToAction("LoadDetails", new { loadId = model.Id });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Cancel")] 

        public async Task<IActionResult> CancelLoad(Guid loadId)
        {
            TempData["isEditing"] = false;
            
            if (loadId == Guid.Empty)
            {
                TempData.SetErrorMessage(LoadCouldNotBeRetrieved + " " + LoadIdInvalid);
				return View("LoadDetails");
            }

            LoadViewModel load = await loadService.GetLoadByIdAsync(loadId);

            try
            {
                await loadService.CancelLoadAsync(loadId);

                TempData.SetSuccessMessage(LoadCancelledSuccessfully);
				return RedirectToAction("LoadBoard");
            }
            catch (Exception e)
            {
                TempData.SetErrorMessage(ErrorCancellingLoad + e.Message);
				return View("LoadDetails", load);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Post")]
        public async Task<IActionResult> PostALoad(Guid loadId)
        {
            
            if (!ModelState.IsValid)
            {
                return RedirectToAction("LoadDetails", new { loadId });
            }

            if (loadId == Guid.Empty)
            {
                TempData.SetErrorMessage(LoadIdInvalid);
				return View("LoadBoard");
            }

            LoadViewModel load = await loadService.GetLoadByIdAsync(loadId);

            if (load.Status != LoadStatus.Created.ToString())
            {
                if (load.Status == LoadStatus.Available.ToString())
                {
                    TempData.SetErrorMessage(LoadIsAlreadyPosted);
                }
                else
                {
                    TempData.SetErrorMessage(LoadIsNotInCorrectStatus);
				}

				return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }

            try
            {
                await loadService.PostLoadAsync(load.Id);

                TempData.SetActiveTab(PostedActiveTab); // navigate to the posted tab
				TempData.SetSuccessMessage(LoadPostedSuccessfully);

				return RedirectToAction("LoadBoard");
			}
            catch (Exception e)
            {
                TempData.SetErrorMessage(ErrorPostingLoad + e.Message);
				return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Unpost")]
        public async Task<IActionResult> UnpostALoad(Guid loadId)
        {

            if (!ModelState.IsValid)
            {
                return RedirectToAction("LoadDetails", new { loadId });
            }

            if (loadId == Guid.Empty)
            {
                TempData.SetErrorMessage(LoadIdInvalid);
				return View("LoadBoard");
            }

            LoadViewModel load = await loadService.GetLoadByIdAsync(loadId);

            if (load.Status != LoadStatus.Available.ToString())
            {
                if (load.Status == LoadStatus.Created.ToString())
                {
					TempData.SetErrorMessage(LoadIsNotPosted); // if the load is already not posted ( created status ) 
				}
				else
                {
                    TempData.SetErrorMessage(LoadIsNotInCorrectStatus); // if the load is booked or billed 
				}

                return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }

            try
            {
                await loadService.UnpostLoadAsync(load.Id); // unpost the load

                TempData.SetActiveTab(CreatedActiveTab); // navigate to the created tab
                TempData.SetSuccessMessage(LoadUnpostedSuccessfully);

                return RedirectToAction("LoadBoard");
            }
            catch (Exception e)
            {
                TempData.SetErrorMessage(ErrorUnpostingLoad + e.Message);
				return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }
        }

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
                return View("Profile", profileModel); // Redirect to the profile page and pass the Model for the password form  
            }

			if (user == null)
	        {
		        return NotFound(UserNotFound);
	        }

	        var result = await profileService.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

	        if (result.Succeeded)
	        {
		       TempData.SetSuccessMessage(PasswordUpdatedSuccessfully);
               TempData.SetActiveTab(ProfileActiveTab); // navigate to the profile tab
                return View("Profile", profileModel); // Redirect to the profile page and pass the Model for the password form  
			}

            var errors = "";

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                errors += error.Description;
            }

            TempData.SetErrorMessage(errors);
            TempData.SetActiveTab(ProfileChangePasswordActiveTab); // navigate to the change password tab
            return View("Profile", profileModel); // Redirect to the profile page and pass the Model for the password form  

        }
    }
}
