using LoadVantage.Core.Models.Load;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Core.Contracts
{
	public interface ILoadHelperService
    {
	    /// <summary>
	    /// Retrieves all the loads from the DB
	    /// </summary>
		Task<IEnumerable<Load>> GetAllLoads();
	    /// <summary>
	    /// Retrieves total load count for all loads 
	    /// </summary>
		Task<int> GetAllLoadCountsAsync();
	    /// <summary>
	    /// Formats a city and state ---> example: chicago,   il ----> Chicago, IL 
	    /// </summary>
		(string FormattedCity, string FormattedState) FormatLocation(string city, string state);
	    /// <summary>
	    /// Verifies if a user can rightfully can view a certain load 
	    /// </summary>
		Task<bool> CanUserViewLoadAsync(Guid userId, Guid loadId);
	    /// <summary>
	    /// Assembles and builds Broker short info for visualizing inside the Load Views 
	    /// </summary>
		BrokerInfoViewModel CreateBrokerInfo(Load? load);
	    /// <summary>
	    /// Assembles and builds Dispatcher short info for visualizing inside the Load Views 
	    /// </summary>
		DispatcherInfoViewModel CreateDispatcherInfo(BookedLoad? bookedLoad);
	    /// <summary>
	    /// Assembles and builds Driver short info for visualizing inside the Load Views 
	    /// </summary>
		DriverInfoViewModel CreateDriverInfo(BookedLoad? bookedLoad);

	}
}
