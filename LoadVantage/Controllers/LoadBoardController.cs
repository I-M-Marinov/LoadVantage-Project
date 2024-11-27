using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Extensions;
using LoadVantage.Filters;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LoadVantage.Common.GeneralConstants.ActiveTabs;
namespace LoadVantage.Controllers
{
	[Authorize]
	public class LoadBoardController : Controller
	{
		private readonly IUserService userService;
		private readonly ILoadBoardService loadBoardService;

		public LoadBoardController(ILoadBoardService _loadBoardService, IUserService _userService)

		{
			loadBoardService = _loadBoardService;
			userService = _userService;
		}

		[HttpGet]
		public async Task<IActionResult> LoadBoard()
		{
			BaseUser? user = userService.GetCurrentUserAsync().Result;

			if (user is Dispatcher)
			{
				TempData.SetActiveTab(PostedActiveTab);
				 var loadBoardInfo = await loadBoardService.GetDispatcherLoadBoardAsync(user.Id);
				 return View(loadBoardInfo);
			}
			else // Is Broker
			{
				 var loadBoardInfo = await loadBoardService.GetBrokerLoadBoardAsync(user.Id);
				 return View(loadBoardInfo);
			}

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ReturnToLoadBoard()
		{
			return RedirectToAction(nameof(LoadBoard));
		}

		[DispatcherOnly]
		public IActionResult GetPostedLoadsTable(Guid userId)
		{
			IEnumerable<LoadViewModel> loads = loadBoardService.GetAllPostedLoadsAsync(userId).Result;

			return PartialView("_PostedLoadsTablePartial", loads);
		}
	}
}

