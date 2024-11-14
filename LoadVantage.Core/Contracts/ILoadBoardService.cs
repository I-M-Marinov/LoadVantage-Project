using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.LoadBoard;
using LoadVantage.Infrastructure.Data.Models;
using System.Threading.Tasks;

namespace LoadVantage.Core.Contracts
{
    public interface ILoadBoardService
    {
		Task<IEnumerable<LoadViewModel>> GetAllCreatedLoadsForBrokerAsync(Guid userId);
		Task<IEnumerable<LoadViewModel>> GetAllPostedLoadsForBrokerAsync(Guid userId); // Broker reviewing his posted loads
		Task<IEnumerable<LoadViewModel>> GetAllPostedLoadsAsync(Guid userId); // All dispatchers viewing posted loads
		Task<IEnumerable<LoadViewModel>> GetAllBookedLoadsForBrokerAsync(Guid userId);
		Task<IEnumerable<LoadViewModel>> GetAllBookedLoadsForDispatcherAsync(Guid userId);
		Task<IEnumerable<LoadViewModel>> GetAllBilledLoadsForBrokerAsync(Guid userId);
		Task<IEnumerable<LoadViewModel>> GetAllBilledLoadsForDispatcherAsync(Guid userId);
		Task<LoadBoardViewModel> GetBrokerLoadBoardAsync(Guid userId);
		Task<LoadBoardViewModel> GetDispatcherLoadBoardAsync(Guid userId);
		Task<int> GetBookedLoadsCountForDispatcherAsync(Guid userId);
		Task<int> GetBilledLoadsCountForDispatcherAsync(Guid userId);
		Task<int> GetCreatedLoadsCountForBrokerAsync(Guid userId);
		Task<int> GetPostedLoadsCountForBrokerAsync(Guid userId);
		Task<int> GetBookedLoadsCountForBrokerAsync(Guid userId);
		Task<int> GetBilledLoadsCountForBrokerAsync(Guid userId);


    }
}
