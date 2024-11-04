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

namespace LoadVantage.Areas.Broker.Controllers
{
    [BrokerOnly]
    [Area("Broker")]
    [Route("Broker")]
    public class BrokerController(IBrokerService brokerService, IBrokerLoadBoardService brokerLoadBoardService, ILoadStatusService loadService) : Controller
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
                TempData.SetErrorMessage(LoadCouldNotBeRetrieved);
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

            var loadId = await loadService.CreateLoadAsync(model, brokerId);

            // Redirect to the details page ( which is exactly the same, but user not allowed to edit )
            return RedirectToAction("LoadDetails", new { loadId });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

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

	}
}
