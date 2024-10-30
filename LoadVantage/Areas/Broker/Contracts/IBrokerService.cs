using LoadVantage.Areas.Broker.Models;
using LoadVantage.Areas.Dispatcher.Models;

namespace LoadVantage.Areas.Broker.Contracts
{
    public interface IBrokerService
    {
        Task<BrokerViewModel> GetBrokerInformationAsync(string userId);
    }
}
