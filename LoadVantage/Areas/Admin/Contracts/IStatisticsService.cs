using LoadVantage.Areas.Admin.Models.Statistics;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IStatisticsService
    {
        Task<AllStatsViewModel> GetAllStatistics(Guid userId);

    }
}
