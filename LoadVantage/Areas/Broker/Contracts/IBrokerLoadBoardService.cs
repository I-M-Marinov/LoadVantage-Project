using LoadVantage.Areas.Broker.Models;

namespace LoadVantage.Areas.Broker.Contracts
{
    public interface IBrokerLoadBoardService
    {
        Task<IEnumerable<BrokerLoadViewModel>> GetAllCreatedLoadsAsync();
        Task<IEnumerable<BrokerLoadViewModel>> GetAllPostedLoadsAsync();
        Task<IEnumerable<BrokerLoadViewModel>> GetAllBookedLoadsForDispatcherAsync(Guid dispatcherId);
        Task<IEnumerable<BrokerLoadViewModel>> GetAllBilledLoadsForDispatcherAsync(Guid dispatcherId);
        Task<BrokerLoadBoardViewModel> GetBrokerLoadBoardAsync(Guid dispatcherId);
    }
}
