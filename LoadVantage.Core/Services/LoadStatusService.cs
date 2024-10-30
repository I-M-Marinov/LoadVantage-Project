using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using LoadVantage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LoadVantage.Core.Models.Load;

namespace LoadVantage.Core.Services
{
    public class LoadStatusService(LoadVantageDbContext context, IDistanceCalculatorService distanceCalculatorService,UserManager<User> userManager) : ILoadStatusService
    {
        public async Task<Guid> CreateLoadAsync(LoadViewModel loadViewModel, Guid brokerId)
        {
            var calculatedDistance = await distanceCalculatorService.GetDistanceBetweenCitiesAsync(loadViewModel.OriginCity, loadViewModel.OriginState, loadViewModel.DestinationCity, loadViewModel.DestinationState);

            var load = new Load
            {
                Id = Guid.NewGuid(),
                OriginCity = loadViewModel.OriginCity,
                OriginState = loadViewModel.OriginState,
                DestinationCity = loadViewModel.DestinationCity,
                DestinationState = loadViewModel.DestinationState,
                PickupTime = loadViewModel.PickupTime,
                DeliveryTime = loadViewModel.DeliveryTime,
                Distance = calculatedDistance,
                Price = loadViewModel.PostedPrice,
                Weight = loadViewModel.Weight,
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

        public async Task<bool> EditLoadAsync(Guid loadId, LoadViewModel updatedLoadViewModel)
        {
            var load = await context.Loads.FindAsync(loadId);
            if (load == null)
            {
                return false; // Load not found
            }
            // If any changes in the Origin City or State
            bool originChanged = load.OriginCity != updatedLoadViewModel.OriginCity || load.OriginState != updatedLoadViewModel.OriginState;
            // If any changes in the Destination City or State
            bool destinationChanged = load.DestinationCity != updatedLoadViewModel.DestinationCity || load.DestinationState != updatedLoadViewModel.DestinationState; 

            // Update properties
            load.OriginCity = updatedLoadViewModel.OriginCity;
            load.OriginState = updatedLoadViewModel.OriginState;
            load.DestinationCity = updatedLoadViewModel.DestinationCity;
            load.DestinationState = updatedLoadViewModel.DestinationState;
            load.PickupTime = updatedLoadViewModel.PickupTime;
            load.DeliveryTime = updatedLoadViewModel.DeliveryTime;
            load.Price = updatedLoadViewModel.PostedPrice;
            load.Weight = updatedLoadViewModel.Weight;

            if (originChanged || destinationChanged) // if either of the two is TRUE ----> Recalculate the distance 
            {
                var distance = await distanceCalculatorService.GetDistanceBetweenCitiesAsync(
                    updatedLoadViewModel.OriginCity,
                    updatedLoadViewModel.OriginState,
                    updatedLoadViewModel.DestinationCity,
                    updatedLoadViewModel.DestinationState
                );

                load.Distance = distance;
            }

            context.Loads.Update(load);
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
                return false; // Load not found or not owned by broker
            }

            // Update status to cancelled (soft delete)

            load.Status = LoadStatus.Cancelled;
            context.Loads.Update(load);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
