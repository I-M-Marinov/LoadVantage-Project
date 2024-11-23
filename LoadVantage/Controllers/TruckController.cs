using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Truck;
using LoadVantage.Extensions;
using LoadVantage.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using static LoadVantage.Common.GeneralConstants.SuccessMessages;

namespace LoadVantage.Controllers
{
	[Authorize]
	[DispatcherOnly]
	public class TruckController : Controller
	{

		private readonly ITruckService truckService;


        public TruckController(ITruckService _truckService)
		{
			truckService = _truckService;
		}

		[HttpGet]
		public async Task<IActionResult> ShowTrucks(Guid userId)
		{
			var trucks = await truckService.GetAllTrucksAsync(userId);
			trucks.NewTruck = new TruckViewModel();
			trucks.EditedTruck = new TruckViewModel();

			return View(trucks); 
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
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
		public async Task<IActionResult> EditTruck(TrucksViewModel model)
		{
			Guid userId = User.GetUserId()!.Value;

			ModelState.Clear(); // Clear any other model state errors found 
			TryValidateModel(model.EditedTruck); // Validate just the truck  

			if (!ModelState.IsValid)
			{
				return RedirectToAction("ShowTrucks", new {userId = userId });
			}

			try
			{
				var editedTruck = model.EditedTruck;

				await truckService.UpdateTruckAsync(editedTruck);
				TempData.SetSuccessMessage(TruckWasUpdatedSuccessfully);

				return RedirectToAction("ShowTrucks",new {userid = userId});
			}
			catch (KeyNotFoundException ex)
			{
				TempData["ErrorMessage"] = ex.Message;
                return NotFound(TruckDoesNotExist);
            }
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Error updating the truck: " + ex.Message;
				return RedirectToAction("ShowTrucks");
			}
		}


		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> DeleteTruck(Guid id)
		{
            Guid userId = User.GetUserId()!.Value;

            try
            {
				await truckService.DeleteTruckAsync(id);
                TempData.SetSuccessMessage(TruckWasRemovedSuccessfully);
                return RedirectToAction("ShowTrucks", new { userid = userId });

            }
			catch (KeyNotFoundException)
			{
				return NotFound(TruckDoesNotExist);
			}
		}
	}
}
