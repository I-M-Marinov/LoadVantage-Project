using LoadVantage.Common.Enums;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.LoadBoard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Core.Services
{
	[Authorize]
	public class LoadBoardService : ILoadBoardService
	{
		private readonly LoadVantageDbContext dbContext;
		private readonly UserManager<User> userManager;

		public LoadBoardService(LoadVantageDbContext _context, UserManager<User> _userManager)
		{
			dbContext = _context;
			userManager = _userManager;
		}

		public async Task<IEnumerable<LoadViewModel>> GetAllCreatedLoadsForBrokerAsync(Guid userId)
		{
			var createdLoads = await dbContext.Loads
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
			var postedLoads = await dbContext.Loads
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

		public async Task<IEnumerable<LoadViewModel>> GetAllPostedLoadsAsync(Guid userId)
		{
			var postedLoads = await dbContext.Loads
				.Include(load => load.PostedLoad)
				.Where(load => load.Status == LoadStatus.Available)
				.OrderByDescending(load => load.PostedLoad.PostedDate)
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
			}).ToList();
		}

		public async Task<IEnumerable<LoadViewModel>> GetAllBookedLoadsForBrokerAsync(Guid userId)
		{
			// Retrieve the booked loads for the broker
			var bookedLoads = await dbContext.Loads
				.Where(load => load.BookedLoad != null && load.BookedLoad.BrokerId == userId)
				.Include(load => load.BookedLoad)
				.ThenInclude(bookedLoad => bookedLoad.Dispatcher)
				.Include(load => load.BookedLoad)
				.ThenInclude(bookedLoad => bookedLoad.Driver)
				.ThenInclude(driver => driver.Truck)
				.ToListAsync();

			// Select and map to LoadViewModel
			var loadViewModels = bookedLoads.Select(load => new LoadViewModel
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

				// Include Dispatcher Info if available
				DispatcherInfo = load.BookedLoad.DispatcherId != null
					? new DispatcherInfoViewModel
					{
						DispatcherName = load.BookedLoad.Dispatcher.FullName,
						DispatcherEmail = load.BookedLoad.Dispatcher.Email,
						DispatcherPhone = load.BookedLoad.Dispatcher.PhoneNumber
					}
					: null,

				// Include Driver Info if available
				DriverInfo = load.BookedLoad.DriverId != null
					? new DriverInfoViewModel
					{
						DriverName = load.BookedLoad.Driver.FullName,
						DriverTruckNumber = load.BookedLoad.Driver.Truck.TruckNumber,
						DriverLicenseNumber = load.BookedLoad.Driver.LicenseNumber
					}
					: null
			}).ToList();

			return loadViewModels;
		}

		public async Task<IEnumerable<LoadViewModel>> GetAllBookedLoadsForDispatcherAsync(Guid userId)
		{
			var bookedLoads = await dbContext.Loads
				.Include(l => l.BookedLoad)
				.Where(load => load.BookedLoad!.DispatcherId == userId)
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
				DispatcherId = load.BookedLoad.DispatcherId,
				DriverId = load.BookedLoad.DriverId

			});
		}

		public async Task<IEnumerable<LoadViewModel>> GetAllBilledLoadsForBrokerAsync(Guid userId)
		{
			var billedLoads = await dbContext.Loads
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

		public async Task<IEnumerable<LoadViewModel>> GetAllBilledLoadsForDispatcherAsync(Guid userId)
		{
			var billedLoads = await dbContext.Loads
				.Include(l => l.BilledLoad)
				.Include(l => l.BookedLoad)
				.Where(load =>
					load.BookedLoad != null && load.BookedLoad.DispatcherId == userId &&
					load.Status == LoadStatus.Booked)
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

		public async Task<LoadBoardViewModel> GetBrokerLoadBoardAsync(Guid userId)
		{
			var user = await userManager.Users
				.FirstOrDefaultAsync(u => u.Id == userId);

			var createdLoads = await GetAllCreatedLoadsForBrokerAsync(userId);
			var postedLoads = await GetAllPostedLoadsForBrokerAsync(userId);
			var bookedLoads = await GetAllBookedLoadsForBrokerAsync(userId);
			var billedLoads = await GetAllBilledLoadsForBrokerAsync(userId);

			var userImage = await dbContext.UsersImages.SingleOrDefaultAsync(ui => ui.UserId == userId);


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

			var loadBoardViewModel = new LoadBoardViewModel
			{
				UserId = userId,
				FirstName = user.FirstName,
				LastName = user.LastName,
				CompanyName = user.CompanyName,
				Position = user.Position,
				CreatedLoads = createdLoads.ToList(),
				PostedLoads = postedLoads,
				BookedLoads = bookedLoads.ToList(),
				BilledLoads = billedLoads.ToList(),
				Profile = profileModel
			};

			return loadBoardViewModel;
		}

		public async Task<LoadBoardViewModel> GetDispatcherLoadBoardAsync(Guid userId)
		{
			var user = await userManager.Users
				.FirstOrDefaultAsync(u => u.Id == userId);

			var createdLoads = await GetAllCreatedLoadsForBrokerAsync(userId);
			var postedLoads = await GetAllPostedLoadsAsync(userId);
			var bookedLoads = await GetAllBookedLoadsForDispatcherAsync(userId);
			var billedLoads = await GetAllBilledLoadsForBrokerAsync(userId);

			var userImage = await dbContext.UsersImages.SingleOrDefaultAsync(ui => ui.UserId == userId);


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
				PostedLoads = postedLoads,
				BookedLoads = bookedLoads.ToList(),
				BilledLoads = billedLoads.ToList(),
				Profile = profileModel
			};
		}

		public async Task<Dictionary<LoadStatus, int>> GetLoadCountsForBrokerAsync(Guid brokerId)
		{
			var loadCounts = await dbContext.Loads
				.Where(load => load.BrokerId == brokerId)
				.AsNoTracking()
				.GroupBy(load => load.Status)
				.Select(group => new
				{
					Status = group.Key,
					Count = group.Count()
				})
				.ToDictionaryAsync(result => result.Status, result => result.Count);

			return loadCounts;
		}

		public async Task<Dictionary<LoadStatus, int>> GetLoadCountsForDispatcherAsync(Guid dispatcherId)
		{
			var loadCounts = await dbContext.Loads
				.Include(l => l.BookedLoad)
				.Include(l => l.BilledLoad)
				.Where(load => load.BookedLoad!.DispatcherId == dispatcherId)
				.AsNoTracking()
				.GroupBy(load => load.Status)
				.Select(group => new
				{
					Status = group.Key,
					Count = group.Count()
				})
				.ToDictionaryAsync(result => result.Status, result => result.Count);

			return loadCounts;
		}

		public async Task<Dictionary<string, Dictionary<LoadStatus, int>>> GetLoadCountsForUserAsync(Guid userId,
			string userPosition)
		{

			if (userPosition == nameof(Broker))
			{
				var brokerLoadCounts = await GetLoadCountsForBrokerAsync(userId);
				return new Dictionary<string, Dictionary<LoadStatus, int>>
				{
					{ nameof(Broker), brokerLoadCounts }
				};
			}
			else if (userPosition == nameof(Dispatcher))
			{
				var dispatcherLoadCounts = await GetLoadCountsForDispatcherAsync(userId);
				return new Dictionary<string, Dictionary<LoadStatus, int>>
				{
					{ nameof(Dispatcher), dispatcherLoadCounts }
				};
			}
			else
			{
				throw new ArgumentException("Invalid user type");
			}
		}
	}
}
