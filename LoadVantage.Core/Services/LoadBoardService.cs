﻿using LoadVantage.Common.Enums;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.LoadBoard;
using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Core.Services
{
	public class LoadBoardService(LoadVantageDbContext context, UserManager<User> userManager) : ILoadBoardService
	{
		public async Task<IEnumerable<LoadViewModel>> GetAllCreatedLoadsForBrokerAsync(Guid userId)
		{
			var createdLoads = await context.Loads
				.Where(load => load.Status == LoadStatus.Created && load.BrokerId == userId)
				.OrderBy(l => l.PickupTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.ToListAsync();

			return createdLoads.Select(load => new LoadViewModel()
			{
				Id = load.Id,
				OriginCity = load.OriginCity,
				OriginState = load.OriginState,
				DestinationCity = load.DestinationCity,
				DestinationState = load.DestinationState,
				PickupTime = load.PickupTime,
				DeliveryTime = load.DeliveryTime,
				PostedPrice = load.Price,
				Distance = load.Distance,
				Weight = load.Weight,
				Status = load.Status.ToString(),
				BrokerId = load.BrokerId
			});
		}

		public async Task<IEnumerable<LoadViewModel>> GetAllPostedLoadsForBrokerAsync(Guid userId)
		{
			var postedLoads = await context.Loads
				.Where(load => load.Status == LoadStatus.Available && load.BrokerId == userId)
				.ToListAsync();

			return postedLoads.Select(load => new LoadViewModel()
			{
				Id = load.Id,
				OriginCity = load.OriginCity,
				OriginState = load.OriginState,
				DestinationCity = load.DestinationCity,
				DestinationState = load.DestinationState,
				PickupTime = load.PickupTime,
				DeliveryTime = load.DeliveryTime,
				PostedPrice = load.Price,
				Distance = load.Distance,
				Weight = load.Weight,
				Status = load.Status.ToString(),
				BrokerId = load.BrokerId
			});
		}

		public async Task<IEnumerable<LoadViewModel>> GetAllBookedLoadsForBrokerAsync(Guid userId)
		{
			var bookedLoads = await context.Loads
				.Where(load => load.BookedLoad != null && load.BrokerId == userId)
				.ToListAsync();

			return bookedLoads.Select(load => new LoadViewModel()
			{
				Id = load.Id,
				OriginCity = load.OriginCity,
				OriginState = load.OriginState,
				DestinationCity = load.DestinationCity,
				DestinationState = load.DestinationState,
				PickupTime = load.PickupTime,
				DeliveryTime = load.DeliveryTime,
				PostedPrice = load.Price,
				Distance = load.Distance,
				Weight = load.Weight,
				Status = load.Status.ToString(),
				BrokerId = load.BrokerId,

			});
		}

		public async Task<IEnumerable<LoadViewModel>> GetAllBilledLoadsForBrokerAsync(Guid userId)
		{
			var billedLoads = await context.Loads
				 .Where(load => load.BilledLoad != null && load.BrokerId == userId)
				 .ToListAsync();

			return billedLoads.Select(load => new LoadViewModel
			{
				Id = load.Id,
				OriginCity = load.OriginCity,
				OriginState = load.OriginState,
				DestinationCity = load.DestinationCity,
				DestinationState = load.DestinationState,
				PickupTime = load.PickupTime,
				DeliveryTime = load.DeliveryTime,
				PostedPrice = load.Price,
				Distance = load.Distance,
				Weight = load.Weight,
				Status = load.Status.ToString(),
				BrokerId = load.BrokerId,
			});
		}

		public async Task<LoadBoardViewModel> GetLoadBoardAsync(Guid userId)
		{
			var user = await userManager.Users
				.FirstOrDefaultAsync(u => u.Id == userId);

			var createdLoads = await GetAllCreatedLoadsForBrokerAsync(userId);
			var postedLoads = await GetAllPostedLoadsForBrokerAsync(userId);
			var bookedLoads = await GetAllBookedLoadsForBrokerAsync(userId);
			var billedLoads = await GetAllBilledLoadsForBrokerAsync(userId);

			var userImage = await context.UsersImages.SingleOrDefaultAsync(ui => ui.UserId == userId);


			var profileModel = new ProfileViewModel
			{
				Username = user.UserName,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				CompanyName = user.CompanyName,
				Position = user.Position,
				PhoneNumber = user.PhoneNumber,
				UserImageUrl = userImage?.ImageUrl
			};

			return new LoadBoardViewModel
			{
				UserId = userId,
				FirstName = user.FirstName,
				LastName = user.LastName,
				CompanyName = user.CompanyName,
				Position = user.Position,
				CreatedLoads = createdLoads.ToList(),
				PostedLoads = postedLoads.ToList(),
				BookedLoads = bookedLoads.ToList(),
				BilledLoads = billedLoads.ToList(),
				Profile = profileModel
			};
		}
	}
	
}