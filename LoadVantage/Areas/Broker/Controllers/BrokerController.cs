using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LoadVantage.Areas.Broker.Contracts;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Filters;
using LoadVantage.Areas.Broker.Services;

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
                return null;
            }

            return View(loadToShow); // Specify the view name if needed
        }

        [HttpGet]
        [Route("Load/Create")]
        public async Task<IActionResult> CreateLoad(LoadViewModel model)
        {

            return View(model);
        }

        [HttpPost]
        [Route("Load/Create")]
        public async Task<IActionResult> CreateLoad(LoadViewModel model, Guid brokerId)
        {

            if (!ModelState.IsValid)
            {
                // Return the same view with the current model if validation fails
                return View(model);
            }

            var loadId = await loadService.CreateLoadAsync(model, brokerId);

            // Redirect to a success or details page, or to a list of loads, using the new load ID
            return RedirectToAction("LoadDetails", new { loadId });

        }

    }
}
