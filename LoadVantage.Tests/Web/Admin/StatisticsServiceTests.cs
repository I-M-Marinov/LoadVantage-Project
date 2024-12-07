using Moq;
using NUnit.Framework;

using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Services;
using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Tests.Web.Admin
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


	}
}
