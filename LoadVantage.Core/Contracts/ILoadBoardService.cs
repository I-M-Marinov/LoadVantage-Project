using LoadVantage.Common.Enums;
using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.LoadBoard;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Core.Contracts
{
    public interface ILoadBoardService
    {
	    /// <summary>
	    /// Retrieves all loads for the user depending if he is the Broker or Dispatcher on the load
	    /// </summary>
		Task<IEnumerable<Load>> GetAllLoads(Guid userId);
	    /// <summary>
	    /// Retrieves all posted loads to visualize them in the Posted Table Tab for all dispatchers 
	    /// </summary>
		Task<IEnumerable<LoadViewModel>> GetAllPostedLoadsAsync(Guid userId);
	    /// <summary>
	    /// Retrieves all information for the Broker Load Board, builds it and serves it back
	    /// </summary>
		Task<LoadBoardViewModel> GetBrokerLoadBoardAsync(Guid userId);
	    /// <summary>
	    /// Retrieves all information for the Dispatcher Load Board, builds it and serves it back
	    /// </summary>
		Task<LoadBoardViewModel> GetDispatcherLoadBoardAsync(Guid userId);
	    /// <summary>
	    /// Retrieves all load count for each load status for all loads that the user is a Broker of. 
	    /// </summary>
		Task<Dictionary<LoadStatus, int>> GetLoadCountsForBrokerAsync(Guid brokerId);
	    /// <summary>
	    /// Retrieves all load count for each load status for all loads that the user is a Dispatcher of. 
	    /// </summary>
		Task<Dictionary<LoadStatus, int>> GetLoadCountsForDispatcherAsync(Guid dispatcherId);
	    /// <summary>
	    /// Retrieves all load counts for each load status for a Broker or Dispatcher.
	    /// Creates a dictionary with key user type and a value of another dictionary containing the load counts for the loads arranged as load status as the key and count as value.
	    /// </summary>
		Task<Dictionary<string, Dictionary<LoadStatus, int>>> GetLoadCountsForUserAsync(Guid userId, string userPosition);


    }
}
