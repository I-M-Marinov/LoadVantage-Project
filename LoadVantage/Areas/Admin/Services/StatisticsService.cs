using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.Statistics;
using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;

namespace LoadVantage.Areas.Admin.Services
{
	public class StatisticsService : IStatisticsService
    {
        private readonly IAdminProfileService adminProfileService;
        private readonly ILoadHelperService loadHelperService;
        private readonly IUserService userService;
        private readonly IDriverService driverService;
        private readonly ITruckService truckService;

        public StatisticsService(
            IAdminProfileService _adminProfileService, 
            ILoadHelperService _loadHelperService, 
            IUserService _userService,
            IDriverService _driverService,
            ITruckService _truckService)
        {
            adminProfileService = _adminProfileService;
            loadHelperService = _loadHelperService;
            userService = _userService;
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
            var totalRevenue = await GetTotalRevenues();



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

        private async Task<Dictionary<string, int>> GetLoadCountsByStatusAsync()
        {
            var allLoads = await loadHelperService.GetAllLoads();

            return allLoads
                .GroupBy(load => load.Status)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());
        }
        private async Task<int> GetTotalUserCountAsync()
        {
            return await userService.GetUserCountAsync();
        }
        private async Task<int> GetDispatcherCountAsync()
        {
            return await userService.GetDispatcherCountAsync();
        }
        private async Task<int> GetBrokerCountAsync()
        {
            return await userService.GetBrokerCountAsync();
        }
        private async Task<Dictionary<string, int>> GetGroupedCompanyNamesAsync()
        {
            var allUsers = await userService.GetAllUsersFromACompany();

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

            var availableTrucks = trucks.Count(truck => !truck.IsAvailable && truck.IsActive);
            var decommissionedTrucks = trucks.Count(truck => !truck.IsActive);

            return (availableTrucks, decommissionedTrucks);
        }
        private async Task<decimal> GetTotalRevenues()
        {
            var allLoads = await loadHelperService.GetAllLoads();

            var revenues = allLoads
                .Where(l => l.Status == LoadStatus.Delivered)  
                .Sum(l => l.Price);  

            return revenues;
        }

    }
}
