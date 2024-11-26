using CloudinaryDotNet.Actions;
using LoadVantage.Common.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Driver;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Core.Models.Truck;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.Identity.Client;
using System.Diagnostics;


namespace LoadVantage.Core.Services
{
	public class DriverService : IDriverService
	{
		private readonly LoadVantageDbContext context;
		private readonly IProfileService profileService;
		private readonly IUserService userService;

		public DriverService(LoadVantageDbContext _context, IProfileService _profileService, IUserService _userService)
		{
			context = _context;
			profileService = _profileService;
			userService = _userService;
		}

		public async Task<DriversViewModel> GetAllDriversAsync(Guid userId)
		{
			var user = await userService.GetUserByIdAsync(userId);

			ProfileViewModel profile = await profileService.GetUserInformation(user.Id);

			var drivers = await context.Drivers
				.Include(d => d.Truck)
				.Where(d => d.IsFired != true)
				.Where(d => d.DispatcherId == userId)
				.Select(d => new DriverViewModel
				{
					Id = d.DriverId,
					FirstName = d.FirstName,
					LastName = d.LastName,
					LicenseNumber = d.LicenseNumber,
					TruckNumber = d.Truck != null ? d.Truck.TruckNumber : "N/A",
					IsAvailable = d.IsAvailable,
					IsBusy = d.IsBusy
				})
				.OrderBy(d => d.IsAvailable)
				.ThenBy(d => d.FirstName)
				.ToListAsync();

			var driverViewModel = new DriversViewModel()
			{
				Profile = profile,
				Drivers = drivers,
				NewDriver = new DriverViewModel(),
				EditedDriver = new DriverViewModel()
			};

			return driverViewModel;
		}

		public async Task<DriverViewModel?> GetDriverByIdAsync(Guid id)
		{
			var user = await userService.GetCurrentUserAsync();


			var driver = await context.Drivers
				.Include(d => d.Truck)
				.Where(d => d.IsFired == false)
				.Where(d => d.DispatcherId == user.Id)
				.Where(d => d.DriverId == id)
				.FirstOrDefaultAsync();

			if (driver == null)
			{
				return null;
			}

			var model = new DriverViewModel()
			{
				Id = driver.DriverId,
				FirstName = driver.FirstName,
				LastName = driver.LastName,
				LicenseNumber = driver.LicenseNumber,
				TruckNumber = driver.Truck != null ? driver.Truck.TruckNumber : "N/A",
				IsAvailable = driver.IsAvailable,
				IsBusy = driver.IsBusy
			};

			return model;
		}

		public async Task AddDriverAsync(DriverViewModel model, Guid userId)
		{
			var driver = new Driver
			{
				DriverId = model.Id,
				FirstName = model.FirstName,
				LastName = model.LastName,
				LicenseNumber = model.LicenseNumber,
				DispatcherId = userId,
				TruckId = null,
				IsAvailable = true,
				IsFired = false,
				IsBusy = false
			};

			context.Drivers.Add(driver);
			await context.SaveChangesAsync();
		}

		public async Task UpdateDriverAsync(DriverViewModel model)
		{
			var driver = await context.Drivers.FindAsync(model.Id);

			if (driver == null)
			{
				throw new KeyNotFoundException("Driver not found.");
			}

			driver.FirstName = model.FirstName;
			driver.LastName = model.LastName;
			driver.LicenseNumber = model.LicenseNumber;
			driver.IsFired = model.isFired;
			driver.IsBusy = model.IsBusy;

			context.Drivers.Update(driver);
			await context.SaveChangesAsync();
		}

		public async Task FireDriverAsync(Guid id)
		{
			var driver = await context.Drivers.FindAsync(id);

			if (driver == null)
			{
				throw new KeyNotFoundException("Driver not found.");
			}

			driver.IsBusy = false; // set work status to false 
			driver.IsAvailable = false; // set availability to false before firing the employee 
			driver.IsFired = true; // and fire the employee ---> Soft delete 

			await context.SaveChangesAsync();
		}

		public async Task<List<Driver>> GetAvailableDriversAsync()
		{
			var user = await userService.GetCurrentUserAsync();

			var availableDrivers = await context.Drivers
				.Where(d => d.IsAvailable)
				.Where(d => d.IsBusy == false)
				.Where(d => d.IsFired == false)
				.Where(d => d.DispatcherId == user.Id)
				.OrderBy(d => d.FirstName)
				.ToListAsync();

			return availableDrivers;
		}

		public async Task<bool> AssignADriverToLoadAsync(Guid loadId, Guid driverId, Guid userId)
		{

			var load = await context.Loads
				.Include(l => l.BookedLoad)
				.FirstOrDefaultAsync(l =>
					l.Id == loadId &&
					l.Status == LoadStatus.Booked &&
					l.BookedLoad.DispatcherId == userId);



			var driver = await context.Drivers
				.Include(d => d.Dispatcher)
				.Include(d => d.Truck)
				.FirstOrDefaultAsync(d =>
					d.DriverId == driverId &&
					d.DispatcherId == userId &&
					d.TruckId != null);

			if (load == null || driver == null)
			{
				return false;
			}

			if (load.BookedLoad.Driver != null) // if there is another driver assigned, change his work status IsBusy to false
			{
				load.BookedLoad.Driver.IsBusy = false;
				await context.SaveChangesAsync();
			}

			load.BookedLoad.DriverId = driver.DriverId; // set the driver for that load 
			driver.IsBusy = true; // mark driver as busy so he is not showing for another load until load is delivered

			await context.SaveChangesAsync();

			return true;

		}

		public async Task<List<Driver>> GetDriversWithTrucksAsync()
		{
			var user = await userService.GetCurrentUserAsync();

			var driversWithTrucks = await context.Drivers
				.Where(d => d.TruckId != null && d.DispatcherId == user.Id)
				.Where(d => d.IsBusy == false) // get only drivers with trucks and not working at the moment
				.ToListAsync();

			return driversWithTrucks;
		}
	}
}
