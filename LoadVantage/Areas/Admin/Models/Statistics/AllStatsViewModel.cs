using LoadVantage.Areas.Admin.Models.Profile;

namespace LoadVantage.Areas.Admin.Models.Statistics
{
	public class AllStatsViewModel
    {
        public AdminProfileViewModel Profile { get; set; } = null!;

        public Dictionary<string, int> LoadCountsByStatus { get; set; } = new Dictionary<string, int>();

        public int TotalUserCount { get; set; }
        public int DispatcherCount { get; set; }
        public int BrokerCount { get; set; }
        public Dictionary<string, int> GroupedCompanyNames { get; set; } = new Dictionary<string, int>();

        public int ActiveDrivers { get; set; }
        public int FiredDrivers { get; set; }

        public int AvailableTrucks { get; set; }
        public int DecommissionedTrucks { get; set; }

        public decimal TotalRevenues { get; set; }
    }
}
