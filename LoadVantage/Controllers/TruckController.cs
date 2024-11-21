using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Truck;
using LoadVantage.Core.Services;
using LoadVantage.Extensions;
using LoadVantage.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using static LoadVantage.Common.GeneralConstants.SuccessMessages;

namespace LoadVantage.Controllers
{
	[Authorize]
	public class TruckController : Controller
	{

		private readonly ITruckService truckService;

		public TruckController(ITruckService _truckService)
		{
			truckService = _truckService;
		}

		[HttpGet]
		[DispatcherOnly]
		public async Task<IActionResult> ShowTrucks(Guid userId)
		{
			var trucks = await truckService.GetAllTrucksAsync(userId);
			trucks.NewTruck = new TruckViewModel();
			trucks.EditedTruck = new TruckViewModel();

			return View(trucks); 
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[DispatcherOnly]
		public async Task<IActionResult> AddTruck(TrucksViewModel trucksViewModel)
		{
			Guid userId = User.GetUserId()!.Value;

			ModelState.Clear(); // Clear any other model state errors found 
			TryValidateModel(trucksViewModel.NewTruck); // Validate just the truck  

			if (!ModelState.IsValid)
			{
				var updatedViewModel = await truckService.GetAllTrucksAsync(userId);
				updatedViewModel.NewTruck = trucksViewModel.NewTruck;

				var errorMessages = string.Join(" ", ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage));

				TempData.SetErrorMessage(TruckWasNotCreated + errorMessages);
				return View("ShowTrucks", updatedViewModel);
			}

			try
			{
				await truckService.AddTruckAsync(trucksViewModel.NewTruck, userId);
				TempData.SetSuccessMessage(TruckWasAddedSuccessfully);
				return RedirectToAction("ShowTrucks", new {userId = userId});
			}
			catch (Exception ex)
			{
				TempData.SetErrorMessage(TruckCreateError);
				return RedirectToAction("ShowTrucks", new { userId = userId});
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetTruckDetails(Guid id)
		{
			var truck = await truckService.GetTruckByIdAsync(id);

			if (truck == null)
			{
				return NotFound();
			}
			return Json(truck);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[DispatcherOnly]
		public async Task<IActionResult> EditTruck(TrucksViewModel model)
		{
			Guid userId = User.GetUserId()!.Value;

			ModelState.Clear(); // Clear any other model state errors found 
			var result = TryValidateModel(model.EditedTruck); // Validate just the truck  

			if (!ModelState.IsValid)
			{
				return RedirectToAction("ShowTrucks", new {userId = userId });
			}

			try
			{
				var editedTruck = model.EditedTruck;

				await truckService.UpdateTruckAsync(editedTruck);
				TempData.SetSuccessMessage("Truck was updated successfully.");
				return RedirectToAction("ShowTrucks",new {userid = userId});
			}
			catch (KeyNotFoundException ex)
			{
				TempData["ErrorMessage"] = ex.Message;
				return RedirectToAction("ShowTrucks");
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Error updating the truck: " + ex.Message;
				return RedirectToAction("ShowTrucks");
			}
		}

		//// GET: Truck/Delete/5
		//public async Task<IActionResult> Delete(Guid id)
		//{
		//	var truck = await truckService.GetTruckByIdAsync(id);
		//	if (truck == null)
		//	{
		//		return NotFound();
		//	}
		//	return View(truck); // Ensure a corresponding view exists.
		//}

		//// POST: Truck/Delete/5
		//[HttpPost, ActionName("Delete")]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> DeleteConfirmed(Guid id)
		//{
		//	try
		//	{
		//		await truckService.DeleteTruckAsync(id);
		//	}
		//	catch (KeyNotFoundException)
		//	{
		//		return NotFound();
		//	}
		//	return RedirectToAction(nameof(Index));
		//}
	}
}
