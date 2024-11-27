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
		private readonly LoadVantageDbContext context;
		private readonly UserManager<BaseUser> userManager;
		public readonly IProfileService profileService;



		public LoadBoardService(LoadVantageDbContext _context, UserManager<BaseUser> _userManager, IProfileService _profileService)
		{
			context = _context;
			userManager = _userManager;
			profileService = _profileService;

		}

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

		public async Task<IEnumerable<LoadViewModel>> GetAllPostedLoadsAsync(Guid userId)
		{
			var postedLoads = await context.Loads
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
			var bookedLoads = await context.Loads
				.Include(load => load.BookedLoad)
				.ThenInclude(bookedLoad => bookedLoad.Dispatcher)
				.Include(load => load.BookedLoad)
				.ThenInclude(bookedLoad => bookedLoad.Driver)
				.ThenInclude(driver => driver.Truck)
				.Where(load => load.BookedLoad != null && load.BookedLoad.BrokerId == userId)
				.Where(load => load.Status == LoadStatus.Booked)
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
				DispatcherId = load.BookedLoad.DispatcherId,
				DriverId = load.BookedLoad.DriverId,

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
			var bookedLoads = await context.Loads
				.Include(l => l.BookedLoad)
				.Where(load => load.BookedLoad!.DispatcherId == userId)
				.Where(load => load.Status == LoadStatus.Booked)
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

		public async Task<IEnumerable<DeliveredLoadViewModel>> GetAllDeliveredLoadsForBrokerAsync(Guid userId)
		{
			var deliveredLoads = await context.Loads
				.Include(l => l.BookedLoad)
				.Include(l => l.DeliveredLoad)
				.Include(l => l.BookedLoad.Dispatcher)
				.Include(l => l.BookedLoad.Driver)
				.Include(l => l.Broker)
				.Where(load => load.DeliveredLoad != null && load.BrokerId == userId)
				.ToListAsync();


			return deliveredLoads.Select(load => new DeliveredLoadViewModel
			{
				Id = load.DeliveredLoad.Id,
				LoadLocations = $"{load.OriginCity}, {load.OriginState} - {load.DestinationCity},{load.DestinationState}",
				Distance = load.Distance,
				Price = load.Price,
				DeliveredOn = load.DeliveredLoad.DeliveredDate,
				BrokerName = load.Broker.FullName,
				DispatcherName = load.BookedLoad.Dispatcher.FullName,
				DriverName = load.BookedLoad.Driver.FullName
			});
			
		}

		public async Task<IEnumerable<DeliveredLoadViewModel>> GetAllDeliveredLoadsForDispatcherAsync(Guid userId)
		{
			var deliveredLoads = await context.Loads
				.Include(l => l.BookedLoad)
				.Include(l => l.DeliveredLoad)
				.Include(l => l.BookedLoad.Dispatcher)
				.Include(l => l.BookedLoad.Driver)
				.Include(l => l.Broker)
				.Where(load => load.DeliveredLoad != null && load.BookedLoad.DispatcherId == userId)
				.ToListAsync();

			return deliveredLoads.Select(load => new DeliveredLoadViewModel
			{
				Id = load.DeliveredLoad.Id,
				LoadLocations = $"{load.OriginCity}, {load.OriginState} - {load.DestinationCity},{load.DestinationState}",
				Distance = load.Distance,
				Price = load.Price,
				DeliveredOn = load.DeliveredLoad.DeliveredDate,
				BrokerName = load.Broker.FullName,
				DispatcherName = load.BookedLoad.Dispatcher.FullName,
				DriverName = load.BookedLoad.Driver.FullName
			});
		}

		public async Task<LoadBoardViewModel> GetBrokerLoadBoardAsync(Guid userId)
		{
			var user = await userManager.Users
				.FirstOrDefaultAsync(u => u.Id == userId);

			var createdLoads = await GetAllCreatedLoadsForBrokerAsync(userId);
			var postedLoads = await GetAllPostedLoadsForBrokerAsync(userId);
			var bookedLoads = await GetAllBookedLoadsForBrokerAsync(userId);
			var deliveredLoads = await GetAllDeliveredLoadsForBrokerAsync(userId);

			var userProfile = await profileService.GetUserInformation(userId);

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
				DeliveredLoads = deliveredLoads,
				Profile = userProfile
			};

			return loadBoardViewModel;
		}

		public async Task<LoadBoardViewModel> GetDispatcherLoadBoardAsync(Guid userId)
		{
			var user = await userManager.Users
				.FirstOrDefaultAsync(u => u.Id == userId);

			var createdLoads = new List<LoadViewModel>();
			var postedLoads = await GetAllPostedLoadsAsync(userId);
			var bookedLoads = await GetAllBookedLoadsForDispatcherAsync(userId);
			var deliveredLoads = await GetAllDeliveredLoadsForDispatcherAsync(userId);

			var userProfile = await profileService.GetUserInformation(userId);


			return new LoadBoardViewModel
			{
				UserId = userId,
				FirstName = user.FirstName,
				LastName = user.LastName,
				CompanyName = user.CompanyName,
				Position = user.Position,
				CreatedLoads = createdLoads,
				PostedLoads = postedLoads,
				BookedLoads = bookedLoads.ToList(),
				DeliveredLoads = deliveredLoads.ToList(),
				Profile = userProfile
			};
		}

		public async Task<Dictionary<LoadStatus, int>> GetLoadCountsForBrokerAsync(Guid brokerId)
		{
			var loadCounts = await context.Loads
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
			var loadCounts = await context.Loads
				.Include(l => l.BookedLoad)
				.Include(l => l.DeliveredLoad)
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
