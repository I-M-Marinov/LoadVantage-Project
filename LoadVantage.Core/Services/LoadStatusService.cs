﻿using Microsoft.EntityFrameworkCore;

using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data.Contracts;


using static LoadVantage.Common.GeneralConstants.ErrorMessages;


namespace LoadVantage.Core.Services
{
    public class LoadStatusService : ILoadStatusService
    {
	    public readonly IProfileService profileService;
	    public readonly LoadVantageDbContext context;
	    public readonly IDistanceCalculatorService distanceCalculatorService;
        public readonly ILoadHelperService loadHelperService;
        private readonly IHtmlSanitizerService htmlSanitizer;
		private readonly IGeocodeService geocodeService;
		private readonly IMapBoxService mapBoxService;


		public LoadStatusService(
			IProfileService _profileService, 
			LoadVantageDbContext _context, 
			IDistanceCalculatorService _distanceCalculatorService, 
			ILoadHelperService _loadHelperService,
			IHtmlSanitizerService _htmlSanitizer,
			IGeocodeService _geocodeService,
			IMapBoxService _mapBoxService)
	    {
		    profileService = _profileService;
            context = _context;
            distanceCalculatorService = _distanceCalculatorService;
            loadHelperService = _loadHelperService;
			htmlSanitizer = _htmlSanitizer;
			geocodeService = _geocodeService;
			mapBoxService = _mapBoxService;
	    }

	    public async Task<LoadViewModel?> GetLoadDetailsAsync(Guid loadId, Guid userId)
	    {

		    var userAllowedToView = await loadHelperService.CanUserViewLoadAsync(userId, loadId);

			if (!userAllowedToView)
		    {
			    return null;
		    }

		    var load = await context.Loads
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

		    var dispatcherInfo = loadHelperService.CreateDispatcherInfo(load.BookedLoad);
		    var driverInfo = loadHelperService.CreateDriverInfo(load.BookedLoad);
		    var userProfile = await profileService.GetUserInformation(userId);

		    var mapUrl = mapBoxService.GetStaticMapUrl(load.OriginLatitude, load.OriginLongitude, load.DestinationLatitude,load.DestinationLongitude);

			var loadViewModel = new LoadViewModel
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
			    DispatcherId = load.BookedLoad?.DispatcherId,
			    DispatcherInfo = dispatcherInfo,
			    DriverInfo = driverInfo,
			    UserProfile = userProfile
		    };

			return loadViewModel;

	    }
		public async Task<LoadViewModel> GetLoadByIdAsync(Guid loadId)
        {
            var load = await context.Loads.FindAsync(loadId);

            if (load == null)
            {
                return null;
            }

            var foundLoad = new LoadViewModel
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
                BrokerId = load.BrokerId,
                Status = load.Status.ToString()
            };

