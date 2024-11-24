using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Driver;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Core.Models.Truck;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.Identity.Client;


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
				.Select(d => new DriverViewModel
				{
					Id = d.DriverId,
					FirstName = d.FirstName,
					LastName = d.LastName,
					LicenseNumber = d.LicenseNumber,
					TruckNumber = d.Truck != null ? d.Truck.TruckNumber : "N/A",
					IsAvailable = d.IsAvailable
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
			var driver = await context.Drivers
				.Include(d => d.Truck)
				.Where(d => d.IsFired == false)
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
				IsAvailable = driver.IsAvailable
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
				IsFired = false
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
			driver.IsAvailable = model.IsAvailable;

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

			driver.IsAvailable = false; // set availability to false before firing the employee 
			driver.IsFired = true; // and fire the employee ---> Soft delete 

			await context.SaveChangesAsync();
		}
	}
}
