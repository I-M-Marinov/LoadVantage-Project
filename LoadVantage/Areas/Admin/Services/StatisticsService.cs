using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.Statistics;
using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;

namespace LoadVantage.Areas.Admin.Services
{
	public class StatisticsService : IStatisticsService
    {
        private readonly IAdminProfileService adminProfileService;
        private readonly IAdminUserService adminUserService;
        private readonly ILoadHelperService loadHelperService;
        private readonly IDriverService driverService;
        private readonly ITruckService truckService;

        public StatisticsService(
            IAdminProfileService _adminProfileService,
            IAdminUserService _adminUserService,
            ILoadHelperService _loadHelperService, 
            IDriverService _driverService,
            ITruckService _truckService)
        {
            adminProfileService = _adminProfileService;
            adminUserService = _adminUserService;
            loadHelperService = _loadHelperService;
            driverService = _driverService;
            truckService = _truckService;
        }

        public async Task<AllStatsViewModel> GetAllStatistics(Guid adminId)
        {
            var adminProfile = await adminProfileService.GetAdminInformation(adminId);
            var loadCountsByStatus = await GetLoadCountsByStatusAsync();
            var totalUserCount = await GetTotalUserCountAsync();
            var dispatchersCount = await GetDispatcherCountAsync();
            var brokersCount = await GetBrokerCountAsync();
            var companies = await GetGroupedCompanyNamesAsync();
            var driverCounts = await GetDriverCountsAsync();
            var truckCounts = await GetTruckCountsAsync();
            var totalRevenue = await GetTotalRevenuesAsync();



            var allStatsViewModel = new AllStatsViewModel
            {
                Profile = adminProfile!,
                LoadCountsByStatus = loadCountsByStatus,
                TotalUserCount = totalUserCount,
                DispatcherCount = dispatchersCount,
                BrokerCount = brokersCount,
                GroupedCompanyNames = companies,
                ActiveDrivers = driverCounts.ActiveDrivers,
                FiredDrivers = driverCounts.FiredDrivers,
                AvailableTrucks = truckCounts.AvailableTrucks,
                DecommissionedTrucks = truckCounts.DecommissionedTrucks,
                TotalRevenues = totalRevenue
            };

            return allStatsViewModel;
        }

        public async Task<int> GetTotalLoadCountAsync()
        {
	        return await loadHelperService.GetAllLoadCountsAsync();
        }
		public async Task<int> GetTotalUserCountAsync()
        {
            return await adminUserService.GetUserCountAsync();
        }
        public async Task<decimal> GetTotalRevenuesAsync()
        {
	        var allLoads = await loadHelperService.GetAllLoads();

	        var revenues = allLoads
		        .Where(l => l.Status == LoadStatus.Delivered)
		        .Sum(l => l.Price);

	        return revenues;
        }
        private async Task<Dictionary<string, int>> GetLoadCountsByStatusAsync()
        {
	        var allLoads = await loadHelperService.GetAllLoads();

	        return allLoads
		        .GroupBy(load => load.Status)
		        .ToDictionary(g => g.Key.ToString(), g => g.Count());
        }
		private async Task<int> GetDispatcherCountAsync()
        {
            return await adminUserService.GetDispatcherCountAsync();
        }
        private async Task<int> GetBrokerCountAsync()
        {
            return await adminUserService.GetBrokerCountAsync();
        }
        private async Task<Dictionary<string, int>> GetGroupedCompanyNamesAsync()
        {
            var allUsers = await adminUserService.GetAllUsersFromACompany();

            var groupedUsers = allUsers
                .GroupBy(user => user.CompanyName)
                .ToDictionary(group => group.Key!, group => group.Count());

            return groupedUsers;
        }
        private async Task<(int ActiveDrivers, int FiredDrivers)> GetDriverCountsAsync()
        {
            var drivers = await driverService.GetAllDrivers();

            var activeDrivers = drivers.Count(driver => driver.IsFired == false);
            var firedDrivers = drivers.Count(driver => driver.IsFired == true);

            return (activeDrivers, firedDrivers);
        }
        private async Task<(int AvailableTrucks, int DecommissionedTrucks)> GetTruckCountsAsync()
        {
            var trucks = await truckService.GetAllTrucksAsync();

            var availableTrucks = trucks.Count(truck => truck.IsActive);
            var decommissionedTrucks = trucks.Count(truck => !truck.IsActive);

            return (availableTrucks, decommissionedTrucks);
        }
        

    }
}
