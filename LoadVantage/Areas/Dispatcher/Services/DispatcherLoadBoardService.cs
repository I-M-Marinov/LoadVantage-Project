using LoadVantage.Areas.Dispatcher.Contracts;
using LoadVantage.Areas.Dispatcher.Models;
using LoadVantage.Common.Enums;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Areas.Dispatcher.Services
{
    public class DispatcherLoadBoardService(LoadVantageDbContext context) : IDispatcherLoadBoardService
    {
        public async Task<IEnumerable<DispatcherLoadViewModel>> GetAllPostedLoadsAsync()
        {
            var postedLoads = await context.Loads
                .Where(load => load.Status == LoadStatus.Available)
                .ToListAsync();

            return postedLoads.Select(load => new DispatcherLoadViewModel
            {
                Id = load.Id,
                OriginCity = load.OriginCity,
                OriginState = load.OriginState,
                DestinationCity = load.DestinationCity,
                DestinationState = load.DestinationState,
                PickupTime = load.PickupTime,
                DeliveryTime = load.DeliveryTime,
                PostedPrice = load.Price,
                Weight = load.Weight,
                Status = load.Status.ToString(), // or however you want to display status
                BrokerId = load.BrokerId
            });
        }

        public async Task<IEnumerable<DispatcherLoadViewModel>> GetAllBookedLoadsForDispatcherAsync(Guid dispatcherId)
        {
            var bookedLoads = await context.Loads
                .Where(load => load.BookedLoad != null && load.BookedLoad.DispatcherId == dispatcherId)
                .ToListAsync();

            return bookedLoads.Select(load => new DispatcherLoadViewModel
            {
                Id = load.Id,
                OriginCity = load.OriginCity,
                OriginState = load.OriginState,
                DestinationCity = load.DestinationCity,
                DestinationState = load.DestinationState,
                PickupTime = load.PickupTime,
                DeliveryTime = load.DeliveryTime,
                PostedPrice = load.Price,
                Weight = load.Weight,
                Status = load.Status.ToString(),
                BrokerId = load.BrokerId,
                DispatcherId = dispatcherId, // Include dispatcherId for clarity
                                             // Map other necessary properties from booked load if needed
            });
        }

        public async Task<IEnumerable<DispatcherLoadViewModel>> GetAllBilledLoadsForDispatcherAsync(Guid dispatcherId)
        {
            var billedLoads = await context.Loads
                .Where(load => load.BilledLoad != null && load.BookedLoad!.DispatcherId == dispatcherId)
                .ToListAsync();

            return billedLoads.Select(load => new DispatcherLoadViewModel
            {
                Id = load.Id,
                OriginCity = load.OriginCity,
                OriginState = load.OriginState,
                DestinationCity = load.DestinationCity,
                DestinationState = load.DestinationState,
                PickupTime = load.PickupTime,
                DeliveryTime = load.DeliveryTime,
                PostedPrice = load.Price,
                Weight = load.Weight,
                Status = load.Status.ToString(),
                BrokerId = load.BrokerId,
                // Map additional properties from billed load if needed
            });
        }

        public async Task<DispatcherLoadBoardViewModel> GetDispatcherLoadBoardAsync(Guid dispatcherId)
        {
            var postedLoads = await GetAllPostedLoadsAsync();
            var bookedLoads = await GetAllBookedLoadsForDispatcherAsync(dispatcherId);
            var billedLoads = await GetAllBilledLoadsForDispatcherAsync(dispatcherId);

            return new DispatcherLoadBoardViewModel
            {
                PostedLoads = postedLoads.ToList(),
                BookedLoads = bookedLoads.ToList(),
                BilledLoads = billedLoads.ToList()
            };
        }
    }

}
