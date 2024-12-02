using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.LoadBoard;
using LoadVantage.Common.Enums;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Core.Services
{
	[Authorize]
	public class LoadBoardService : ILoadBoardService
	{
		private readonly LoadVantageDbContext context;
		public readonly IProfileService profileService;
		public readonly ILoadHelperService loadHelperService;



		public LoadBoardService(LoadVantageDbContext _context, IProfileService _profileService, ILoadHelperService _loadHelperService)
		{
			context = _context;
			profileService = _profileService;
            loadHelperService = _loadHelperService;

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

		public async Task<LoadBoardViewModel> GetBrokerLoadBoardAsync(Guid userId)
		{
			var allLoads = await loadHelperService.GetAllLoads();

			var createdLoads = allLoads
				.Where(load => load.Status == LoadStatus.Created)
				.Where(load => load.BrokerId == userId)
				.OrderBy(l => l.PickupTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.Select(load => new LoadViewModel()
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
				})
				.ToList();

			var postedLoads = allLoads
				.Where(load => load.Status == LoadStatus.Available)
				.Where(load => load.BrokerId == userId)
				.OrderBy(l => l.PickupTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.Select(load => new LoadViewModel()
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
				})
				.ToList();

			var bookedLoads = allLoads
				.Where(load => load.Status == LoadStatus.Booked)
				.Where(load => load.BrokerId == userId)
				.OrderBy(l => l.PickupTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.Select(load => new LoadViewModel()
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
							DriverLicenseNumber = load.BookedLoad.Driver.LicenseNumber
						}
						: null
				})
				.ToList();

            

			var deliveredLoads = allLoads
				.Where(load => load.Status == LoadStatus.Delivered)
				.Where(load => load.BrokerId == userId)
				.OrderBy(l => l.DeliveryTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.Select(load => new DeliveredLoadViewModel
				{
					Id = load.DeliveredLoad.Id,
					LoadLocations = $"{load.OriginCity}, {load.OriginState} to {load.DestinationCity}, {load.DestinationState}",
					Distance = load.Distance,
					Price = load.Price,
					DeliveredOn = load.DeliveredLoad.DeliveredDate,
					BrokerName = load.Broker.FullName,
					DispatcherName = load.BookedLoad.Dispatcher.FullName,
					DriverName = load.BookedLoad.Driver.FullName
				})
				.ToList();

			var userProfile = await profileService.GetUserInformation(userId);

			var loadBoardViewModel = new LoadBoardViewModel
			{
				UserId = userId,
				CreatedLoads = createdLoads,
				PostedLoads = postedLoads,
				BookedLoads = bookedLoads,
				DeliveredLoads = deliveredLoads,
				Profile = userProfile
			};

			return loadBoardViewModel;
		}

		public async Task<LoadBoardViewModel> GetDispatcherLoadBoardAsync(Guid userId)
		{
			
			var allLoads = await loadHelperService.GetAllLoads();

			var createdLoads = new List<LoadViewModel>();

			var postedLoads = allLoads
				.Where(load => load.Status == LoadStatus.Available)
				.OrderBy(l => l.PickupTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.Select(load => new LoadViewModel()
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
				})
				.ToList();



			var bookedLoads = allLoads
				.Where(load => load.Status == LoadStatus.Booked)
				.Where(load => load.BookedLoad!.DispatcherId == userId)
				.OrderBy(l => l.PickupTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.Select(load => new LoadViewModel()
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
					DispatcherId = load.BookedLoad!.DispatcherId,
					DriverId = load.BookedLoad.DriverId
				})
				.ToList();

			var deliveredLoads = allLoads
				.Where(load => load.Status == LoadStatus.Delivered)
				.Where(load => load.BookedLoad!.DispatcherId == userId)
				.OrderBy(l => l.DeliveryTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.Select(load => new DeliveredLoadViewModel
				{
					Id = load.DeliveredLoad!.Id,
					LoadLocations = $"{load.OriginCity}, {load.OriginState} to {load.DestinationCity}, {load.DestinationState}",
					Distance = load.Distance,
					Price = load.Price,
					DeliveredOn = load.DeliveredLoad.DeliveredDate,
					BrokerName = load.Broker.FullName,
					DispatcherName = load.BookedLoad!.Dispatcher.FullName,
					DriverName = load.BookedLoad!.Driver!.FullName
				})
				.ToList();


			var userProfile = await profileService.GetUserInformation(userId);


			return new LoadBoardViewModel
			{
				UserId = userId,
				CreatedLoads = createdLoads,
				PostedLoads = postedLoads,
				BookedLoads = bookedLoads,
				DeliveredLoads = deliveredLoads,
				Profile = userProfile!
			};
		}

		private async Task<Dictionary<LoadStatus, int>> GetLoadCountsForBrokerAsync(Guid brokerId)
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

		private async Task<Dictionary<LoadStatus, int>> GetLoadCountsForDispatcherAsync(Guid dispatcherId)
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

		public async Task<Dictionary<string, Dictionary<LoadStatus, int>>> GetLoadCountsForUserAsync(Guid userId, string userPosition)
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
			
			throw new ArgumentException(InvalidUserType);
			
		}
	}
}
