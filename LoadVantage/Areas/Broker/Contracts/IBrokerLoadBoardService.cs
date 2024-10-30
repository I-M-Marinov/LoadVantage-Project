using LoadVantage.Areas.Broker.Models;

namespace LoadVantage.Areas.Broker.Contracts
{
    public interface IBrokerLoadBoardService
    {
        Task<IEnumerable<BrokerLoadViewModel>> GetAllCreatedLoadsForBrokerAsync(Guid brokerId);
        Task<IEnumerable<BrokerLoadViewModel>> GetAllPostedLoadsForBrokerAsync(Guid brokerId);
        Task<IEnumerable<BrokerLoadViewModel>> GetAllBookedLoadsForBrokerAsync(Guid brokerId);
        Task<IEnumerable<BrokerLoadViewModel>> GetAllBilledLoadsForBrokerAsync(Guid brokerId);        
        Task<BrokerLoadBoardViewModel> GetBrokerLoadBoardAsync(Guid brokerId);
    }
}
