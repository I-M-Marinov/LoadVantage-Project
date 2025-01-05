
using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.Load;
using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;

using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Contracts;
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
		private IHtmlSanitizerService htmlSanitizer;
		private readonly IGeocodeService geocodeService;
		private readonly IMapBoxService mapBoxService;



		public AdminLoadStatusService(
			LoadVantageDbContext _context, 
			IAdminProfileService _adminProfileService, 
			ILoadHelperService _loadHelperService, 
			IDistanceCalculatorService _distanceCalculatorServiceService,
			IHtmlSanitizerService _htmlSanitizer,
			IGeocodeService _geocodeService,
			IMapBoxService _mapBoxService)
		{
			context = _context;
			adminProfileService = _adminProfileService;
			loadHelperService = _loadHelperService;
			distanceCalculatorService = _distanceCalculatorServiceService;
			htmlSanitizer = _htmlSanitizer;
			geocodeService = _geocodeService;
			mapBoxService = _mapBoxService;
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
			// sanitize the origin and destination city and state
			var sanitizedOriginCity = htmlSanitizer.Sanitize(model.OriginCity);
			var sanitizedOriginState = htmlSanitizer.Sanitize(model.OriginState);
			var sanitizedDestinationCity = htmlSanitizer.Sanitize(model.DestinationCity);
			var sanitizedDestinationState = htmlSanitizer.Sanitize(model.DestinationState);

			var load = await context.Loads.FindAsync(loadId);

			if (load == null)
			{
				return false; // Load not found
			}

			var (originFormattedCity, originFormattedState) = loadHelperService.FormatLocation(sanitizedOriginCity, sanitizedOriginState);
			var (destinationFormattedCity, destinationFormattedState) = loadHelperService.FormatLocation(sanitizedDestinationCity, sanitizedDestinationState);

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
						originFormattedCity,
						originFormattedState,
						destinationFormattedCity,
						destinationFormattedState
					);

					load.Distance = distance;

					context.Loads.Update(load);
					await context.SaveChangesAsync();
				}
				catch (Exception e)
				{
					throw new Exception(ErrorUpdatingThisLoad);
				}
			}

			// update the coordinates for the origin and destination if any changes

			if (originChanged)
			{
				var originCoordinates = await geocodeService.GetCoordinatesAsync(originFormattedCity, originFormattedState); // get the updated coordinates for the origin

				load.OriginLatitude = originCoordinates.Latitude;
				load.OriginLongitude = originCoordinates.Longitude;
			}
			else if (destinationChanged)
			{
				var destinationCoordinates = await geocodeService.GetCoordinatesAsync(destinationFormattedCity, destinationFormattedState); // get the updated coordinates for the destination

				load.DestinationLatitude = destinationCoordinates.Latitude;
				load.DestinationLongitude = destinationCoordinates.Longitude;
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

			var mapUrl = mapBoxService.GetStaticMapUrl(load.OriginLatitude, load.OriginLongitude, load.DestinationLatitude, load.DestinationLongitude);

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
				MapUrl = mapUrl,
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
