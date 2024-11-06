using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data.Contracts;

using System.Globalization;
using Microsoft.Extensions.Logging;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;


namespace LoadVantage.Core.Services
{
    public class LoadStatusService(LoadVantageDbContext context, IDistanceCalculatorService distanceCalculatorService,UserManager<User> userManager, ILogger<LoadStatusService> logger) : ILoadStatusService
    {
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
            double calculatedDistance = 0;

            if (model != null)
            {
                try
                {
                    calculatedDistance = await distanceCalculatorService.GetDistanceBetweenCitiesAsync(model.OriginCity, model.OriginState, model.DestinationCity, model.DestinationState);
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"Error calculating distance!");
                    calculatedDistance = -1;
                    throw;
                }
            }

            var (originFormattedCity, originFormattedState) = FormatLocation(model.OriginCity, model.OriginState);
            var (destinationFormattedCity, destinationFormattedState) = FormatLocation(model.DestinationCity, model.DestinationState);


            var load = new Load
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now, 
                OriginCity = originFormattedCity,
                OriginState = originFormattedState,
                DestinationCity = destinationFormattedCity,
                DestinationState = destinationFormattedState,
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
                context.PostedLoads.Remove(load.PostedLoad);
                load.PostedLoad = null; // Clear the navigation property
            }

            await context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> EditLoadAsync(Guid loadId, LoadViewModel model)
        {
            var load = await context.Loads.FindAsync(loadId);

            if (load == null)
            {
                return false; // Load not found
            }

            var (originFormattedCity, originFormattedState) = FormatLocation(model.OriginCity, model.OriginState);
            var (destinationFormattedCity, destinationFormattedState) = FormatLocation(model.DestinationCity, model.DestinationState);

            // If any changes in the Origin City or State
            bool originChanged = load.OriginCity != originFormattedCity || load.OriginState != originFormattedState;
            // If any changes in the Destination City or State
            bool destinationChanged = load.DestinationCity != destinationFormattedCity || load.DestinationState != destinationFormattedState; 

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
                    logger.LogError(ErrorEditingLoad);
                    throw;
                }
            }

            return false;
        }
        public async Task<bool> BookLoadAsync(Guid loadId, Guid dispatcherId)
        {
            var load = await context.Loads
                .Include(l => l.PostedLoad)
                .Include(l => l.BookedLoad)
                .FirstOrDefaultAsync(l => l.Id == loadId);

            if (load == null || load.Status != LoadStatus.Available)
            {
                return false; // Only allow booking if the load is "Available"
            }

            // Update status and add BookedLoad record
            load.Status = LoadStatus.Booked;
            load.BookedLoad = new BookedLoad
            {
                Id = Guid.NewGuid(),
                LoadId = load.Id,
                DispatcherId = dispatcherId,
                BrokerId = load.BrokerId,
                BookedDate = DateTime.UtcNow,
                DriverId = null // assign a driver later
            };

            context.Loads.Update(load);
            await context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> LoadDeliveredAsync(Guid loadId)
        {
            var load = await context.Loads
                .Include(l => l.BookedLoad)
                .Include(l => l.BilledLoad)
                .FirstOrDefaultAsync(l => l.Id == loadId);

            if (load == null || load.Status != LoadStatus.Booked)
            {
                return false; // Only allow marking as delivered if the load is "Booked"
            }

            // Update status and add BilledLoad record
            load.Status = LoadStatus.Delivered;
            load.BilledLoad = new BilledLoad
            {
                LoadId = load.Id,
                BilledAmount = load.Price,
                BilledDate = DateTime.Now
            };

            context.Loads.Update(load);
            await context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> CancelLoadAsync(Guid loadId)
        {
            var load = await context.Loads.FindAsync(loadId);

            if (load == null)
            {
                return false;
            }

            // Update status to cancelled (soft delete)

            load.Status = LoadStatus.Cancelled;
            context.Loads.Update(load);
            await context.SaveChangesAsync();

            return true;
        }
        public async Task<LoadViewModel> SeeLoadDetails(Guid loadId)
        {
            var load = await context.Loads
                .FirstOrDefaultAsync(l => l.Id == loadId);

            if (load == null)
            { 
                throw new Exception(ErrorEditingLoad);
            }

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
                Status = load.Status.ToString(),
                BrokerId = load.BrokerId,
                DispatcherId = null
            };

            return loadViewModel;

        }
        private (string FormattedCity, string FormattedState) FormatLocation(string city, string state)
        {
            string formattedCity = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(city.Trim().ToLower());
            string formattedState = state.Trim().ToUpper();

            return (formattedCity, formattedState);
        }
    }

}  
