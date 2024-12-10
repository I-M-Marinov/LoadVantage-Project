using LoadVantage.Areas.Admin.Models.Statistics;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IStatisticsService
    {
	    /// <summary>
	    /// Retrieves all statistics visualized in the Statistics View 
	    /// </summary>
		Task<AllStatsViewModel> GetAllStatistics(Guid userId);
	    /// <summary>
	    /// Retrieves total count of all users 
	    /// </summary>
		Task<int> GetTotalUserCountAsync();
	    /// <summary>
	    /// Retrieves total amount of revenue of all loads with Status Delivered
	    /// </summary>
		Task<decimal> GetTotalRevenuesAsync();
	    /// <summary>
	    /// Retrieves total load count of all users in all statuses ( Cancelled included ) 
	    /// </summary>
		Task<int> GetTotalLoadCountAsync();
	    /// <summary>
	    /// Retrieves total count per status of all loads and returns a dictionary with a Load.Status key and load counts value
	    /// </summary>
		Task<Dictionary<string, int>> GetLoadCountsByStatusAsync();
	    /// <summary>
	    /// Retrieves total count of all drivers ( All Dispatchers )  
	    /// </summary>
		Task<(int ActiveDrivers, int FiredDrivers)> GetDriverCountsAsync();
	    /// <summary>
	    /// Retrieves total count of all Dispatchers  
	    /// </summary>
		Task<int> GetDispatcherCountAsync();
	    /// <summary>
	    /// Retrieves total count of all Brokers  
	    /// </summary>
		Task<int> GetBrokerCountAsync();
	    /// <summary>
	    /// Retrieves total count per Company Name of all users and returns a dictionary with a Company Name as key and accounts ( user ) count as value  
	    /// </summary>
		Task<Dictionary<string, int>> GetGroupedCompanyNamesAsync();
		/// <summary>
		/// Retrieves total count of all the trucks that are active(available) and not active(decommissioned)  
		/// </summary>
		Task<(int AvailableTrucks, int DecommissionedTrucks)> GetTruckCountsAsync();



    }
}
