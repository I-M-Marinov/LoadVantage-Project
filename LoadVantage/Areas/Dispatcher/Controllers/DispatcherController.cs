using LoadVantage.Areas.Dispatcher.Contracts;
using LoadVantage.Areas.Dispatcher.Models;
using LoadVantage.Areas.Dispatcher.Services;
using LoadVantage.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static LoadVantage.Common.GeneralConstants.UserRoles;


namespace LoadVantage.Areas.Dispatcher.Controllers
{
    [DispatcherOnly]
    [Area("Dispatcher")]
    [Route("Dispatcher")]
    public class DispatcherController(IDispatcherService dispatcherService) : Controller
    {

        [HttpGet]
        [Route("Dashboard")]
        public async Task<IActionResult> DispatcherDashboard()
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
    }
}
