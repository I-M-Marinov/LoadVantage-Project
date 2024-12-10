using LoadVantage.Areas.Admin.Models.Statistics;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IStatisticsService
    {
        Task<AllStatsViewModel> GetAllStatistics(Guid userId);
        Task<int> GetTotalUserCountAsync();
        Task<decimal> GetTotalRevenuesAsync();
        Task<int> GetTotalLoadCountAsync();

        Task<Dictionary<string, int>> GetLoadCountsByStatusAsync();
        Task<(int ActiveDrivers, int FiredDrivers)> GetDriverCountsAsync();
        Task<int> GetDispatcherCountAsync();
        Task<int> GetBrokerCountAsync();
        Task<Dictionary<string, int>> GetGroupedCompanyNamesAsync();
        Task<(int AvailableTrucks, int DecommissionedTrucks)> GetTruckCountsAsync();



    }
}