            return foundLoad;
        }
        public async Task<Guid> CreateLoadAsync(LoadViewModel model, Guid brokerId)
        {
			// sanitize the origin and destination city and state
	        var sanitizedOriginCity = htmlSanitizer.Sanitize(model.OriginCity);
	        var sanitizedOriginState = htmlSanitizer.Sanitize(model.OriginState);
	        var sanitizedDestinationCity = htmlSanitizer.Sanitize(model.DestinationCity);
	        var sanitizedDestinationState = htmlSanitizer.Sanitize(model.DestinationState);

			double calculatedDistance = 0;

			try
	        {
		        calculatedDistance =
			        await distanceCalculatorService.GetDistanceBetweenCitiesAsync(sanitizedOriginCity, sanitizedOriginState,
				        sanitizedDestinationCity, sanitizedDestinationState);
			}
	        catch (Exception e)
	        {
		        throw new Exception(e.Message);
	        }

	        var (originFormattedCity, originFormattedState) = loadHelperService.FormatLocation(sanitizedOriginCity, sanitizedOriginState);
	        var (destinationFormattedCity, destinationFormattedState) = loadHelperService.FormatLocation(sanitizedDestinationCity, sanitizedDestinationState);

	        var originCoordinates = await geocodeService.GetCoordinatesAsync(sanitizedOriginCity, sanitizedOriginState);
	        var destinationCoordinates = await geocodeService.GetCoordinatesAsync(sanitizedDestinationCity, sanitizedDestinationCity);

			var load = new Load
	        {
		        Id = Guid.NewGuid(),
		        CreatedDate = DateTime.Now,
		        OriginCity = originFormattedCity,
		        OriginState = originFormattedState,
		        DestinationCity = destinationFormattedCity,
		        DestinationState = destinationFormattedState,
				OriginLatitude = originCoordinates.Latitude, // add the origin latitude to the DB
				OriginLongitude = originCoordinates.Longitude, // add the origin longitude to the DB
				DestinationLatitude = destinationCoordinates.Latitude, // add the destination latitude to the DB
				DestinationLongitude = destinationCoordinates.Longitude, // add the destination longitude to the DB
				PickupTime = model.PickupTime,
		        DeliveryTime = model.DeliveryTime,
		        Distance = calculatedDistance,
		        Price = model.PostedPrice,
		        Weight = model.Weight,
		        BrokerId = brokerId,
		        Status = LoadStatus.Created
	        };

	        await context.Loads.AddAsync(load);
	        await context.SaveChangesAsync();

	        return load.Id;
		}
		public async Task<bool> EditLoadAsync(Guid loadId, LoadViewModel model)
		{
			var load = await context.Loads.FindAsync(loadId);

			if (load == null)
			{
				return false; // Load not found
			}


			// sanitize the origin and destination city and state if the load is found 

			var sanitizedOriginCity = htmlSanitizer.Sanitize(model.OriginCity);
			var sanitizedOriginState = htmlSanitizer.Sanitize(model.OriginState);
			var sanitizedDestinationCity = htmlSanitizer.Sanitize(model.DestinationCity);
			var sanitizedDestinationState = htmlSanitizer.Sanitize(model.DestinationState);

			var (originFormattedCity, originFormattedState) = loadHelperService.FormatLocation(sanitizedOriginCity, sanitizedOriginState);
			var (destinationFormattedCity, destinationFormattedState) = loadHelperService.FormatLocation(sanitizedDestinationCity, sanitizedDestinationState);

			// If any changes in the Origin City or State
			bool originChanged = load.OriginCity != originFormattedCity || load.OriginState != originFormattedState;
			// If any changes in the Destination City or State
			bool destinationChanged = load.DestinationCity != destinationFormattedCity || load.DestinationState != destinationFormattedState;

			bool pickupTimeChanged = load.PickupTime != model.PickupTime;
			bool deliveryTimeChanged = load.DeliveryTime != model.DeliveryTime;
			bool priceChanged = load.Price != model.PostedPrice;
			bool weightChanged = load.Weight != model.Weight;

			bool changes = originChanged || destinationChanged ||
							 pickupTimeChanged || deliveryTimeChanged ||
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
		public async Task<bool> CancelLoadAsync(Guid loadId)
		{
			var load = await context.Loads
				.Include(l => l.BookedLoad)
				.FirstOrDefaultAsync(l => l.Id == loadId);

			if (load == null)
			{
				return false;
			}

			if (load.Status == LoadStatus.Booked)
			{
				await CancelLoadBookingAsync(loadId, load.BrokerId); // Cancel the carrier first if the load is in Booked Status
			}
			// Update status to cancelled (soft delete)

			load.Status = LoadStatus.Cancelled;
			context.Loads.Update(load);
			await context.SaveChangesAsync();

			return true;
		}
		public async Task<bool> PostLoadAsync(Guid loadId)
        {
            var load = await context.Loads
                .Include(l => l.PostedLoad)
                .FirstOrDefaultAsync(l => l.Id == loadId);

            if (load == null || load.Status != LoadStatus.Created)
            {
                return false; // Only allow posting if the load is in "Created" status
            }

            // Update status and add PostedLoad record
            load.Status = LoadStatus.Available;
            load.PostedLoad = new PostedLoad
            {
                LoadId = load.Id,
                PostedDate = DateTime.Now
            };

            context.Loads.Update(load);
            await context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UnpostLoadAsync(Guid loadId)
        {
            var load = await context.Loads
                .Include(l => l.PostedLoad)
                .FirstOrDefaultAsync(l => l.Id == loadId);

            if (load == null || load.Status != LoadStatus.Available)
            {
                return false; // Only allow unposting if the load is in "Available" status
            }

            // Update status to Created
            load.Status = LoadStatus.Created;
            

            if (load.PostedLoad != null)
            {
				context.PostedLoads.Remove(load.PostedLoad); // Do not need to remove the posted date and time ( because the load is deleted from the table ) 
				load.PostedLoad = null; // Clear the navigation property in the Loads table
			}

            await context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UnpostAllLoadsAsync(Guid brokerId)
        {
			var allPostedLoads = await context.Loads
				.Include(l => l.PostedLoad)
				.Where(load => load.Status == LoadStatus.Available && load.BrokerId == brokerId)
				.ToListAsync();

			if (allPostedLoads.Count == 0)
	        {
		        return false; // no loads posted for this broker, so cannot unpost them
	        }


	        foreach (var load in allPostedLoads)
	        {
				load.Status = LoadStatus.Created; // Update status to Created

				context.PostedLoads.Remove(load.PostedLoad!);
				load.PostedLoad = null; // Clear the navigation property in the Loads table
			}


	        await context.SaveChangesAsync();

	        return true;
        }
        public async Task<bool> BookLoadAsync(Guid loadId, Guid dispatcherId)
        {
            var load = await context.Loads
                .Include(l => l.PostedLoad)
                .Include(l => l.BookedLoad)
                .FirstOrDefaultAsync(l => l.Id == loadId);

            if (load == null || load.Status != LoadStatus.Available)
            {
                return false; // Only allow booking if the load is "Available" or it exists in the DB
            }

            // Update status and add BookedLoad record
            load.Status = LoadStatus.Booked;
            load.BookedLoad = new BookedLoad
            {
                Id = Guid.NewGuid(),
                LoadId = load.Id,
                DispatcherId = dispatcherId,
                BrokerId = load.BrokerId,
                BookedDate = DateTime.UtcNow.ToLocalTime(),
                DriverId = null // assign a driver later
            };

            await context.BookedLoads.AddAsync(load.BookedLoad);
            await context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> CancelLoadBookingAsync(Guid loadId, Guid? userId)
        {
	        var load = await context.Loads
		        .Include(l => l.BookedLoad)
		        .ThenInclude(bl => bl!.Driver)
		        .FirstOrDefaultAsync(l => l.Id == loadId);

	        if (load.BrokerId != userId)
	        {
		        throw new UnauthorizedAccessException(NoPermissionToCancel);
	        }

	        if (load.BookedLoad == null || load.Status != LoadStatus.Booked)
	        {
		        throw new InvalidOperationException(LoadNotInBookedStatus);
	        }

	        if (load.BookedLoad.DriverId != null)
	        {
		        load.BookedLoad.Driver.IsBusy = false; // Put the driver's IsBusy status to false, so he shows up available for the dispatcher again. 
	        }

	        context.BookedLoads.Remove(load.BookedLoad); // Remove the BookedLoad entity 
			load.Status = LoadStatus.Available; // Set status back to Available (Posted) to repost the truck

			var result = await context.SaveChangesAsync();

	        return result > 0; // Return true if there were affected rows returned from the DB
        }
        public async Task<bool> ReturnLoadBackToBroker(Guid loadId, Guid? userId)
        {
	        var load = await context.Loads
		        .Include(l => l.BookedLoad)
		        .ThenInclude(bl => bl.Driver)
		        .FirstOrDefaultAsync(l => l.Id == loadId);

	        if (load == null)
	        {
		        throw new Exception(LoadNotFound);
	        }

	        if (load.BookedLoad?.DispatcherId != userId)
	        {
		        throw new UnauthorizedAccessException(NoPermissionToCancel);
	        }

	        if (load.BookedLoad == null || load.Status != LoadStatus.Booked)
	        {
		        throw new InvalidOperationException(LoadNotInBookedStatus);
	        }

	        if (load.BookedLoad.DriverId != null)
	        {
		        load.BookedLoad.Driver.IsBusy = false; // Put the driver's IsBusy status to false, so he shows up available for the dispatcher again. 
	        }

			context.BookedLoads.Remove(load.BookedLoad); // Remove the BookedLoad entity 
			load.Status = LoadStatus.Available; // Set status back to Available (Posted) to repost the truck

			var result = await context.SaveChangesAsync();

			return result > 0; // Return true if there were affected rows returned from the DB
		}
        public async Task<bool> LoadDeliveredAsync(Guid loadId)
        {
	        var load = await context.Loads
		        .Include(l => l.BookedLoad)
		        .FirstOrDefaultAsync(l => l.Id == loadId);

	        if (load == null || load.Status != LoadStatus.Booked || load.BookedLoad == null)
	        {
		        return false; 
	        }

	        if (!load.BookedLoad.DriverId.HasValue)
	        {
		        throw new ArgumentException(CannotMarkLoadDeliveredWithoutADriver);
	        }

	        var driver = await context.Drivers
		        .Where(d => d.DriverId == load.BookedLoad.DriverId)
		        .SingleOrDefaultAsync();


			load.Status = LoadStatus.Delivered; // load is marked delivered
			driver!.IsBusy = false; // driver is freed up to get another load

	        var deliveredLoad = new DeliveredLoad
	        {
		        LoadId = load.Id,
		        DriverId = load.BookedLoad.DriverId.Value,
		        DispatcherId = load.BookedLoad.DispatcherId,
		        BrokerId = load.BrokerId,
		        DeliveredDate = DateTime.UtcNow.ToLocalTime(),
		        BookedLoadId = load.BookedLoad.Id,
	        };

	        await context.DeliveredLoads.AddAsync(deliveredLoad);
	        await context.SaveChangesAsync();
	        
	        return true;
        }
        
	}

}  
