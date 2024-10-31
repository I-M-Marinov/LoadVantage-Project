using LoadVantage.Areas.Broker.Contracts;
using LoadVantage.Areas.Broker.Models;
using LoadVantage.Common.Enums;
using LoadVantage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Areas.Broker.Services
{
    public class BrokerLoadBoardService(LoadVantageDbContext context) : IBrokerLoadBoardService
    {
        public async Task<IEnumerable<BrokerLoadViewModel>> GetAllCreatedLoadsForBrokerAsync(Guid brokerId)
        {
            var createdLoads = await context.Loads
                .Where(load => load.Status == LoadStatus.Created && load.BrokerId == brokerId)
                .ToListAsync();

            return createdLoads.Select(load => new BrokerLoadViewModel()
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

        public async Task<IEnumerable<BrokerLoadViewModel>> GetAllPostedLoadsForBrokerAsync(Guid brokerId)
        {
            var postedLoads = await context.Loads
                .Where(load => load.Status == LoadStatus.Available && load.BrokerId == brokerId)
                .ToListAsync();

            return postedLoads.Select(load => new BrokerLoadViewModel()
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

        public async Task<IEnumerable<BrokerLoadViewModel>> GetAllBookedLoadsForBrokerAsync(Guid brokerId)
        {
            var bookedLoads = await context.Loads
                .Where(load => load.BookedLoad != null && load.BrokerId == brokerId)
                .ToListAsync();

            return bookedLoads.Select(load => new BrokerLoadViewModel()
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

            });
        }

        public async Task<IEnumerable<BrokerLoadViewModel>> GetAllBilledLoadsForBrokerAsync(Guid brokerId)
        {
            var billedLoads = await context.Loads
                 .Where(load => load.BilledLoad != null && load.BrokerId == brokerId)
                 .ToListAsync();

            return billedLoads.Select(load => new BrokerLoadViewModel
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
            });
        }

        public async Task<BrokerLoadBoardViewModel> GetBrokerLoadBoardAsync(Guid brokerId)
        {
            var createdLoads = await GetAllCreatedLoadsForBrokerAsync(brokerId);
            var postedLoads = await GetAllPostedLoadsForBrokerAsync(brokerId);
            var bookedLoads = await GetAllBookedLoadsForBrokerAsync(brokerId);
            var billedLoads = await GetAllBilledLoadsForBrokerAsync(brokerId);
            
            return new BrokerLoadBoardViewModel
            {
                BrokerId = brokerId,
                CreatedLoads = createdLoads.ToList(),
                PostedLoads = postedLoads.ToList(),
                BookedLoads = bookedLoads.ToList(),
                BilledLoads = billedLoads.ToList()
            };
        }
    }
}
