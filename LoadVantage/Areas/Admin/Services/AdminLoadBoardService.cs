using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models;
using LoadVantage.Areas.Admin.Models.Load;
using LoadVantage.Areas.Admin.Models.LoadBoard;
using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.LoadBoard;
using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace LoadVantage.Areas.Admin.Services
{
	public class AdminLoadBoardService : IAdminLoadBoardService
	{
		private readonly UserManager<BaseUser> userManager;
		private readonly IUserService userService;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly LoadVantageDbContext context;
		private readonly IImageService imageService;
		private readonly IAdminProfileService adminProfileService;

		public AdminLoadBoardService(
			UserManager<BaseUser> _userManager,
			IUserService _userService,
			IHttpContextAccessor _httpContextAccessor,
			LoadVantageDbContext _context,
			IImageService _imageService,
			IAdminProfileService _adminProfileService)
		{
			userManager = _userManager;
			userService = _userService;
			httpContextAccessor = _httpContextAccessor;
			context = _context;
			imageService = _imageService;
			adminProfileService = _adminProfileService;
		}

		private async Task<IEnumerable<Load>> GetAllLoads(Guid userId)
		{
			var allLoads = await context.Loads
				.Include(l => l.Broker)
				.Include(l => l.PostedLoad)
				.Include(l => l.BookedLoad)
				.ThenInclude(bl => bl.Driver)
				.Include(l => l.BookedLoad)
				.ThenInclude(bl => bl.Dispatcher)
				.ThenInclude(d => d.Trucks)
				.ThenInclude(t => t.Driver) 
				.Include(l => l.DeliveredLoad)
				.ToListAsync();


			return allLoads;
		}

		public async Task<AdminLoadBoardViewModel> GetLoadBoardManager(Guid userId)
		{
			var allLoads = await GetAllLoads(userId);

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
							DriverTruckNumber = load.BookedLoad.Driver.Truck?.TruckNumber,
							DriverLicenseNumber = load.BookedLoad.Driver.LicenseNumber
						}
						: null

				})
				.ToList();

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
							DriverTruckNumber = load.BookedLoad.Driver.Truck?.TruckNumber,
							DriverLicenseNumber = load.BookedLoad.Driver.LicenseNumber
						}
						: null
				})
				.ToList();

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
					DispatcherId = load.BookedLoad.DispatcherId,
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
							DriverTruckNumber = load.BookedLoad.Driver.Truck?.TruckNumber,
							DriverLicenseNumber = load.BookedLoad.Driver.LicenseNumber
						}
						: null
				})
				.ToList();

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
					DispatcherId = load.BookedLoad.DispatcherId,
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
							DriverTruckNumber = load.BookedLoad.Driver.Truck?.TruckNumber,
							DriverLicenseNumber = load.BookedLoad.Driver.LicenseNumber
						}
						: null
				})
				.ToList();



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
							DriverTruckNumber = load.BookedLoad.Driver.Truck?.TruckNumber,
							DriverLicenseNumber = load.BookedLoad.Driver.LicenseNumber
						}
						: null
				})
				.ToList();

			var userProfile = await adminProfileService.GetAdminInformation(userId);

			var loadBoardViewModel = new AdminLoadBoardViewModel()
			{
				UserId = userId,
				CreatedLoads = createdLoads,
				PostedLoads = postedLoads,
				BookedLoads = bookedLoads,
				DeliveredLoads = deliveredLoads,
				CancelledLoads = cancelledLoads,
				Profile = userProfile
			};

			return loadBoardViewModel;
		}
	}
}
