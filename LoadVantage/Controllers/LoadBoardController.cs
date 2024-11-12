using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LoadVantage.Controllers
{
	public class LoadBoardController(ILoadBoardService loadBoardService) : Controller
	{

		[HttpGet]
		public async Task<IActionResult> LoadBoard()
		{
			Guid? userId = User.GetUserId();
			var loadBoardInfo = await loadBoardService.GetLoadBoardAsync(userId.Value);

			return View(loadBoardInfo);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ReturnToLoadBoard()
		{
			return RedirectToAction(nameof(LoadBoard));
		}
	}
}

