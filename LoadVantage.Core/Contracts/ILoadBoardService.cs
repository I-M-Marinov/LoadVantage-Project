using LoadVantage.Common.Enums;
using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.LoadBoard;

namespace LoadVantage.Core.Contracts
{
    public interface ILoadBoardService
    {
		Task<IEnumerable<LoadViewModel>> GetAllCreatedLoadsForBrokerAsync(Guid userId);
		Task<IEnumerable<LoadViewModel>> GetAllPostedLoadsForBrokerAsync(Guid userId); // Broker reviewing his posted loads
		Task<IEnumerable<LoadViewModel>> GetAllPostedLoadsAsync(Guid userId); // All dispatchers viewing posted loads
		Task<IEnumerable<LoadViewModel>> GetAllBookedLoadsForBrokerAsync(Guid userId);
		Task<IEnumerable<LoadViewModel>> GetAllBookedLoadsForDispatcherAsync(Guid userId);
		Task<IEnumerable<DeliveredLoadViewModel>> GetAllDeliveredLoadsForBrokerAsync(Guid userId);
		Task<IEnumerable<DeliveredLoadViewModel>> GetAllDeliveredLoadsForDispatcherAsync(Guid userId);     
		Task<LoadBoardViewModel> GetBrokerLoadBoardAsync(Guid userId);
		Task<LoadBoardViewModel> GetDispatcherLoadBoardAsync(Guid userId);
		Task<Dictionary<LoadStatus, int>> GetLoadCountsForBrokerAsync(Guid brokerId);
		Task<Dictionary<LoadStatus, int>> GetLoadCountsForDispatcherAsync(Guid dispatcherId);
		Task<Dictionary<string, Dictionary<LoadStatus, int>>> GetLoadCountsForUserAsync(Guid userId, string userPosition);


    }
}
