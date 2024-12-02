using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.Statistics;
using LoadVantage.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoadVantage.Areas.Admin.Controllers
{
	[Authorize]
	[AdminOnly]
	[Area("Admin")]
	public class StatisticsController : Controller
    {
        private static IStatisticsService statisticsService;

        public StatisticsController(IStatisticsService _statisticsService)
        {
			statisticsService = _statisticsService;
        }

		[HttpGet]
		public async Task<IActionResult> Statistics(Guid adminId)
        {

            var model = await statisticsService.GetAllStatistics(adminId);

			return View("~/Areas/Admin/Views/Admin/Statistics/Statistics.cshtml", model);
		}
	}
}
