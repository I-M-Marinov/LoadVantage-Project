using LoadVantage.Areas.Broker.Models;
using LoadVantage.Areas.Dispatcher.Models;
using LoadVantage.Core.Models.Profile;

namespace LoadVantage.Areas.Broker.Contracts
{
    public interface IBrokerService
    {
        Task<ProfileViewModel> GetBrokerInformationAsync(string userId);
    }
}
