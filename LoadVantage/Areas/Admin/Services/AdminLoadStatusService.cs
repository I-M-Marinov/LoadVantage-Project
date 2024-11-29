using System.Globalization;
using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.Load;
using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Services;
using LoadVantage.Extensions;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data.Services;
using Microsoft.EntityFrameworkCore;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Areas.Admin.Services
{
	public class AdminLoadStatusService : IAdminLoadStatusService
	{
		private readonly LoadVantageDbContext context;
		private readonly IAdminProfileService adminProfileService;
		private readonly ILoadHelperService loadHelperService;
		private IDistanceCalculatorService distanceCalculatorService;



		public AdminLoadStatusService(LoadVantageDbContext _context, IAdminProfileService _adminProfileService, ILoadHelperService _loadHelperService, IDistanceCalculatorService _distanceCalculatorServiceService)
		{
			context = _context;
			adminProfileService = _adminProfileService;
			loadHelperService = _loadHelperService;
			distanceCalculatorService = _distanceCalculatorServiceService;
		}

		public async Task<bool> RestoreLoadAsync(Guid loadId)
		{
			var load = await context.Loads.FindAsync(loadId);

			if (load == null)
			{
				return false; // Load not found
			}

			load.Status = LoadStatus.Created;
			context.Loads.Update(load);
			await context.SaveChangesAsync();

			return true;
		}
		public async Task<bool> EditLoadAsync(Guid loadId, AdminLoadViewModel model)
		{
			var load = await context.Loads.FindAsync(loadId);

			if (load == null)
			{
				return false; // Load not found
			}

			var (originFormattedCity, originFormattedState) = loadHelperService.FormatLocation(model.OriginCity, model.OriginState);
			var (destinationFormattedCity, destinationFormattedState) = loadHelperService.FormatLocation(model.DestinationCity, model.DestinationState);

			// If any changes in the Origin City or State
			bool originChanged = load.OriginCity != originFormattedCity || load.OriginState != originFormattedState;
			// If any changes in the Destination City or State
			bool destinationChanged = load.DestinationCity != destinationFormattedCity || load.DestinationState != destinationFormattedState;

			bool pickupOrDeliveryTimesChanged = load.PickupTime != model.PickupTime || load.DeliveryTime != model.DeliveryTime;
			bool priceChanged = load.Price != model.PostedPrice;
			bool weightChanged = load.Weight != model.Weight;

			bool changes = originChanged || destinationChanged ||
			               pickupOrDeliveryTimesChanged ||
							 priceChanged || weightChanged;

			if (!changes)
			{
				return false;
			}

			// Update properties
			load.Id = loadId;
			load.OriginCity = originFormattedCity;
			load.OriginState = originFormattedState;
			load.DestinationCity = destinationFormattedCity;
			load.DestinationState = destinationFormattedState;
			load.PickupTime = model.PickupTime;
			load.DeliveryTime = model.DeliveryTime;
			load.Price = model.PostedPrice;
			load.Weight = model.Weight;

			if (originChanged || destinationChanged) // if either of the two is TRUE ----> Recalculate the distance 
			{
				try
				{
					var distance = await distanceCalculatorService.GetDistanceBetweenCitiesAsync(
						model.OriginCity,
						model.OriginState,
						model.DestinationCity,
						model.DestinationState
					);

					load.Distance = distance;

					context.Loads.Update(load);
					await context.SaveChangesAsync();

					return true;
				}
				catch (Exception e)
				{
					throw new Exception(ErrorUpdatingThisLoad);
				}
			}

			context.Loads.Update(load);
			await context.SaveChangesAsync();

			return true;
		}
		public async Task<AdminLoadViewModel?> GetLoadInformation(Guid loadId, Guid userId)
		{
			var load = await context.Loads
				.Include(l => l.Broker) 
				.Include(l => l.BookedLoad)
				.ThenInclude(bl => bl!.Dispatcher)
				.Include(l => l.BookedLoad!.Driver)
				.ThenInclude(d => d!.Truck)
				.Include(l => l.DeliveredLoad)
				.FirstOrDefaultAsync(l => l.Id == loadId);

			if (load == null)
			{
				throw new Exception(LoadCouldNotBeRetrieved);
			}

			var brokerInfo = loadHelperService.CreateBrokerInfo(load);
			var dispatcherInfo = loadHelperService.CreateDispatcherInfo(load.BookedLoad);
			var driverInfo = loadHelperService.CreateDriverInfo(load.BookedLoad);
			var adminProfile = await adminProfileService.GetAdminInformation(userId);



			var loadViewModel = new AdminLoadViewModel
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
				DeliveredDate = load.DeliveredLoad?.DeliveredDate,
				DispatcherId = load.BookedLoad?.DispatcherId,
				DispatcherInfo = dispatcherInfo,
				BrokerInfo = brokerInfo,
				DriverInfo = driverInfo,
				AdminProfile = adminProfile
			};

			return loadViewModel;

		}

	}
}
