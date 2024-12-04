using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using LoadVantage.Core.Hubs;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Extensions;
using LoadVantage.Filters;
using LoadVantage.Common.Enums;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using static LoadVantage.Common.GeneralConstants.SuccessMessages;
using static LoadVantage.Common.GeneralConstants.ActiveTabs;


namespace LoadVantage.Controllers
{
    [Authorize]
    public class LoadController : Controller
    {
	    private readonly IProfileService profileService;
		private readonly IUserService userService;
		private readonly ILoadStatusService loadService;
        private readonly IHubContext<LoadHub> loadHub;

        public LoadController(IProfileService _profileService, ILoadStatusService _loadService, IUserService _userService, IHubContext<LoadHub> _loadHub)
		{
			profileService = _profileService;
			userService = _userService;
			loadService = _loadService;
			loadHub = _loadHub;
		}

        [HttpGet]

        public async Task<IActionResult> LoadDetails(Guid loadId)
        {
	        BaseUser? user = await userService.GetCurrentUserAsync();

			if (user == null)
            {
	            return RedirectToAction("Login", "Account");
			}

            try
            {
                var loadToShow = await loadService.GetLoadDetailsAsync(loadId,user.Id);

                if (loadToShow == null) // if for any of the reasons when a user is not allowed to see a load it returns null 
                {
	                TempData.SetErrorMessage(NoPermissionToSeeTheLoad);
	                return RedirectToAction("LoadBoard","LoadBoard");
				}

                return View(loadToShow);
            }
            catch (Exception e)
            {
                TempData.SetErrorMessage(e.Message);
                return RedirectToAction("LoadDetails", new { loadId });
            }
        }

