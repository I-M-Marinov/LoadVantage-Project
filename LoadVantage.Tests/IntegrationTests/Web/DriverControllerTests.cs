using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

using Moq;
using NUnit.Framework;

using LoadVantage.Controllers;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Driver;
using LoadVantage.Core.Models.Profile;




namespace LoadVantage.Tests.IntegrationTests.Web
{
	public class DriverControllerTests
	{
		private Mock<IDriverService> mockDriverService;
		private DriverController controller;

		[SetUp]
		public void Setup()
		{
			mockDriverService = new Mock<IDriverService>();
			controller = new DriverController(mockDriverService.Object);

			var services = new ServiceCollection();
			services.AddControllers();
			var serviceProvider = services.BuildServiceProvider();

			var httpContext = new DefaultHttpContext
			{
				RequestServices = serviceProvider
			};

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			};

			var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
			}));

			httpContext.User = user;

			var tempDataProvider = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
			controller.TempData = tempDataProvider;

			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = httpContext
			};
		}


		[Test]
		public async Task ShowDrivers_ReturnsViewWithDrivers()
		{
			var userId = Guid.NewGuid();

			var driverList = new List<DriverViewModel>
			{
				new DriverViewModel { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
				new DriverViewModel { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
			};

			var profile = new ProfileViewModel { Id = userId.ToString(), FirstName  = "Test", LastName = "User"};

			var driversViewModel = new DriversViewModel
			{
				Profile = profile,
				Drivers = driverList
			};

			mockDriverService.Setup(service => service.GetAllDriversAsync(userId)).ReturnsAsync(driversViewModel);

			var result = await controller.ShowDrivers(userId) as ViewResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Model, Is.InstanceOf<DriversViewModel>());

			var model = result.Model as DriversViewModel;
			Assert.That(model.Drivers.Count, Is.EqualTo(driverList.Count));
			Assert.That(model.Profile, Is.EqualTo(profile));
			Assert.That(model.NewDriver, Is.Not.Null);
			Assert.That(model.EditedDriver, Is.Not.Null);

			mockDriverService.Verify(service => service.GetAllDriversAsync(userId), Times.Once);
		}

		[Test]
		public async Task GetDriverDetails_ValidDriverId_ReturnsDriver()
		{
			var driverId = Guid.NewGuid();
			var expectedDriver = new DriverViewModel()
			{
				Id = driverId,
				FirstName = "John",
				LastName = "Doe",
				LicenseNumber = "123456789",
				IsAvailable = true
			};

			mockDriverService.Setup(service => service.GetDriverByIdAsync(driverId))
				.ReturnsAsync(expectedDriver);

			var result = await controller.GetDriverDetails(driverId);


			Assert.That(result, Is.InstanceOf<JsonResult>());
			var jsonResult = result as JsonResult;
			Assert.That(jsonResult.Value, Is.EqualTo(expectedDriver));
		}

		[Test]
		public async Task GetDriverDetails_InvalidDriverId_ReturnsNotFound()
		{
			var driverId = Guid.NewGuid();

			mockDriverService.Setup(service => service.GetDriverByIdAsync(driverId))
				.ReturnsAsync((DriverViewModel)null);

			var result = await controller.GetDriverDetails(driverId);

			Assert.That(result, Is.InstanceOf<NotFoundResult>());
		}

		


	}
}
