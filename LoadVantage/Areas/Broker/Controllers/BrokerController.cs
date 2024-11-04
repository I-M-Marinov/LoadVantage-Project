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


        [HttpGet]
        [Route("Load")]
        public async Task<IActionResult> LoadDetails(Guid loadId)
        {
            var loadToShow = await loadService.SeeLoadDetails(loadId);

            if (loadToShow == null)
            {
                TempData["ErrorMessage"] = LoadInformationCouldNotBeRetrieved;
                return View(nameof(LoadBoard));
            }

			return View(loadToShow); 
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
                    Console.WriteLine(error.ErrorMessage); 
                }

                return View(model);
            }

            try
            {
                var loadId = await loadService.CreateLoadAsync(model, brokerId);
                var result = await loadService.GetLoadByIdAsync(loadId);

                // Redirect to the details page ( which is exactly the same, but user not allowed to edit )
                return RedirectToAction("LoadDetails", new { loadId });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating load");
                ModelState.AddModelError(string.Empty, ErrorCreatingLoad);
                return View(model);
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
                    // debugging purposes only
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }

                    return View("LoadDetails", model);

                }

                await loadService.EditLoadAsync(loadId, model); 

                TempData["isEditing"] = false;

                return RedirectToAction("LoadDetails", new { loadId });
            }

            return RedirectToAction("LoadDetails", new { loadId });
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Cancel")] 

        public async Task<IActionResult> CancelLoad(Guid loadId)
        {
            TempData["isEditing"] = false;

            if (loadId == Guid.Empty)
            {
				TempData["ErrorMessage"] = LoadCouldNotBeRetrieved;
				return View("LoadDetails");
            }

            if (await loadService.CancelLoadAsync(loadId))
            {
	            TempData["SuccessMessage"] = LoadCancelledSuccessfully;
				return RedirectToAction("LoadBoard");
            }

			TempData["ErrorMessage"] = LoadIdInvalid;
			return View("LoadDetails");
        }

    }
}