        [HttpGet]
        [BrokerOnly]
        public async Task<IActionResult> CreateLoad()
        {
	        Guid userId = User.GetUserId()!.Value;
	        var profile = await profileService.GetUserInformation(userId);


			var model = new LoadViewModel
            {
                BrokerId = userId,
                UserProfile = profile
			};
             
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [BrokerOnly]

        public async Task<IActionResult> CreateLoad(LoadViewModel model)
        {
            model.Status = LoadStatus.Created.ToString();
            Guid userId = User.GetUserId().Value;
            var profile = await userService.GetUserInformation(userId);
            model.UserProfile = profile;

			if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (userId != model.BrokerId)
            {
                return RedirectToAction("LoadBoard", "LoadBoard"); // If the brokerId is not the same as logged user's id, redirect him back to the LoadBoard
            }

            try
            {
                var loadId = await loadService.CreateLoadAsync(model, model.BrokerId);

                TempData.SetSuccessMessage(LoadCreatedSuccessfully);

                return RedirectToAction("LoadDetails", new { loadId });
            }
            catch (Exception ex)
            {
               TempData.SetErrorMessage(ex.Message);
                return View("CreateLoad", model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [BrokerOnly]

        public async Task<IActionResult> EditLoad(LoadViewModel model, bool isEditing, Guid loadId)
        {

            Guid userId = User.GetUserId()!.Value;
            var profile = await userService.GetUserInformation(userId);
            model.UserProfile = profile; // pass the user profile information in the view 

			if (isEditing)
            {
                if (!ModelState.IsValid)
                {
                    return View("LoadDetails", model);
                }

                if (userId != model.BrokerId)
                {
                    return RedirectToAction("LoadBoard", "LoadBoard"); // If the brokerId is not the same as logged user's id, redirect him back to the LoadBoard

                }

                try
                {
                    var result = await loadService.EditLoadAsync(loadId, model);

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

                return RedirectToAction("LoadDetails", new { loadId = model.Id });
            }

            return RedirectToAction("LoadDetails", new { loadId = model.Id });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [BrokerOnly]

        public async Task<IActionResult> CancelLoad(Guid loadId)
        {
	        if (loadId == Guid.Empty)
	        {
		        TempData.SetErrorMessage(LoadCouldNotBeRetrieved + " " + LoadIdInvalid);
		        return View("LoadDetails");
	        }

	        TempData["isEditing"] = false;

	        Guid? userId = User.GetUserId();

            LoadViewModel load = await loadService.GetLoadByIdAsync(loadId);

            try
            {
                if (userId != load.BrokerId)
                {
                    return RedirectToAction("LoadBoard", "LoadBoard"); // If the load's broker is not the same as logged user, redirect him back to the LoadBoard
                }

                await loadService.CancelLoadAsync(loadId);

                TempData.SetSuccessMessage(LoadCancelledSuccessfully);
                return RedirectToAction("LoadBoard", "LoadBoard");
            }
            catch (Exception e)
            {
                TempData.SetErrorMessage(ErrorCancellingLoad + e.Message);
                return View("LoadDetails", load);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [BrokerOnly]

        public async Task<IActionResult> PostALoad(Guid loadId)
        {
            Guid? userId = User.GetUserId();

            if (!ModelState.IsValid)
            {
                return RedirectToAction("LoadDetails", new { loadId });
            }

            if (loadId == Guid.Empty)
            {
                TempData.SetErrorMessage(LoadIdInvalid);
                return RedirectToAction("LoadBoard", "LoadBoard");
            }

            LoadViewModel load = await loadService.GetLoadByIdAsync(loadId);

            if (userId != load.BrokerId)
            {
                return RedirectToAction("LoadBoard", "LoadBoard"); // If the load's broker is not the same as logged user, redirect him back to the LoadBoard
            }

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
                await loadHub.Clients.All.SendAsync("ReceiveLoadPostedNotification", loadId); // send a notification to all Dispatchers the load is posted using SignalR

                TempData.SetActiveTab(PostedActiveTab); // navigate to the posted tab
                TempData.SetSuccessMessage(LoadPostedSuccessfully);

                return RedirectToAction("LoadBoard", "LoadBoard");
            }
            catch (Exception e)
            {
                TempData.SetErrorMessage(ErrorPostingLoad + e.Message);
                return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [BrokerOnly]

        public async Task<IActionResult> UnpostALoad(Guid loadId)
        {
            Guid? userId = User.GetUserId();

            if (!ModelState.IsValid)
            {
                return RedirectToAction("LoadDetails", new { loadId });
            }

            if (loadId == Guid.Empty)
            {
                TempData.SetErrorMessage(LoadIdInvalid);
                return RedirectToAction("LoadBoard", "LoadBoard");
            }

            LoadViewModel load = await loadService.GetLoadByIdAsync(loadId);


            if (userId != load.BrokerId)
            {
                return RedirectToAction("LoadBoard", "LoadBoard"); // If the load's broker is not the same as logged user, redirect him back to the LoadBoard
            }


            try
            {
                await loadService.UnpostLoadAsync(load.Id); // unpost the load
                await loadHub.Clients.All.SendAsync("ReloadPostedLoadsTable"); // Send SignalR notification on the websocket to refresh the table ( Posted Loads ) to all Clients

                TempData.SetSuccessMessage(LoadUnpostedSuccessfully);

                return RedirectToAction("LoadBoard", "LoadBoard");
            }
            catch (Exception e)
            {
                TempData.SetErrorMessage(ErrorUnpostingLoad + e.Message);
                return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [BrokerOnly]

        public async Task<IActionResult> UnpostAllLoads(Guid brokerId)
        {

            try
            {
                await loadService.UnpostAllLoadsAsync(brokerId); // unpost all the broker's loads
                await loadHub.Clients.All.SendAsync("ReloadPostedLoadsTable"); // Send SignalR notification on the websocket to refresh the table ( Posted Loads ) to all Clients


                TempData.SetSuccessMessage(LoadsUnpostedSuccessfully);

                return RedirectToAction("LoadBoard", "LoadBoard");
            }
            catch (Exception e)
            {
                TempData.SetErrorMessage(ErrorUnpostingLoads);
                return RedirectToAction("LoadBoard", "LoadBoard");
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DispatcherOnly]

        public async Task<IActionResult> BookALoad(Guid loadId)
        {
	        var userId = User.GetUserId().Value; 

	        try
	        {
		        var success = await loadService.BookLoadAsync(loadId, userId);

		        if (!success)
		        {
                    TempData.SetErrorMessage(UnableToBookTheLoad);
					return RedirectToAction("LoadDetails", new { id = loadId });
		        }

                TempData.SetSuccessMessage(LoadWasBookSuccessfully);
                return RedirectToAction("LoadDetails", new { loadId });
			}
			catch (Exception ex)
	        {
		        TempData.SetErrorMessage(UnableToBookTheLoad); 
		        return RedirectToAction("LoadDetails", new { id = loadId });
	        }
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [BrokerOnly]

		public async Task<IActionResult> CancelBooking(Guid loadId)
        {
	        Guid? userId = User.GetUserId();

			if (string.IsNullOrEmpty(userId.ToString()))
			{
				return NotFound();
			}

			try
	        {
		        var success = await loadService.CancelLoadBookingAsync(loadId, userId.Value);

		        if (success)
		        {
			        TempData.SetSuccessMessage(LoadRepostedAgain); 
		        }
		        else
		        {
                    TempData.SetErrorMessage(FailedToCancelLoadCarrier);
		        }
	        }
	        catch (Exception ex)
	        {
                TempData.SetErrorMessage(ex.Message);
	        }

	        return RedirectToAction("LoadDetails", new { loadId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		[DispatcherOnly]
        public async Task<IActionResult> ReturnLoadToBroker(Guid loadId)
        {
	        Guid? userId = User.GetUserId();

	        bool success = await loadService.ReturnLoadBackToBroker(loadId, userId);

	        if (success)
	        {
                TempData.SetSuccessMessage(LoadReturnedToBrokerSuccessfully);
                return RedirectToAction("LoadDetails", new { loadId });
	        }

	        TempData.SetSuccessMessage(FailedToCancelLoadBroker);
	        return RedirectToAction("LoadDetails", new { loadId });
		}

		[HttpPost]
        [ValidateAntiForgeryToken]
        [DispatcherOnly]

        public async Task<IActionResult> DeliverALoad(Guid loadId)
        {
	        try
	        {
		        var success = await loadService.LoadDeliveredAsync(loadId);

		        if (!success)
		        {
			        TempData.SetErrorMessage(UnableToMarkLoadDelivered);
			        return RedirectToAction("LoadDetails", new {  loadId });
		        }

		        TempData.SetSuccessMessage(LoadWasDeliveredSuccessfully);
		        return RedirectToAction("LoadBoard", "LoadBoard");
	        }
	        catch (ArgumentException ex)
	        {
		        TempData.SetErrorMessage(ex.Message);
		        return RedirectToAction("LoadDetails", new {  loadId });
	        }
			catch (Exception ex)
	        {
                TempData.SetErrorMessage(ErrorDeliveringLoad);
		        return RedirectToAction("LoadDetails", new { loadId });
	        }
        }
	}
}
