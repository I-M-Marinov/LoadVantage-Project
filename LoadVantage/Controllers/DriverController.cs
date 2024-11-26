using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Driver;
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
	[DispatcherOnly]
	public class DriverController : Controller
	{
		private readonly IDriverService driverService;

		public DriverController(IDriverService _driverService)
		{
			driverService = _driverService;
		}

		[HttpGet]
		public async Task<IActionResult> ShowDrivers(Guid userId)
		{
			var drivers = await driverService.GetAllDriversAsync(userId);
			drivers.NewDriver = new DriverViewModel();
			drivers.EditedDriver = new DriverViewModel();

			return View(drivers);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddDriver(DriversViewModel driversViewModel)
		{
			Guid userId = User.GetUserId()!.Value;

			ModelState.Clear(); // Clear any other model state errors found 
			driversViewModel.NewDriver.Id = Guid.NewGuid();
			TryValidateModel(driversViewModel.NewDriver); // Validate just the driver  

			if (!ModelState.IsValid)
			{
				var updatedViewModel = await driverService.GetAllDriversAsync(userId);
				updatedViewModel.NewDriver = driversViewModel.NewDriver;

				var errorMessages = string.Join(" ", ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage));

				TempData.SetErrorMessage(DriverWasNotCreated + errorMessages);
				return View("ShowDrivers", updatedViewModel);
			}

			try
			{
				await driverService.AddDriverAsync(driversViewModel.NewDriver, userId);
				TempData.SetSuccessMessage(DriverWasAddedSuccessfully);
				return RedirectToAction("ShowDrivers", new { userId = userId });
			}
			catch (Exception ex)
			{
				TempData.SetErrorMessage(DriverCreateError);
				return RedirectToAction("ShowDrivers", new { userId = userId });
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetDriverDetails(Guid id)
		{
			var driver = await driverService.GetDriverByIdAsync(id);

			if (driver == null)
			{
				return NotFound();
			}
			return Json(driver);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditDriver(DriversViewModel model)
		{
			Guid userId = User.GetUserId()!.Value;

			ModelState.Clear(); // Clear any other model state errors found 
			TryValidateModel(model.EditedDriver); // Validate just the driver that is being edited

			if (!ModelState.IsValid)
			{
				return RedirectToAction("ShowDrivers", new { userId = userId });
			}

			try
			{
				var editedDriver = model.EditedDriver;

				await driverService.UpdateDriverAsync(editedDriver);
				TempData.SetSuccessMessage(DriverWasUpdatedSuccessfully);

				return RedirectToAction("ShowDrivers", new { userid = userId });
			}
			catch (KeyNotFoundException ex)
			{
				TempData["ErrorMessage"] = ex.Message;
				return NotFound(DriverDoesNotExist);
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Error updating the truck: " + ex.Message;
				return RedirectToAction("ShowDrivers");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> FireDriver(Guid id)
		{
			Guid userId = User.GetUserId()!.Value;

			try
			{
				await driverService.FireDriverAsync(id);
				TempData.SetSuccessMessage(DriverWasFiredSuccessfully);
				return RedirectToAction("ShowDrivers", new { userid = userId });

			}
			catch (KeyNotFoundException)
			{
				return NotFound(DriverDoesNotExist);
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetAvailableDrivers()
		{
			try
			{
				var availableDrivers = await driverService.GetAvailableDriversAsync();
				var driverList = availableDrivers.Select(d => new
				{
					Id = d.DriverId,
					Name = d.FullName
				});

				return Json(driverList); 
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while retrieving drivers.");
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetDriversWithTrucks()
		{
			try
			{
				var drivers = await driverService.GetDriversWithTrucksAsync();
				var driverList = drivers.Select(d => new
				{
					DriverId = d.DriverId, 
					Name = d.FullName
				});

				return Json(driverList);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while retrieving drivers.");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AssignADriverToLoad(Guid loadId, Guid driverId)
		{
			Guid userId = User.GetUserId().Value; 

			bool result = await driverService.AssignADriverToLoadAsync(loadId, driverId, userId);

			if (!result)
			{
				TempData.SetErrorMessage(DriverWasNotAssignedToTheLoad);
				return RedirectToAction("LoadDetails", "Load", new {loadId = loadId}); 
			}

			TempData.SetSuccessMessage(DriverWasAssignedToLoadSuccessfully);
			return RedirectToAction("LoadDetails", "Load", new { loadId = loadId });
		}



	}
}
