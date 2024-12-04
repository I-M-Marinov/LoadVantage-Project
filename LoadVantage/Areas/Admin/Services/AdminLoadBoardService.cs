using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.Load;
using LoadVantage.Areas.Admin.Models.LoadBoard;
using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Infrastructure.Data.Models;



namespace LoadVantage.Areas.Admin.Services
{
	public class AdminLoadBoardService : IAdminLoadBoardService
	{
		private readonly IAdminProfileService adminProfileService;
		private readonly ILoadHelperService loadHelperService;

		public AdminLoadBoardService(
			IAdminProfileService _adminProfileService, 
			ILoadHelperService _loadHelperService)
		{
			adminProfileService = _adminProfileService;
			loadHelperService = _loadHelperService;
		}

		public async Task<AdminLoadBoardViewModel> GetLoadBoardManager(Guid userId)
		{
			var allLoads = await loadHelperService.GetAllLoads();

			var createdLoads = GetCreatedLoads(allLoads);
			var postedLoads = GetPostedLoads(allLoads);
			var bookedLoads = GetBookedLoads(allLoads);
			var deliveredLoads = GetDeliveredLoads(allLoads);
			var cancelledLoads = GetCancelledLoads(allLoads);
			var userProfile = await adminProfileService.GetAdminInformation(userId);

			var loadBoardViewModel = new AdminLoadBoardViewModel()
			{
				UserId = userId,
				CreatedLoads = createdLoads,
				PostedLoads = postedLoads,
				BookedLoads = bookedLoads,
				DeliveredLoads = deliveredLoads,
				CancelledLoads = cancelledLoads,
				Profile = userProfile!
			};

			return loadBoardViewModel;
		}

		public async Task<IEnumerable<AdminLoadViewModel>> GetAllPostedLoadsAsync(Guid userId)
		{
			var postedLoads = await loadHelperService.GetAllLoads();
			postedLoads = postedLoads.Where(l => l.Status == LoadStatus.Available);

			var postedLoadsModels = postedLoads
				.Select(load => new AdminLoadViewModel()
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
					BrokerInfo = load.BrokerId != Guid.Empty
						? new BrokerInfoViewModel()
						{
							BrokerName = load.Broker.FullName,
							BrokerEmail = load.Broker.Email,
							BrokerPhone = load.Broker.PhoneNumber,
						}
						: null,
					// Include Dispatcher Info if available
					DispatcherInfo = load.BookedLoad?.DispatcherId != null
						? new DispatcherInfoViewModel
						{
							DispatcherName = load.BookedLoad.Dispatcher.FullName,
							DispatcherEmail = load.BookedLoad.Dispatcher.Email,
							DispatcherPhone = load.BookedLoad.Dispatcher.PhoneNumber,
						}
						: null,

					// Include Driver Info if available
					DriverInfo = load.BookedLoad?.DriverId != null
						? new DriverInfoViewModel
						{
							DriverName = load.BookedLoad.Driver?.FullName,
							DriverLicenseNumber = load.BookedLoad?.Driver?.LicenseNumber
						}
						: null
				})
				.ToList();

			return postedLoadsModels;
		}

		private List<AdminLoadViewModel> GetCreatedLoads(IEnumerable<Load> allLoads)
		{
			var createdLoads = allLoads
				.Where(load => load.Status == LoadStatus.Created)
				.OrderBy(l => l.PickupTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.Select(load => new AdminLoadViewModel()
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
					Broker = load.Broker,
					BrokerInfo = load.BrokerId != Guid.Empty
						? new BrokerInfoViewModel()
						{
							BrokerName = load.Broker.FullName,
							BrokerEmail = load.Broker.Email,
							BrokerPhone = load.Broker.PhoneNumber
						}
						: null,
					// Include Dispatcher Info if available
					DispatcherInfo = load.BookedLoad?.DispatcherId != null
						? new DispatcherInfoViewModel
						{
							DispatcherName = load.BookedLoad.Dispatcher.FullName,
							DispatcherEmail = load.BookedLoad.Dispatcher.Email,
							DispatcherPhone = load.BookedLoad.Dispatcher.PhoneNumber
						}
						: null,

					// Include Driver Info if available
					DriverInfo = load.BookedLoad?.DriverId != null
						? new DriverInfoViewModel
						{
							DriverName = load.BookedLoad.Driver?.FullName,
							DriverLicenseNumber = load.BookedLoad.Driver.LicenseNumber
						}
						: null

				})
				.ToList();

			return createdLoads;
		}

