using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Moq;
using NUnit.Framework;

using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Services;
using LoadVantage.Areas.Admin.Models.Load;
using LoadVantage.Areas.Admin.Models.Profile;
using LoadVantage.Common.Enums;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Tests.UnitTests.Web.Admin
{
    public class AdminLoadStatusServiceTests
    {
        private LoadVantageDbContext _dbContext;
        private Mock<IAdminProfileService> _adminProfileService;
        private Mock<ILoadHelperService> _mockLoadHelperService;
        private Mock<IDistanceCalculatorService> _mockDistanceCalculatorService;
        private Mock<IHtmlSanitizerService> _mockHtmlSanitizerService;

        private IAdminLoadStatusService _mockAdminLoadStatusService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<LoadVantageDbContext>()
                .UseInMemoryDatabase(databaseName: "LoadVantageTestInMemoryDB")
                .Options;

            _dbContext = new LoadVantageDbContext(options);

            var mockOptions = new Mock<IOptions<IdentityOptions>>();
            mockOptions.Setup(o => o.Value).Returns(new IdentityOptions());

            _mockDistanceCalculatorService = new Mock<IDistanceCalculatorService>();
            _mockLoadHelperService = new Mock<ILoadHelperService>();
            _adminProfileService = new Mock<IAdminProfileService>();
            _mockHtmlSanitizerService = new Mock<IHtmlSanitizerService>();

            _mockDistanceCalculatorService.Setup(s =>
                    s.GetDistanceBetweenCitiesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(100);

            _mockLoadHelperService.Setup(s => s.FormatLocation(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string city, string state) => (city, state));


            _mockAdminLoadStatusService = new AdminLoadStatusService(
                _dbContext,
                _adminProfileService.Object,
                _mockLoadHelperService.Object,
                _mockDistanceCalculatorService.Object,
                _mockHtmlSanitizerService.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task RestoreLoadAsync_LoadExists_ReturnsTrueAndRestoresLoadStatus()
        {
            var loadId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "Sacramento",
                OriginState = "CA",
                DestinationCity = "Chicago",
                DestinationState = "IL",
                Status = LoadStatus.Available
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _mockAdminLoadStatusService.RestoreLoadAsync(loadId);

            var restoredLoad = await _dbContext.Loads.FindAsync(loadId);
            Assert.That(result, Is.True);
            Assert.That(restoredLoad.Status, Is.EqualTo(LoadStatus.Created));
        }

        [Test]
        public async Task RestoreLoadAsync_LoadNotFound_ReturnsFalse()
        {
            var loadId = Guid.NewGuid();

            var result = await _mockAdminLoadStatusService.RestoreLoadAsync(loadId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditLoadAsync_LoadNotFound_ReturnsFalse()
        {
            var loadId = Guid.NewGuid();

            var model = new AdminLoadViewModel
            {
                OriginCity = "City",
                OriginState = "State",
                DestinationCity = "Destination",
                DestinationState = "DestinationState",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(2),
                PostedPrice = 100m,
                Weight = 10.5
            };

            var result = await _mockAdminLoadStatusService.EditLoadAsync(loadId, model);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditLoadAsync_LoadExistsWithChanges_ReturnsTrueAndUpdatesLoad()
        {
            var loadId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "OldCity",
                OriginState = "OldState",
                DestinationCity = "OldDestination",
                DestinationState = "OldDestinationState",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(2),
                Price = 100m,
                Weight = 10.5,
                Distance = 150
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var model = new AdminLoadViewModel
            {
                OriginCity = "NewCity",
                OriginState = "NewState",
                DestinationCity = "NewDestination",
                DestinationState = "NewDestinationState",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(3),
                PostedPrice = 200m,
                Weight = 15.5
            };

            _mockHtmlSanitizerService
                .Setup(s => s.Sanitize(It.IsAny<string>()))
                .Returns<string>(input => input);

            _mockLoadHelperService.Setup(s => s.FormatLocation(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string city, string state) => (city, state));

            var result = await _mockAdminLoadStatusService.EditLoadAsync(loadId, model);

            var updatedLoad = await _dbContext.Loads.FindAsync(loadId);

            Assert.That(result, Is.True);
            Assert.That(updatedLoad.OriginCity, Is.EqualTo("NewCity"));
            Assert.That(updatedLoad.OriginState, Is.EqualTo("NewState"));
            Assert.That(updatedLoad.DestinationCity, Is.EqualTo("NewDestination"));
            Assert.That(updatedLoad.DestinationState, Is.EqualTo("NewDestinationState"));
            Assert.That(updatedLoad.PickupTime, Is.EqualTo(model.PickupTime));
            Assert.That(updatedLoad.DeliveryTime, Is.EqualTo(model.DeliveryTime));
            Assert.That(updatedLoad.Price, Is.EqualTo(model.PostedPrice));
            Assert.That(updatedLoad.Weight, Is.EqualTo(model.Weight));
            Assert.That(updatedLoad.Distance, Is.EqualTo(100));
        }

        [Test]
        public async Task EditLoadAsync_NoChanges_ReturnsFalse()
        {

            var loadId = Guid.NewGuid();
            var load = new Load
            {
                Id = loadId,
                OriginCity = "City",
                OriginState = "State",
                DestinationCity = "Destination",
                DestinationState = "DestinationState",
                PickupTime = new DateTime(2024, 12, 8, 10, 0, 0),
                DeliveryTime = new DateTime(2024, 12, 10, 10, 0, 0),
                Price = 100m,
                Weight = 10.5
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var model = new AdminLoadViewModel
            {
                OriginCity = "City",
                OriginState = "State",
                DestinationCity = "Destination",
                DestinationState = "DestinationState",
                PickupTime = new DateTime(2024, 12, 8, 10, 0, 0),
                DeliveryTime = new DateTime(2024, 12, 10, 10, 0, 0),
                PostedPrice = 100m,
                Weight = 10.5
            };

            _mockHtmlSanitizerService
                .Setup(s => s.Sanitize(It.IsAny<string>()))
                .Returns<string>(input => input);

            _mockLoadHelperService.Setup(s => s.FormatLocation(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string city, string state) => (city, state));

            var result = await _mockAdminLoadStatusService.EditLoadAsync(loadId, model);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditLoadAsync_OriginOrDestinationChange_UpdatesDistance()
        {
            var loadId = Guid.NewGuid();
            var load = new Load
            {
                Id = loadId,
                OriginCity = "OldCity",
                OriginState = "OldState",
                DestinationCity = "OldDestination",
                DestinationState = "OldDestinationState",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(2),
                Price = 100m,
                Weight = 10.5,
                Distance = 150
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var model = new AdminLoadViewModel
            {
                OriginCity = "NewCity",
                OriginState = "NewState",
                DestinationCity = "NewDestination",
                DestinationState = "NewDestinationState",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(2),
                PostedPrice = 100m,
                Weight = 10.5
            };

            _mockHtmlSanitizerService
                .Setup(s => s.Sanitize(It.IsAny<string>()))
                .Returns<string>(input => input);

            _mockLoadHelperService.Setup(s => s.FormatLocation(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string city, string state) => (city, state));

            var mockDistanceCalculator = new Mock<IDistanceCalculatorService>();

            mockDistanceCalculator.Setup(d => d.GetDistanceBetweenCitiesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(200);

            var result = await _mockAdminLoadStatusService.EditLoadAsync(loadId, model);

            var updatedLoad = await _dbContext.Loads.FindAsync(loadId);

            Assert.That(result, Is.True);
            Assert.That(updatedLoad.Distance, Is.EqualTo(100));
        }

        [Test]
        public async Task EditLoadAsync_DistanceCalculationFails_ThrowsException()
        {
            var loadId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "OldCity",
                OriginState = "OldState",
                DestinationCity = "OldDestination",
                DestinationState = "OldDestinationState",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(2),
                Price = 100m,
                Weight = 10.5,
                Distance = 150
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var model = new AdminLoadViewModel
            {
                OriginCity = "NewCity",
                OriginState = "NewState",
                DestinationCity = "NewDestination",
                DestinationState = "NewDestinationState",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(3),
                PostedPrice = 200m,
                Weight = 15.5
            };

            _mockDistanceCalculatorService
                .Setup(s => s.GetDistanceBetweenCitiesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Error calculating distance"));

            Assert.ThrowsAsync<Exception>(async () =>
                await _mockAdminLoadStatusService.EditLoadAsync(loadId, model)
            );
        }

        [Test]
        public async Task GetLoadInformation_LoadExists_ReturnsValidAdminLoadViewModel()
        {
            var loadId = Guid.NewGuid();

            var userId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "OriginCity",
                OriginState = "OriginState",
                DestinationCity = "DestinationCity",
                DestinationState = "DestinationState",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(2),
                Price = 100m,
                Weight = 10.5,
                Distance = 150,
                Status = LoadStatus.Available,
                BrokerId = Guid.NewGuid(),
                Broker = new Broker
                {
                    Id = Guid.NewGuid(),
                    FirstName = "BrokerFirstName",
                    LastName = "BrokerLastName",
                    UserName = "broker.username",
                    Email = "broker@example.com"
                },
                BookedLoad = new BookedLoad
                {
                    DispatcherId = Guid.NewGuid(),
                    Dispatcher = new Dispatcher
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "DispatcherFirstName",
                        LastName = "DispatcherLastName",
                        UserName = "dispatcher.username",
                        Email = "dispatcher@example.com"
                    },
                    Driver = new Driver
                    {
                        DriverId = Guid.NewGuid(),
                        FirstName = "DriverFirstName",
                        LastName = "DriverLastName",
                        LicenseNumber = "DL123456789"
                    }
                }
            };

            _mockLoadHelperService.Setup(s => s.CreateBrokerInfo(It.IsAny<Load>())).Returns(new BrokerInfoViewModel());
            _mockLoadHelperService.Setup(s => s.CreateDispatcherInfo(It.IsAny<BookedLoad>())).Returns(new DispatcherInfoViewModel());
            _mockLoadHelperService.Setup(s => s.CreateDriverInfo(It.IsAny<BookedLoad>())).Returns(new DriverInfoViewModel());
            _adminProfileService.Setup(s => s.GetAdminInformation(It.IsAny<Guid>())).ReturnsAsync(
                new AdminProfileViewModel()
                {
                    Id = userId.ToString(),
                    FirstName = "Admin",
                    LastName = "Administratorium"
                });

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _mockAdminLoadStatusService.GetLoadInformation(loadId, userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(loadId));
            Assert.That(result.OriginCity, Is.EqualTo("OriginCity"));
            Assert.That(result.OriginState, Is.EqualTo("OriginState"));
            Assert.That(result.DestinationCity, Is.EqualTo("DestinationCity"));
            Assert.That(result.DestinationState, Is.EqualTo("DestinationState"));
            Assert.That(result.PickupTime, Is.EqualTo(load.PickupTime));
            Assert.That(result.DeliveryTime, Is.EqualTo(load.DeliveryTime));
            Assert.That(result.PostedPrice, Is.EqualTo(load.Price));
            Assert.That(result.Weight, Is.EqualTo(load.Weight));
            Assert.That(result.Distance, Is.EqualTo(load.Distance));
            Assert.That(result.Status, Is.EqualTo(load.Status.ToString()));
            Assert.That(result.Broker.FirstName, Is.EqualTo(load.Broker.FirstName));
            Assert.That(result.Broker.LastName, Is.EqualTo(load.Broker.LastName));
            Assert.That(result.Broker.UserName, Is.EqualTo(load.Broker.UserName));
            Assert.That(result.Broker.Email, Is.EqualTo(load.Broker.Email));

        }

        [Test]
        public async Task GetLoadInformation_LoadNotFound_ThrowsException()
        {
            var loadId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await _mockAdminLoadStatusService.GetLoadInformation(loadId, userId)
            );

            Assert.That(exception.Message, Is.EqualTo(LoadCouldNotBeRetrieved));
        }

        [Test]
        public async Task GetLoadInformation_NoDispatcherAndDriver_ReturnsValidAdminLoadViewModel()
        {
            var loadId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "OriginCity",
                OriginState = "OriginState",
                DestinationCity = "DestinationCity",
                DestinationState = "DestinationState",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(2),
                Price = 100m,
                Weight = 10.5,
                Distance = 150,
                Status = LoadStatus.Available,
                BrokerId = Guid.NewGuid(),
                Broker = new Broker
                {
                    Id = Guid.NewGuid(),
                    FirstName = "BrokerFirstName",
                    LastName = "BrokerLastName",
                    UserName = "broker.username",
                    Email = "broker@example.com"
                },
                BookedLoad = new BookedLoad
                {
                    DispatcherId = Guid.Empty,
                    Dispatcher = null,
                    Driver = null,
                    DriverId = Guid.Empty
                }
            };

            _mockLoadHelperService.Setup(s => s.CreateBrokerInfo(It.IsAny<Load>())).Returns(new BrokerInfoViewModel());
            _mockLoadHelperService.Setup(s => s.CreateDispatcherInfo(It.IsAny<BookedLoad>())).Returns((DispatcherInfoViewModel)null);
            _mockLoadHelperService.Setup(s => s.CreateDriverInfo(It.IsAny<BookedLoad>())).Returns((DriverInfoViewModel)null);
            _adminProfileService.Setup(s => s.GetAdminInformation(It.IsAny<Guid>())).ReturnsAsync(
                new AdminProfileViewModel()
                {
                    Id = userId.ToString(),
                    FirstName = "Admin",
                    LastName = "Administratorium"
                });


            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _mockAdminLoadStatusService.GetLoadInformation(loadId, userId);

            Assert.That(result.DispatcherInfo, Is.Null);
            Assert.That(result.DriverInfo, Is.Null);


        }

        [Test]
        public async Task GetLoadInformation_WithDispatcherAndDriver_ReturnsValidAdminLoadViewModel()
        {

            var loadId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "OriginCity",
                OriginState = "OriginState",
                DestinationCity = "DestinationCity",
                DestinationState = "DestinationState",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(2),
                Price = 100m,
                Weight = 10.5,
                Distance = 150,
                Status = LoadStatus.Available,
                BrokerId = Guid.NewGuid(),
                Broker = new Broker
                {
                    Id = Guid.NewGuid(),
                    FirstName = "BrokerFirstName",
                    LastName = "BrokerLastName",
                    UserName = "broker.username",
                    Email = "broker@example.com"
                },
                BookedLoad = new BookedLoad
                {
                    DispatcherId = Guid.NewGuid(),
                    DriverId = Guid.NewGuid(),
                    Dispatcher = new Dispatcher
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "DispatcherFirstName",
                        LastName = "DispatcherLastName",
                        UserName = "dispatcher.username",
                        Email = "dispatcher@example.com"
                    },
                    Driver = new Driver
                    {
                        DriverId = Guid.NewGuid(),
                        FirstName = "DriverFirstName",
                        LastName = "DriverLastName",
                        LicenseNumber = "DL123456789"
                    }
                }
            };

            _mockLoadHelperService.Setup(s => s.CreateBrokerInfo(It.IsAny<Load>())).Returns(new BrokerInfoViewModel());
            _mockLoadHelperService.Setup(s => s.CreateDispatcherInfo(It.IsAny<BookedLoad>())).Returns(new DispatcherInfoViewModel
            {
                DispatcherName = "DispatcherFirstName DispatcherLastName",
                DispatcherEmail = "dispatcher@example.com"
            });
            _mockLoadHelperService.Setup(s => s.CreateDriverInfo(It.IsAny<BookedLoad>())).Returns(new DriverInfoViewModel
            {
                DriverName = "DriverFirstName DriverLastName",
                DriverLicenseNumber = "DL123456789"
            });
            _adminProfileService.Setup(s => s.GetAdminInformation(It.IsAny<Guid>())).ReturnsAsync(
                new AdminProfileViewModel()
                {
                    Id = userId.ToString(),
                    FirstName = "Admin",
                    LastName = "Administratorium"
                });

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _mockAdminLoadStatusService.GetLoadInformation(loadId, userId);

            Assert.That(result.DispatcherInfo.DispatcherName, Is.Not.Null);
            Assert.That(result.DispatcherInfo.DispatcherEmail, Is.Not.Null);
            Assert.That(result.DriverInfo.DriverName, Is.Not.Null);
            Assert.That(result.DriverInfo.DriverLicenseNumber, Is.Not.Null);

            Assert.That(result.DispatcherInfo.DispatcherName, Is.EqualTo("DispatcherFirstName DispatcherLastName"));
            Assert.That(result.DriverInfo.DriverName, Is.EqualTo("DriverFirstName DriverLastName"), "DriverName should match.");
            Assert.That(result.DriverInfo.DriverLicenseNumber, Is.EqualTo("DL123456789"), "DriverLicense should match.");
        }

        [Test]
        public async Task GetLoadInformation_BookedWithoutDriver_ReturnsValidAdminLoadViewModel()
        {
            var loadId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "OriginCity",
                OriginState = "OriginState",
                DestinationCity = "DestinationCity",
                DestinationState = "DestinationState",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(2),
                Price = 100m,
                Weight = 10.5,
                Distance = 150,
                Status = LoadStatus.Booked,
                BrokerId = Guid.NewGuid(),
                Broker = new Broker
                {
                    Id = Guid.NewGuid(),
                    FirstName = "BrokerFirstName",
                    LastName = "BrokerLastName",
                    UserName = "broker.username",
                    Email = "broker@example.com"
                },
                BookedLoad = new BookedLoad
                {
                    DispatcherId = Guid.NewGuid(),
                    Driver = null
                }
            };

            _mockLoadHelperService.Setup(s => s.CreateBrokerInfo(It.IsAny<Load>())).Returns(new BrokerInfoViewModel());
            _mockLoadHelperService.Setup(s => s.CreateDispatcherInfo(It.IsAny<BookedLoad>())).Returns(new DispatcherInfoViewModel
            {
                DispatcherName = "DispatcherFirstName DispatcherLastName"
            });
            _mockLoadHelperService.Setup(s => s.CreateDriverInfo(It.IsAny<BookedLoad>())).Returns((DriverInfoViewModel)null);
            _adminProfileService.Setup(s => s.GetAdminInformation(It.IsAny<Guid>())).ReturnsAsync(
                new AdminProfileViewModel()
                {
                    Id = userId.ToString(),
                    FirstName = "Admin",
                    LastName = "Administratorium"
                });

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _mockAdminLoadStatusService.GetLoadInformation(loadId, userId);

            Assert.That(result.DispatcherInfo, Is.Not.Null);
            Assert.That(result.DriverInfo, Is.Null);
            Assert.That(result.DispatcherInfo.DispatcherName, Is.EqualTo("DispatcherFirstName DispatcherLastName"));
        }

        [Test]
        public async Task GetLoadInformation_CreatedLoad_ReturnsValidBrokerInfo()
        {
            var loadId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "OriginCity",
                OriginState = "OriginState",
                DestinationCity = "DestinationCity",
                DestinationState = "DestinationState",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(2),
                Price = 100m,
                Weight = 10.5,
                Distance = 150,
                Status = LoadStatus.Created,
                BrokerId = Guid.NewGuid(),
                Broker = new Broker
                {
                    Id = Guid.NewGuid(),
                    FirstName = "BrokerFirstName",
                    LastName = "BrokerLastName",
                    UserName = "broker.username",
                    Email = "broker@example.com"
                }
            };

            _mockLoadHelperService.Setup(s => s.CreateBrokerInfo(It.IsAny<Load>())).Returns(new BrokerInfoViewModel
            {
                BrokerName = "BrokerFirstName BrokerLastName",
                BrokerEmail = "broker@example.com",
                BrokerPhone = "123-456-7890"
            });

            _mockLoadHelperService.Setup(s => s.CreateDispatcherInfo(It.IsAny<BookedLoad>())).Returns((DispatcherInfoViewModel)null);
            _mockLoadHelperService.Setup(s => s.CreateDriverInfo(It.IsAny<BookedLoad>())).Returns((DriverInfoViewModel)null);
            _adminProfileService.Setup(s => s.GetAdminInformation(It.IsAny<Guid>())).ReturnsAsync(
                new AdminProfileViewModel()
                {
                    Id = userId.ToString(),
                    FirstName = "Admin",
                    LastName = "Administratorium"
                });

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _mockAdminLoadStatusService.GetLoadInformation(loadId, userId);

            Assert.That(result.BrokerInfo, Is.Not.Null);
            Assert.That(result.BrokerInfo.BrokerName, Is.EqualTo("BrokerFirstName BrokerLastName"));
            Assert.That(result.BrokerInfo.BrokerEmail, Is.EqualTo("broker@example.com"));
            Assert.That(result.BrokerInfo.BrokerPhone, Is.EqualTo("123-456-7890"));
        }

        [Test]
        public async Task GetLoadInformation_DeliveredLoad_ReturnsValidBrokerDispatcherDriverInfo()
        {

            var loadId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "OriginCity",
                OriginState = "OriginState",
                DestinationCity = "DestinationCity",
                DestinationState = "DestinationState",
                PickupTime = DateTime.Now.AddHours(-3),
                DeliveryTime = DateTime.Now.AddHours(-1),
                Price = 150m,
                Weight = 12.5,
                Distance = 200,
                Status = LoadStatus.Delivered,
                BrokerId = Guid.NewGuid(),
                Broker = new Broker
                {
                    Id = Guid.NewGuid(),
                    FirstName = "BrokerFirstName",
                    LastName = "BrokerLastName",
                    UserName = "broker.username",
                    Email = "broker@example.com"
                },
                BookedLoad = new BookedLoad
                {
                    DispatcherId = Guid.NewGuid(),
                    Dispatcher = new Dispatcher
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "DispatcherFirstName",
                        LastName = "DispatcherLastName",
                        UserName = "dispatcher.username",
                        Email = "dispatcher@example.com"
                    },
                    Driver = new Driver
                    {
                        DriverId = Guid.NewGuid(),
                        FirstName = "DriverFirstName",
                        LastName = "DriverLastName",
                        LicenseNumber = "DL123456789"
                    }
                }
            };

            _mockLoadHelperService.Setup(s => s.CreateBrokerInfo(It.IsAny<Load>())).Returns(new BrokerInfoViewModel
            {
                BrokerName = "BrokerFirstName BrokerLastName",
                BrokerEmail = "broker@example.com",
                BrokerPhone = "123-456-7890"
            });

            _mockLoadHelperService.Setup(s => s.CreateDispatcherInfo(It.IsAny<BookedLoad>())).Returns(new DispatcherInfoViewModel
            {
                DispatcherName = "DispatcherFirstName DispatcherLastName",
                DispatcherEmail = "dispatcher@example.com"
            });

            _mockLoadHelperService.Setup(s => s.CreateDriverInfo(It.IsAny<BookedLoad>())).Returns(new DriverInfoViewModel
            {
                DriverName = "DriverFirstName DriverLastName",
                DriverLicenseNumber = "DL123456789"
            });

            _adminProfileService.Setup(s => s.GetAdminInformation(It.IsAny<Guid>())).ReturnsAsync(
                new AdminProfileViewModel()
                {
                    Id = userId.ToString(),
                    FirstName = "Admin",
                    LastName = "Administratorium"
                });

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();


            var result = await _mockAdminLoadStatusService.GetLoadInformation(loadId, userId);


            Assert.That(result.BrokerInfo, Is.Not.Null);
            Assert.That(result.BrokerInfo.BrokerName, Is.EqualTo("BrokerFirstName BrokerLastName"));
            Assert.That(result.BrokerInfo.BrokerEmail, Is.EqualTo("broker@example.com"));
            Assert.That(result.BrokerInfo.BrokerPhone, Is.EqualTo("123-456-7890"));

            Assert.That(result.DispatcherInfo, Is.Not.Null);
            Assert.That(result.DispatcherInfo.DispatcherName, Is.EqualTo("DispatcherFirstName DispatcherLastName"));
            Assert.That(result.DispatcherInfo.DispatcherEmail, Is.EqualTo("dispatcher@example.com"));

            Assert.That(result.DriverInfo, Is.Not.Null);
            Assert.That(result.DriverInfo.DriverName, Is.EqualTo("DriverFirstName DriverLastName"));
            Assert.That(result.DriverInfo.DriverLicenseNumber, Is.EqualTo("DL123456789"));
        }


    }
}
