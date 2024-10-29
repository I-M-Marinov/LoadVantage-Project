using LoadVantage.Areas.Dispatcher.Contracts;
using LoadVantage.Areas.Dispatcher.Models;
using LoadVantage.Areas.Dispatcher.Services;
using LoadVantage.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LoadVantage.Core.Contracts;
using static LoadVantage.Common.GeneralConstants.UserRoles;


namespace LoadVantage.Areas.Dispatcher.Controllers
{
    [DispatcherOnly]
    [Area("Dispatcher")]
    [Route("Dispatcher")]
    public class DispatcherController(IDispatcherService dispatcherService, IDispatcherLoadBoardService dispatcherLoadBoardService) : Controller
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

            var dispatcher = await dispatcherService.GetDispatcherInformationAsync(userId);

            if (dispatcher == null)
            {
                return NotFound(); // Dispatcher not found
            }

            return View(dispatcher); // Pass the dispatcher info to the view
        }

        [HttpGet]
        [Route("LoadBoard")]
        public async Task<IActionResult> LoadBoard(Guid dispatcherId)
        {
            var loadBoardData = await dispatcherLoadBoardService.GetDispatcherLoadBoardAsync(dispatcherId);

            return View(loadBoardData);

        }
    }
}
