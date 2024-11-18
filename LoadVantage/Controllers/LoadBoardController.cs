using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Services;
using LoadVantage.Extensions;
using LoadVantage.Filters;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
			Guid userId = User.GetUserId()!.Value;
			User user = userService.GetUserByIdAsync(userId).Result;

			if (user is Dispatcher)
			{
				TempData.SetActiveTab(PostedActiveTab);
				 var loadBoardInfo = await loadBoardService.GetDispatcherLoadBoardAsync(userId);
				 return View(loadBoardInfo);
			}
			else // Is Broker
			{
				 var loadBoardInfo = await loadBoardService.GetBrokerLoadBoardAsync(userId);
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

