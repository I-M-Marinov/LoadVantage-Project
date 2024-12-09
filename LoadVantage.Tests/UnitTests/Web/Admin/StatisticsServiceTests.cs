using Moq;
using NUnit.Framework;

using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Services;
using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Tests.UnitTests.Web.Admin
{
    public class StatisticsServiceTests
    {

        private Mock<IAdminProfileService> _adminProfileService;
        private Mock<IAdminUserService> _adminUserService;
        private Mock<ILoadHelperService> _loadHelperService;
        private Mock<IDriverService> _driverService;
        private Mock<ITruckService> _truckService;

        private StatisticsService _statisticsService;

        [SetUp]
        public void Setup()
        {


            _adminProfileService = new Mock<IAdminProfileService>();
            _adminUserService = new Mock<IAdminUserService>();
            _loadHelperService = new Mock<ILoadHelperService>();
            _driverService = new Mock<IDriverService>();
            _truckService = new Mock<ITruckService>();



            _statisticsService = new StatisticsService(
                _adminProfileService.Object,
                _adminUserService.Object,
                _loadHelperService.Object,
                _driverService.Object,
                _truckService.Object
            );
        }

        [Test]
        public async Task GetTotalLoadCountAsync_ShouldReturnExpectedCount()
        {
            var expectedCount = 100;

            _loadHelperService.Setup(l => l.GetAllLoadCountsAsync()).ReturnsAsync(expectedCount);

            var result = await _statisticsService.GetTotalLoadCountAsync();

            Assert.That(result, Is.EqualTo(expectedCount));
        }

        [Test]
        public async Task GetTotalUserCountAsync_ShouldReturnExpectedUserCount()
        {
            var expectedCount = 150;

            _adminUserService.Setup(a => a.GetUserCountAsync())
                .ReturnsAsync(expectedCount);
            var result = await _statisticsService.GetTotalUserCountAsync();

            Assert.That(result, Is.EqualTo(expectedCount));
        }

        [Test]
        public async Task GetTotalRevenuesAsync_ShouldReturnExpectedTotalRevenue()
        {
            var loads = new List<Load>
            {
                new Load { Status = LoadStatus.Delivered, Price = 100.50m },
                new Load { Status = LoadStatus.Delivered, Price = 200.75m },
                new Load { Status = LoadStatus.Available, Price = 150.00m },
                new Load { Status = LoadStatus.Delivered, Price = 50.25m }
            };

            _loadHelperService.Setup(l => l.GetAllLoads())
                .ReturnsAsync(loads);

            var expectedRevenue = 100.50m + 200.75m + 50.25m;

            var result = await _statisticsService.GetTotalRevenuesAsync();

            Assert.That(result, Is.EqualTo(expectedRevenue));
        }

        [Test]
        public async Task GetLoadCountsByStatusAsync_ReturnsEmptyDictionary_WhenNoLoadsExist()
        {
	        _loadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(new List<Load>());

	        var result = await _statisticsService.GetLoadCountsByStatusAsync();

	        Assert.That(result, Is.Empty);
        }

		[Test]
		public async Task GetLoadCountsByStatusAsync_ReturnsCorrectCounts_WhenLoadsExist()
		{
			var loads = new List<Load>
		{
			new Load { Status = LoadStatus.Created },
			new Load { Status = LoadStatus.Created },
			new Load { Status = LoadStatus.Available },
			new Load { Status = LoadStatus.Booked },
			new Load { Status = LoadStatus.Booked },
			new Load { Status = LoadStatus.Booked }
		};
			_loadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(loads);

			var result = await _statisticsService.GetLoadCountsByStatusAsync();

			Assert.That(result.Count, Is.EqualTo(3));
			Assert.That(result["Created"], Is.EqualTo(2));
			Assert.That(result["Available"], Is.EqualTo(1));
			Assert.That(result["Booked"], Is.EqualTo(3));
		}

		[Test]
		public async Task GetLoadCountsByStatusAsync_HandlesSingleStatus()
		{
			var loads = new List<Load>
		{
			new Load { Status = LoadStatus.Delivered }
		};
			_loadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(loads);

			var result = await _statisticsService.GetLoadCountsByStatusAsync();

			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result["Delivered"], Is.EqualTo(1));
		}

		[Test]
		public async Task GetLoadCountsByStatusAsync_ReturnsCorrectKeys_WhenStatusesAreMixed()
		{
			var loads = new List<Load>
		{
			new Load { Status = LoadStatus.Available },
			new Load { Status = LoadStatus.Created },
			new Load { Status = LoadStatus.Booked }
		};
			_loadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(loads);

			var result = await _statisticsService.GetLoadCountsByStatusAsync();

			Assert.That(result.Keys, Is.EquivalentTo(new[] { "Available", "Created", "Booked" }));
		}

		[Test]
		public async Task GetDispatcherCountAsync_ReturnsZero_WhenNoDispatchersExist()
		{
			_adminUserService.Setup(s => s.GetDispatcherCountAsync()).ReturnsAsync(0);

			var result = await _statisticsService.GetDispatcherCountAsync();

			Assert.That(result, Is.EqualTo(0));
		}

		[Test]
		public async Task GetDispatcherCountAsync_ReturnsCorrectCount_WhenDispatchersExist()
		{
			_adminUserService.Setup(s => s.GetDispatcherCountAsync()).ReturnsAsync(5);

			var result = await _statisticsService.GetDispatcherCountAsync();

			Assert.That(result, Is.EqualTo(5));
		}

		[Test]
		public void GetDispatcherCountAsync_ThrowsException_WhenServiceThrowsException()
		{
			 _adminUserService.Setup(s => s.GetDispatcherCountAsync()).ThrowsAsync(new InvalidOperationException());

			Assert.That(async () => await _statisticsService.GetDispatcherCountAsync(), Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetDispatcherCountAsync_ReturnsValue_WhenServiceReturnsLargeCount()
		{
			_adminUserService.Setup(s => s.GetDispatcherCountAsync()).ReturnsAsync(10000);

			var result = await _statisticsService.GetDispatcherCountAsync();

			Assert.That(result, Is.EqualTo(10000));
		}

		[Test]
		public async Task GetDispatcherCountAsync_CallsServiceOnce()
		{
			_adminUserService.Setup(s => s.GetDispatcherCountAsync()).ReturnsAsync(5);

			var result = await _statisticsService.GetDispatcherCountAsync();

			Assert.That(result, Is.EqualTo(5));
			_adminUserService.Verify(s => s.GetDispatcherCountAsync(), Times.Once);
		}

		[Test]
		public async Task GetBrokerCountAsync_ReturnsZero_WhenNoBrokersExist()
		{
			_adminUserService.Setup(s => s.GetBrokerCountAsync()).ReturnsAsync(0);

			var result = await _statisticsService.GetBrokerCountAsync();

			Assert.That(result, Is.EqualTo(0));
		}

		[Test]
		public async Task GetBrokerCountAsync_ReturnsCorrectCount_WhenBrokersExist()
		{
			_adminUserService.Setup(s => s.GetBrokerCountAsync()).ReturnsAsync(3);

			var result = await _statisticsService.GetBrokerCountAsync();

			Assert.That(result, Is.EqualTo(3));
		}

		[Test]
		public void GetBrokerCountAsync_ThrowsException_WhenServiceThrowsException()
		{
			_adminUserService.Setup(s => s.GetBrokerCountAsync()).ThrowsAsync(new InvalidOperationException());

			Assert.That(async () => await _statisticsService.GetBrokerCountAsync(), Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetBrokerCountAsync_ReturnsValue_WhenServiceReturnsLargeCount()
		{
			_adminUserService.Setup(s => s.GetBrokerCountAsync()).ReturnsAsync(5000);

			var result = await _statisticsService.GetBrokerCountAsync();

			Assert.That(result, Is.EqualTo(5000));
		}

		[Test]
		public async Task GetBrokerCountAsync_CallsServiceOnce()
		{
			_adminUserService.Setup(s => s.GetBrokerCountAsync()).ReturnsAsync(2);

			var result = await _statisticsService.GetBrokerCountAsync();

			Assert.That(result, Is.EqualTo(2));
			_adminUserService.Verify(s => s.GetBrokerCountAsync(), Times.Once);
		}

		[Test]
		public async Task GetGroupedCompanyNamesAsync_ReturnsEmptyDictionary_WhenNoUsersExist()
		{
			_adminUserService.Setup(s => s.GetAllUsersFromACompany()).ReturnsAsync(new List<User>());

			var result = await _statisticsService.GetGroupedCompanyNamesAsync();

			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task GetGroupedCompanyNamesAsync_ReturnsCorrectGrouping_WhenUsersExist()
		{
			var users = new List<User>
	{
		new User { CompanyName = "CompanyA" },
		new User { CompanyName = "CompanyA" },
		new User { CompanyName = "CompanyB" },
		new User { CompanyName = "CompanyC" }
	};

			_adminUserService.Setup(s => s.GetAllUsersFromACompany()).ReturnsAsync(users);

			var result = await _statisticsService.GetGroupedCompanyNamesAsync();

			Assert.That(result.Count, Is.EqualTo(3));
			Assert.That(result["CompanyA"], Is.EqualTo(2));
			Assert.That(result["CompanyB"], Is.EqualTo(1));
			Assert.That(result["CompanyC"], Is.EqualTo(1));
		}

		[Test]
		public async Task GetGroupedCompanyNamesAsync_ReturnsValue_WhenSingleCompanyExists()
		{
			var users = new List<User>
	{
		new User { CompanyName = "CompanyX" },
		new User { CompanyName = "CompanyX" },
		new User { CompanyName = "CompanyX" }
	};

			_adminUserService.Setup(s => s.GetAllUsersFromACompany()).ReturnsAsync(users);

			var result = await _statisticsService.GetGroupedCompanyNamesAsync();

			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result["CompanyX"], Is.EqualTo(3));
		}

		[Test]
		public async Task GetGroupedCompanyNamesAsync_ThrowsException_WhenServiceThrowsException()
		{
			_adminUserService.Setup(s => s.GetAllUsersFromACompany()).ThrowsAsync(new InvalidOperationException());

			Assert.That(async () => await _statisticsService.GetGroupedCompanyNamesAsync(), Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetGroupedCompanyNamesAsync_CallsServiceOnce()
		{
			_adminUserService.Setup(s => s.GetAllUsersFromACompany()).ReturnsAsync(new List<User>());

			var result = await _statisticsService.GetGroupedCompanyNamesAsync();

			Assert.That(result, Is.Empty);
			_adminUserService.Verify(s => s.GetAllUsersFromACompany(), Times.Once);
		}

		[Test]
		public async Task GetDriverCountsAsync_ReturnsZeroCounts_WhenNoDriversExist()
		{
			_driverService.Setup(s => s.GetAllDrivers()).ReturnsAsync(new List<Driver>());

			var result = await _statisticsService.GetDriverCountsAsync();

			Assert.That(result.ActiveDrivers, Is.EqualTo(0));
			Assert.That(result.FiredDrivers, Is.EqualTo(0));
		}

		[Test]
		public async Task GetDriverCountsAsync_ReturnsCorrectCounts_WhenOnlyActiveDriversExist()
		{
			var drivers = new List<Driver>
			{
				new Driver { IsFired = false },
				new Driver { IsFired = false },
				new Driver { IsFired = false }
			};

			_driverService.Setup(s => s.GetAllDrivers()).ReturnsAsync(drivers);

			var result = await _statisticsService.GetDriverCountsAsync();

			Assert.That(result.ActiveDrivers, Is.EqualTo(3));
			Assert.That(result.FiredDrivers, Is.EqualTo(0));
		}

		[Test]
		public async Task GetDriverCountsAsync_ReturnsCorrectCounts_WhenOnlyFiredDriversExist()
		{
			var drivers = new List<Driver>
			{
				new Driver { IsFired = true },
				new Driver { IsFired = true },
				new Driver { IsFired = true }
			};

			_driverService.Setup(s => s.GetAllDrivers()).ReturnsAsync(drivers);

			var result = await _statisticsService.GetDriverCountsAsync();

			Assert.That(result.ActiveDrivers, Is.EqualTo(0));
			Assert.That(result.FiredDrivers, Is.EqualTo(3));
		}

		[Test]
		public async Task GetDriverCountsAsync_ReturnsCorrectCounts_WhenDriversWithDifferentStatusesExist()
		{
			var drivers = new List<Driver>
			{
				new Driver { IsFired = false },
				new Driver { IsFired = true },
				new Driver { IsFired = false },
				new Driver { IsFired = true },
				new Driver { IsFired = false }
			};

			_driverService.Setup(s => s.GetAllDrivers()).ReturnsAsync(drivers);

			var result = await _statisticsService.GetDriverCountsAsync();

			Assert.That(result.ActiveDrivers, Is.EqualTo(3));
			Assert.That(result.FiredDrivers, Is.EqualTo(2));
		}

		[Test]
		public async Task GetDriverCountsAsync_HandlesEmptyDriverList()
		{
			_driverService.Setup(s => s.GetAllDrivers()).ReturnsAsync(new List<Driver>());

			var result = await _statisticsService.GetDriverCountsAsync();

			Assert.That(result.ActiveDrivers, Is.EqualTo(0));
			Assert.That(result.FiredDrivers, Is.EqualTo(0));
		}

		[Test]
		public async Task GetDriverCountsAsync_ThrowsException_WhenServiceThrowsException()
		{
			_driverService.Setup(s => s.GetAllDrivers()).ThrowsAsync(new InvalidOperationException());

			Assert.That(async () => await _statisticsService.GetDriverCountsAsync(), Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetDriverCountsAsync_CallsServiceOnce()
		{
			_driverService.Setup(s => s.GetAllDrivers()).ReturnsAsync(new List<Driver>());

			var result = await _statisticsService.GetDriverCountsAsync();

			Assert.That(result.ActiveDrivers, Is.EqualTo(0));
			Assert.That(result.FiredDrivers, Is.EqualTo(0));
			_driverService.Verify(s => s.GetAllDrivers(), Times.Once);
		}

		[Test]
		public async Task GetTruckCountsAsync_ReturnsZeroCounts_WhenNoTrucksExist()
		{
			_truckService.Setup(s => s.GetAllTrucksAsync()).ReturnsAsync(new List<Truck>());

			var result = await _statisticsService.GetTruckCountsAsync();

			Assert.That(result.AvailableTrucks, Is.EqualTo(0));
			Assert.That(result.DecommissionedTrucks, Is.EqualTo(0));
		}

		[Test]
		public async Task GetTruckCountsAsync_ReturnsCorrectCounts_WhenOnlyAvailableTrucksExist()
		{
			var trucks = new List<Truck>
			{
				new Truck { IsActive = true },
				new Truck { IsActive = true },
				new Truck { IsActive = true }
			};

			_truckService.Setup(s => s.GetAllTrucksAsync()).ReturnsAsync(trucks);

			var result = await _statisticsService.GetTruckCountsAsync();

			Assert.That(result.AvailableTrucks, Is.EqualTo(3));
			Assert.That(result.DecommissionedTrucks, Is.EqualTo(0));
		}

		[Test]
		public async Task GetTruckCountsAsync_ReturnsCorrectCounts_WhenOnlyDecommissionedTrucksExist()
		{
			var trucks = new List<Truck>
			{
				new Truck { IsActive = false },
				new Truck { IsActive = false },
				new Truck { IsActive = false }
			};

			_truckService.Setup(s => s.GetAllTrucksAsync()).ReturnsAsync(trucks);

			var result = await _statisticsService.GetTruckCountsAsync();

			Assert.That(result.AvailableTrucks, Is.EqualTo(0));
			Assert.That(result.DecommissionedTrucks, Is.EqualTo(3));
		}

		[Test]
		public async Task GetTruckCountsAsync_ReturnsCorrectCounts_WhenMixedTrucksExist()
		{
			var trucks = new List<Truck>
			{
				new Truck { IsActive = true },
				new Truck { IsActive = false },
				new Truck { IsActive = true },
				new Truck { IsActive = false },
				new Truck { IsActive = true }
			};

			_truckService.Setup(s => s.GetAllTrucksAsync()).ReturnsAsync(trucks);

			var result = await _statisticsService.GetTruckCountsAsync();

			Assert.That(result.AvailableTrucks, Is.EqualTo(3));
			Assert.That(result.DecommissionedTrucks, Is.EqualTo(2));
		}

		[Test]
		public async Task GetTruckCountsAsync_HandlesEmptyTruckListGracefully()
		{
			_truckService.Setup(s => s.GetAllTrucksAsync()).ReturnsAsync(new List<Truck>());

			var result = await _statisticsService.GetTruckCountsAsync();

			Assert.That(result.AvailableTrucks, Is.EqualTo(0));
			Assert.That(result.DecommissionedTrucks, Is.EqualTo(0));
		}

		[Test]
		public async Task GetTruckCountsAsync_ThrowsException_WhenServiceThrowsException()
		{
			_truckService.Setup(s => s.GetAllTrucksAsync()).ThrowsAsync(new InvalidOperationException());

			Assert.That(async () => await _statisticsService.GetTruckCountsAsync(), Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetTruckCountsAsync_CallsServiceOnce()
		{
			_truckService.Setup(s => s.GetAllTrucksAsync()).ReturnsAsync(new List<Truck>());

			var result = await _statisticsService.GetTruckCountsAsync();

			Assert.That(result.AvailableTrucks, Is.EqualTo(0));
			Assert.That(result.DecommissionedTrucks, Is.EqualTo(0));
			_truckService.Verify(s => s.GetAllTrucksAsync(), Times.Once);
		}




	}
}
