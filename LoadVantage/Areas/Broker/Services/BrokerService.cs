using LoadVantage.Areas.Broker.Contracts;
using LoadVantage.Areas.Broker.Models;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Areas.Broker.Services
{
    public class BrokerService(UserManager<User> userManager) : IBrokerService
    {
        public async Task<ProfileViewModel> GetBrokerInformationAsync(string userId)
        {
            var broker = await userManager.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (broker == null)
            {
                return null; 
            }

            var foundBroker = new ProfileViewModel()
            {
                Id = broker.Id.ToString(),
                Username = broker.UserName!,
                Email = broker.Email!,
                FirstName = broker.FirstName,
                LastName = broker.LastName,
                CompanyName = broker.CompanyName!,
                PhoneNumber = broker.PhoneNumber!,
                Position = broker.Position!
            };

            return foundBroker;
        }

        
    }
}
