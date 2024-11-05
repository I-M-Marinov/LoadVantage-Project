using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LoadVantage.Areas.Broker.Contracts;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Filters;
using LoadVantage.Areas.Broker.Services;
using LoadVantage.Common.Enums;
using LoadVantage.Extensions;
using LoadVantage.Infrastructure.Data.Models;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using static LoadVantage.Common.GeneralConstants.SuccessMessages;
using LoadVantage.Core.Services;
using Microsoft.EntityFrameworkCore;
using static LoadVantage.Common.GeneralConstants;

namespace LoadVantage.Areas.Broker.Controllers
{
    [BrokerOnly]
    [Area("Broker")]
    [Route("Broker")]
    public class BrokerController(IBrokerService brokerService, IBrokerLoadBoardService brokerLoadBoardService, ILoadStatusService loadService, ILogger<BrokerController> logger) : Controller
    {
        [HttpGet]
        [Route("Profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound(); // User ID not found
            }

            var broker = await brokerService.GetBrokerInformationAsync(userId);

            if (broker == null)
            {

                return View(); // Broker was not found
            }

            return View(broker); // Pass the broker info to the view
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

        [HttpGet]
        [Route("Load")]
        public async Task<IActionResult> LoadDetails(Guid loadId)
        {

            try
            {
                var loadToShow = await loadService.SeeLoadDetails(loadId);

                if (loadToShow == null)
                {
                    TempData["ErrorMessage"] = LoadInformationCouldNotBeRetrieved;

                    return RedirectToAction("LoadDetails", new { loadId });
                }

                return View(loadToShow);
            }
            catch (Exception e)
            {
                logger.LogError(e, ErrorEditingLoad);
                TempData["ErrorMessage"] = ErrorEditingLoad;

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
                // debugging purposes only
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }

                return View(model);
            }

            try
            {
                var loadId = await loadService.CreateLoadAsync(model, brokerId);

                // View the load after creation in the LoadDetails View
                return RedirectToAction("LoadDetails", loadId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorCreatingLoad);
                ModelState.AddModelError(string.Empty, ErrorCreatingLoad);
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
                    // add the models state errors so they are visible in the View
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                        TempData["ErrorMessage"] += $" {error.ErrorMessage}";
                    }

                    return RedirectToAction("LoadDetails", new { loadId = model.Id });

                }

                try
                {
                    await loadService.EditLoadAsync(loadId, model);

                    TempData["isEditing"] = false;
                    TempData["SuccessMessage"] = LoadUpdatedSuccessfully;

                }
                catch (Exception e)
                {
                    logger.LogError(ErrorUpdatingLoad);
                    ModelState.AddModelError(string.Empty, e.Message);
                    TempData["ErrorMessage"] = e.Message;

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
				TempData["ErrorMessage"] = LoadCouldNotBeRetrieved + " " + LoadIdInvalid;
				return View("LoadDetails");
            }

            LoadViewModel load = await loadService.GetLoadByIdAsync(loadId);

            try
            {
                await loadService.CancelLoadAsync(loadId);
                TempData["SuccessMessage"] = LoadCancelledSuccessfully;
                return RedirectToAction("LoadBoard");
            }
            catch (Exception e)
            {
                logger.LogError(ErrorCancellingLoad);
                TempData["ErrorMessage"] = ErrorCancellingLoad;
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
                TempData["ErrorMessage"] = LoadIdInvalid;
                return View("LoadBoard");
            }

            LoadViewModel load = await loadService.GetLoadByIdAsync(loadId);

            if (load.Status != LoadStatus.Created.ToString())
            {
                if (load.Status == LoadStatus.Available.ToString())
                {
                    TempData["ErrorMessage"] = LoadIsAlreadyPosted;
                }
                else
                {
                    TempData["ErrorMessage"] = LoadIsNotInCorrectStatus;
                }

                return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }

            try
            {
                await loadService.PostLoadAsync(load.Id);

                TempData["ActiveTab"] = "posted";
                TempData["SuccessMessage"] = LoadPostedSuccessfully;
                return RedirectToAction("LoadBoard");
			}
            catch (Exception e)
            {
                TempData["ErrorMessage"] = ErrorPostingLoad;
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
                TempData["ErrorMessage"] = LoadIdInvalid;
                return View("LoadBoard");
            }

            LoadViewModel load = await loadService.GetLoadByIdAsync(loadId);

            if (load.Status != LoadStatus.Available.ToString())
            {
                if (load.Status == LoadStatus.Created.ToString())
                {
                    TempData["ErrorMessage"] = LoadIsNotPosted; // if the load is already not posted ( created status ) 
                }
                else
                {
                    TempData["ErrorMessage"] = LoadIsNotInCorrectStatus; // if the load is booked or billed 
                }

                return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }

            try
            {
                await loadService.UnpostLoadAsync(load.Id); // unpost the load

                TempData["ActiveTab"] = "created"; // navigate to the created tab
                TempData["SuccessMessage"] = LoadUnpostedSuccessfully; 
                return RedirectToAction("LoadBoard");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = ErrorUnpostingLoad;
                return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }
        }
    }
}
