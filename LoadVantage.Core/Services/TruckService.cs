using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Core.Models.Truck;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;


namespace LoadVantage.Core.Services
{
	public class TruckService : ITruckService
	{

		private readonly LoadVantageDbContext context;
		private readonly IProfileService profileService;
		private readonly IUserService userService;

		public TruckService(LoadVantageDbContext _context, IProfileService _profileService, IUserService _userService)
		{
			context = _context;
			profileService = _profileService;
			userService = _userService;
		}

		public async Task<TrucksViewModel> GetAllTrucksAsync(Guid userId)
		{
			var user = await userService.GetUserByIdAsync(userId);

			ProfileViewModel profile = await profileService.GetUserInformation(user.Id);

			var trucks = await context.Trucks
				.Include(t => t.Driver)
				.Where(t => t.IsActive)
				.Where(t => t.DispatcherId == userId)
				.OrderByDescending(t => t.Driver.FirstName)
				.Select(t => new TruckViewModel
				{
					Id = t.Id,
					TruckNumber = t.TruckNumber,
					Make = t.Make,
					Model = t.Model,
					Year = t.Year.ToString(),
					DriverName = t.Driver != null ? t.Driver.FullName : "N/A",
					DriverId = t.Driver.DriverId.ToString(),
					IsAvailable = t.IsAvailable
				})
				.OrderBy(t => t.IsAvailable)
				.ToListAsync();

			var trucksViewModel = new TrucksViewModel
			{
				Profile = profile,
				Trucks = trucks,
				NewTruck = new TruckViewModel() 
			};

			return trucksViewModel;
		}

		public async Task<TruckViewModel?> GetTruckByIdAsync(Guid id)
		{
			var user = await userService.GetCurrentUserAsync();

			var truck = await context.Trucks
				.Include(t => t.Driver)
				.Where(t => t.IsActive && t.Id == id)
				.Where(t => t.DispatcherId == user.Id)
				.FirstOrDefaultAsync();

			if (truck == null)
			{
				return null;
			}

			TruckViewModel model = new TruckViewModel
			{
				Id = truck.Id,
				TruckNumber = truck.TruckNumber,
				Make = truck.Make,
				Model = truck.Model,
				Year = truck.Year.ToString(),
				IsAvailable = truck.IsAvailable
			};

			return model;
		}

		public async Task AddTruckAsync(TruckViewModel model, Guid userId)
		{
			var truck = new Truck
			{
				TruckNumber = model.TruckNumber,
				Make = model.Make,
				Model = model.Model,
				Year = int.Parse(model.Year),
				DispatcherId = userId,
				DriverId = null,
				IsAvailable = true,
				IsActive = true,
			};

			context.Trucks.Add(truck);
			await context.SaveChangesAsync();
		}

		public async Task UpdateTruckAsync(TruckViewModel model)
		{
			var truck = await context.Trucks.FindAsync(model.Id);

			if (truck == null)
			{
				throw new KeyNotFoundException("Truck not found.");
			}

			truck.TruckNumber = model.TruckNumber;
			truck.Make = model.Make;
			truck.Model = model.Model;
			truck.Year = int.Parse(model.Year);

			context.Trucks.Update(truck);
			await context.SaveChangesAsync();
		}

		public async Task DeleteTruckAsync(Guid id)
		{
			var truck = await context.Trucks.FindAsync(id);

            if (truck == null)
            {
                throw new KeyNotFoundException("Truck not found.");
            }

            if (!truck.IsAvailable)
            {
	            throw new InvalidOperationException("You cannot delete a truck that is currently in use !");
			}

            truck.IsAvailable = false; // set availability to false before decommissioning the truck 
			truck.IsActive = false; // Soft delete 

			await context.SaveChangesAsync();
		}

		public async Task<bool> AssignDriverToTruckAsync(Guid truckId, Guid driverId)
		{
			var truck = await context.Trucks
				.FirstOrDefaultAsync(t => t.Id == truckId);
			var driver = await context.Drivers
				.FirstOrDefaultAsync(d => d.DriverId == driverId);

			if (truck == null || driver == null || !driver.IsAvailable || !truck.IsAvailable)
			{
				return false;
			}

			truck.DriverId = driverId; // set the driver for that truck 
			driver.TruckId = truckId; // set that truck for that driver 
			driver.IsAvailable = false; // Mark driver as unavailable
			truck.IsAvailable = false; // Mark truck as unavailable

			var result = await context.SaveChangesAsync();

			return result > 0; // true if one or more lines were affected 
		}

		public async Task<bool> ParkTruckAsync(Guid truckId, Guid driverId)
		{
			var truck = await context.Trucks
				.FirstOrDefaultAsync(t => t.Id == truckId);
			var driver = await context.Drivers
				.FirstOrDefaultAsync(d => d.DriverId == driverId);

			if (truck == null || driver == null || driver.IsAvailable || truck.IsAvailable)
			{
				return false;
			}

			truck.DriverId = null; // set the driver for that truck 
			driver.TruckId = null; // set that truck for that driver 
			driver.IsAvailable = true; // Mark driver as available
			truck.IsAvailable = true; // Mark truck as available

			var result = await context.SaveChangesAsync();

			return result > 0; // true if one or more lines were affected 
		}

	}
}
