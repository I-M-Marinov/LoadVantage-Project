using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.LoadBoard;

namespace LoadVantage.Core.Contracts
{
    public interface ILoadBoardService
    {
		Task<IEnumerable<LoadViewModel>> GetAllCreatedLoadsForBrokerAsync(Guid userId);
		Task<IEnumerable<LoadViewModel>> GetAllPostedLoadsForBrokerAsync(Guid userId);
		Task<IEnumerable<LoadViewModel>> GetAllBookedLoadsForBrokerAsync(Guid userId);
		Task<IEnumerable<LoadViewModel>> GetAllBilledLoadsForBrokerAsync(Guid userId);
		Task<LoadBoardViewModel> GetLoadBoardAsync(Guid userId);
	}
}
