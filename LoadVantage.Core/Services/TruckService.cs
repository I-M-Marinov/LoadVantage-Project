using LoadVantage.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Core.Models.Truck;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Core.Services
{
	public class TruckService : ITruckService
	{

		private readonly LoadVantageDbContext context;
		private readonly IProfileService profileService;
		private readonly UserManager<User> userManager;

		public TruckService(LoadVantageDbContext _context, IProfileService _profileService, UserManager<User> _userManager)
		{
			context = _context;
			profileService = _profileService;
			userManager = _userManager;
		}

		public async Task<TrucksViewModel> GetAllTrucksAsync(Guid userId)
		{
			var user = await userManager.FindByIdAsync(userId.ToString());

			ProfileViewModel profile = await profileService.GetUserInformation(user.Id);

			var trucks = await context.Trucks
				.Include(t => t.Driver) 
				.Select(t => new TruckViewModel
				{
					Id = t.Id,
					TruckNumber = t.TruckNumber,
					Make = t.Make,
					Model = t.Model,
					DriverName = t.Driver != null ? t.Driver.FullName : "N/A",
					IsAvailable = t.IsAvailable
				})
				.ToListAsync();

			var trucksViewModel = new TrucksViewModel
			{
				Profile = profile,
				Trucks = trucks,
				NewTruck = new TruckViewModel() 

			};

			return trucksViewModel;
		}

		public async Task<TruckViewModel> GetTruckByIdAsync(Guid id)
		{
			var truck = await context.Trucks
				.Include(t => t.Driver)
				.FirstOrDefaultAsync(t => t.Id == id);

			if (truck == null) return null;

			TruckViewModel model = new TruckViewModel
			{
				Id = truck.Id,
				TruckNumber = truck.TruckNumber,
				Make = truck.Make,
				Model = truck.Model,
				DriverName = truck.Driver?.FullName,
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

			if (truck == null) throw new KeyNotFoundException("Truck not found.");

			truck.TruckNumber = model.TruckNumber;
			truck.Make = model.Make;
			truck.Model = model.Model;
			truck.IsAvailable = model.IsAvailable;

			context.Trucks.Update(truck);
			await context.SaveChangesAsync();
		}

		public async Task DeleteTruckAsync(Guid id)
		{
			var truck = await context.Trucks.FindAsync(id);

			if (truck == null) throw new KeyNotFoundException("Truck not found.");

			context.Trucks.Remove(truck);
			await context.SaveChangesAsync();
		}


	}
}
