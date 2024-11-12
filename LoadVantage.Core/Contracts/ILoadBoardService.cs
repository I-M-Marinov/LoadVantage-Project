using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.LoadBoard;
using System.Threading.Tasks;

namespace LoadVantage.Core.Contracts
{
    public interface ILoadBoardService
    {
		Task<IEnumerable<LoadViewModel>> GetAllCreatedLoadsForBrokerAsync(Guid userId);
		Task<IEnumerable<LoadViewModel>> GetAllPostedLoadsForBrokerAsync(Guid userId); // Broker reviewing his posted loads
		Task<IEnumerable<LoadViewModel>> GetAllPostedLoadsAsync(Guid userId); // All dispatchers viewing posted loads
		Task<IEnumerable<LoadViewModel>> GetAllBookedLoadsForBrokerAsync(Guid userId);
		Task<IEnumerable<LoadViewModel>> GetAllBilledLoadsForBrokerAsync(Guid userId);
		Task<LoadBoardViewModel> GetBrokerLoadBoardAsync(Guid userId);
		Task<LoadBoardViewModel> GetDispatcherLoadBoardAsync(Guid userId);
	}
}