		private List<AdminLoadViewModel> GetPostedLoads(IEnumerable<Load> allLoads)
		{
			var postedLoads = allLoads
				.Where(load => load.Status == LoadStatus.Available)
				.OrderBy(l => l.PickupTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.Select(load => new AdminLoadViewModel()
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
					Broker = load.Broker,
					BrokerInfo = load.BrokerId != Guid.Empty
						? new BrokerInfoViewModel()
						{
							BrokerName = load.Broker.FullName,
							BrokerEmail = load.Broker.Email,
							BrokerPhone = load.Broker.PhoneNumber
						}
						: null,
					// Include Dispatcher Info if available
					DispatcherInfo = load.BookedLoad?.DispatcherId != null
						? new DispatcherInfoViewModel
						{
							DispatcherName = load.BookedLoad.Dispatcher.FullName,
							DispatcherEmail = load.BookedLoad.Dispatcher.Email,
							DispatcherPhone = load.BookedLoad.Dispatcher.PhoneNumber
						}
						: null,

					// Include Driver Info if available
					DriverInfo = load.BookedLoad?.DriverId != null
						? new DriverInfoViewModel
						{
							DriverName = load.BookedLoad.Driver?.FullName,
							DriverLicenseNumber = load.BookedLoad.Driver.LicenseNumber
						}
						: null
				})
				.ToList();

			return postedLoads;
		}

		private List<AdminLoadViewModel> GetBookedLoads(IEnumerable<Load> allLoads)
		{
			var bookedLoads = allLoads
				.Where(load => load.Status == LoadStatus.Booked)
				.OrderBy(l => l.PickupTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.Select(load => new AdminLoadViewModel()
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
					Broker = load.Broker,
					DispatcherId = load.BookedLoad.DispatcherId,
					Dispatcher = load.BookedLoad.Dispatcher,
					DriverId = load.BookedLoad.DriverId,
					BrokerInfo = load.BrokerId != Guid.Empty
						? new BrokerInfoViewModel()
						{
							BrokerName = load.Broker.FullName,
							BrokerEmail = load.Broker.Email,
							BrokerPhone = load.Broker.PhoneNumber
						}
						: null,
					// Include Dispatcher Info if available
					DispatcherInfo = load.BookedLoad?.DispatcherId != null
						? new DispatcherInfoViewModel
						{
							DispatcherName = load.BookedLoad.Dispatcher.FullName,
							DispatcherEmail = load.BookedLoad.Dispatcher.Email,
							DispatcherPhone = load.BookedLoad.Dispatcher.PhoneNumber
						}
						: null,

					// Include Driver Info if available
					DriverInfo = load.BookedLoad?.DriverId != null
						? new DriverInfoViewModel
						{
							DriverName = load.BookedLoad.Driver?.FullName,
							DriverLicenseNumber = load.BookedLoad.Driver.LicenseNumber
						}
						: null
				})
				.ToList();

			return bookedLoads;
		}

		private List<AdminLoadViewModel> GetDeliveredLoads(IEnumerable<Load> allLoads)
		{
			var deliveredLoads = allLoads

				.Where(load => load.Status == LoadStatus.Delivered)
				.OrderBy(l => l.DeliveryTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.Select(load => new AdminLoadViewModel
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
					DeliveredDate = load.DeliveredLoad.DeliveredDate,
					BrokerId = load.BrokerId,
					Broker = load.Broker,
					DispatcherId = load.BookedLoad.DispatcherId,
					Dispatcher = load.BookedLoad.Dispatcher,
					DriverId = load.BookedLoad.DriverId,
					BrokerInfo = load.BrokerId != Guid.Empty
						? new BrokerInfoViewModel()
						{
							BrokerName = load.Broker.FullName,
							BrokerEmail = load.Broker.Email,
							BrokerPhone = load.Broker.PhoneNumber
						}
						: null,
					// Include Dispatcher Info if available
					DispatcherInfo = load.BookedLoad?.DispatcherId != null
						? new DispatcherInfoViewModel
						{
							DispatcherName = load.BookedLoad.Dispatcher.FullName,
							DispatcherEmail = load.BookedLoad.Dispatcher.Email,
							DispatcherPhone = load.BookedLoad.Dispatcher.PhoneNumber
						}
						: null,

					// Include Driver Info if available
					DriverInfo = load.BookedLoad?.DriverId != null
						? new DriverInfoViewModel
						{
							DriverName = load.BookedLoad.Driver?.FullName,
							DriverLicenseNumber = load.BookedLoad.Driver.LicenseNumber
						}
						: null
				})
				.ToList();

			return deliveredLoads;
		}

		private List<AdminLoadViewModel> GetCancelledLoads(IEnumerable<Load> allLoads)
		{
			var cancelledLoads = allLoads
				.Where(load => load.Status == LoadStatus.Cancelled)
				.OrderBy(l => l.PickupTime)
				.ThenByDescending(l => l.OriginCity)
				.ThenByDescending(l => l.OriginState)
				.Select(load => new AdminLoadViewModel()
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
					Broker = load.Broker,
					BrokerInfo = load.BrokerId != Guid.Empty
						? new BrokerInfoViewModel()
						{
							BrokerName = load.Broker.FullName,
							BrokerEmail = load.Broker.Email,
							BrokerPhone = load.Broker.PhoneNumber
						}
						: null
				})
				.ToList();

			return cancelledLoads;
		}
 	}
}
