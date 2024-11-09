using LoadVantage.Areas.Broker.Contracts;
using LoadVantage.Areas.Broker.Models;
using LoadVantage.Common.Enums;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Areas.Broker.Services
{
    public class BrokerLoadBoardService(LoadVantageDbContext context, UserManager<User> userManager) : IBrokerLoadBoardService
    {
        public async Task<IEnumerable<BrokerLoadViewModel>> GetAllCreatedLoadsForBrokerAsync(Guid brokerId)
        {
            var createdLoads = await context.Loads
                .Where(load => load.Status == LoadStatus.Created && load.BrokerId == brokerId)
                .OrderBy(l => l.PickupTime)
                .ThenByDescending(l => l.OriginCity)
                .ThenByDescending(l => l.OriginState)
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
	        var broker = await userManager.Users
		        .FirstOrDefaultAsync(u => u.Id == brokerId);

			var createdLoads = await GetAllCreatedLoadsForBrokerAsync(brokerId);
            var postedLoads = await GetAllPostedLoadsForBrokerAsync(brokerId);
            var bookedLoads = await GetAllBookedLoadsForBrokerAsync(brokerId);
            var billedLoads = await GetAllBilledLoadsForBrokerAsync(brokerId);

            var userImage = await context.UsersImages.SingleOrDefaultAsync(ui => ui.UserId == brokerId);


            var profileModel = new ProfileViewModel
            {
	            Username = broker.UserName,
	            Email = broker.Email,
	            FirstName = broker.FirstName,
	            LastName = broker.LastName,
	            CompanyName = broker.CompanyName,
	            Position = broker.Position,
	            PhoneNumber = broker.PhoneNumber,
                UserImageUrl = userImage?.ImageUrl
            };

			return new BrokerLoadBoardViewModel
            {
                BrokerId = brokerId,
                FirstName = broker.FirstName,
                LastName = broker.LastName,
                CompanyName = broker.CompanyName,
                Position = broker.Position,
				CreatedLoads = createdLoads.ToList(),
                PostedLoads = postedLoads.ToList(),
                BookedLoads = bookedLoads.ToList(),
                BilledLoads = billedLoads.ToList(),
                Profile = profileModel
			};
        }
    }
}
