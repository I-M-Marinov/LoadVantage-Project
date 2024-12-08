using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using NUnit.Framework;
using Moq;

using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data.Contracts;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Tests.UnitTests.Core.Services
{
    public class LoadStatusServiceTests
    {
        private ILoadStatusService _mockLoadStatusService;
        private IProfileService _profileService;
        private LoadVantageDbContext _dbContext;
        private IDistanceCalculatorService _mockDistanceCalculatorService;
        private Mock<ILoadHelperService> _mockLoadHelperService;
        private Mock<IHtmlSanitizerService> _mockHtmlSanitizerService;


        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<LoadVantageDbContext>()
                .UseInMemoryDatabase(databaseName: "LoadVantageTestInMemoryDB")
                .Options;

            _dbContext = new LoadVantageDbContext(options);

            var mockOptions = new Mock<IOptions<IdentityOptions>>();
            mockOptions.Setup(o => o.Value).Returns(new IdentityOptions());

            _mockDistanceCalculatorService = Mock.Of<IDistanceCalculatorService>();
            _mockLoadHelperService = new Mock<ILoadHelperService>();
            _profileService = Mock.Of<IProfileService>();
            _mockHtmlSanitizerService = new Mock<IHtmlSanitizerService>();

            _mockLoadStatusService = new LoadStatusService(
                _profileService,
                _dbContext,
                _mockDistanceCalculatorService,
                _mockLoadHelperService.Object,
                _mockHtmlSanitizerService.Object
            );


        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        private void SeedTestData(out Guid userId, out Guid loadId)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testUser",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            _dbContext.Users.Add(user);
            userId = user.Id;

            var load = new Load
            {
                Id = Guid.NewGuid(),
                OriginCity = "Sacramento",
                OriginState = "CA",
                DestinationCity = "Kent",
                DestinationState = "WA",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(3),
                Price = 100.0m,
                Distance = 500,
                Weight = 1000,
                Status = LoadStatus.Available,
                BrokerId = user.Id,
            };

            _dbContext.Loads.Add(load);
            loadId = load.Id;

            _dbContext.SaveChanges();
        }

        [Test]
        public async Task GetLoadDetailsAsync_ShouldReturnLoadDetails_WhenUserIsAllowed()
        {
            Guid userId;
            Guid loadId;

            SeedTestData(out userId, out loadId);

            var mockLoadHelperService = new Mock<ILoadHelperService>();
            var mockProfileService = new Mock<IProfileService>();

            mockLoadHelperService.Setup(x => x.CanUserViewLoadAsync(userId, loadId)).ReturnsAsync(true);


            var mockUserProfile = new ProfileViewModel()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            mockProfileService.Setup(x => x.GetUserInformation(userId)).ReturnsAsync(mockUserProfile);

            var loadStatusService = new LoadStatusService(mockProfileService.Object, _dbContext,
                _mockDistanceCalculatorService, mockLoadHelperService.Object, _mockHtmlSanitizerService.Object);

            var result = await loadStatusService.GetLoadDetailsAsync(loadId, userId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.OriginCity, Is.EqualTo("Sacramento"));
            Assert.That(result.DestinationCity, Is.EqualTo("Kent"));
            Assert.That(result.PostedPrice, Is.EqualTo(100.0m));
            Assert.That(result.Status, Is.EqualTo(LoadStatus.Available.ToString()));
        }

        [Test]
        public async Task GetLoadByIdAsync_ShouldReturnLoadViewModel_WhenLoadExists()
        {
            Guid loadId;
            SeedTestData(out _, out loadId);

            var result = await _mockLoadStatusService.GetLoadByIdAsync(loadId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(loadId));
            Assert.That(result.OriginCity, Is.EqualTo("Sacramento"));
            Assert.That(result.OriginState, Is.EqualTo("CA"));
            Assert.That(result.DestinationCity, Is.EqualTo("Kent"));
            Assert.That(result.DestinationState, Is.EqualTo("WA"));
            Assert.That(result.PickupTime, Is.Not.Null.And.GreaterThan(DateTime.MinValue));
            Assert.That(result.DeliveryTime, Is.Not.Null.And.GreaterThan(result.PickupTime));
            Assert.That(result.PostedPrice, Is.EqualTo(100.0m));
            Assert.That(result.Distance, Is.EqualTo(500));
            Assert.That(result.Weight, Is.EqualTo(1000));
            Assert.That(result.BrokerId, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo("Available"));
        }

        [Test]
        public async Task GetLoadByIdAsync_ShouldReturnNull_WhenLoadDoesNotExist()
        {
            var nonExistentLoadId = Guid.NewGuid();

            var result = await _mockLoadStatusService.GetLoadByIdAsync(nonExistentLoadId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateLoadAsync_ShouldCreateLoadAndReturnId_WhenValidModelIsProvided()
        {
            Guid brokerId = Guid.NewGuid();

            var loadViewModel = new LoadViewModel
            {
                OriginCity = "Sacramento",
                OriginState = "CA",
                DestinationCity = "Kent",
                DestinationState = "WA",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(3),
                PostedPrice = 500.0m,
                Weight = 1200
            };

            var sanitizedCity = "SanitizedCity";
            var sanitizedState = "SanitizedState";
            var formattedCity = "FormattedCity";
            var formattedState = "FormattedState";
            var calculatedDistance = 800.0;

            var mockDistanceCalculatorService = new Mock<IDistanceCalculatorService>();
            var mockLoadHelperService = new Mock<ILoadHelperService>();
            var mockHtmlSanitizerService = new Mock<IHtmlSanitizerService>();

            mockHtmlSanitizerService.Setup(x => x.Sanitize(It.IsAny<string>()))
                .Returns(sanitizedCity);

            mockDistanceCalculatorService.Setup(x =>
                    x.GetDistanceBetweenCitiesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .ReturnsAsync(calculatedDistance);

            mockLoadHelperService.Setup(x => x.FormatLocation(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((formattedCity, formattedState));

            var loadStatusService = new LoadStatusService(
                _profileService,
                _dbContext,
                mockDistanceCalculatorService.Object,
                mockLoadHelperService.Object,
                mockHtmlSanitizerService.Object
            );

            var result = await loadStatusService.CreateLoadAsync(loadViewModel, brokerId);

            var createdLoad = await _dbContext.Loads.FindAsync(result);

            Assert.That(result, Is.Not.EqualTo(Guid.Empty));
            Assert.That(createdLoad, Is.Not.Null);
            Assert.That(createdLoad.OriginCity, Is.EqualTo(formattedCity));
            Assert.That(createdLoad.OriginState, Is.EqualTo(formattedState));
            Assert.That(createdLoad.DestinationCity, Is.EqualTo(formattedCity));
            Assert.That(createdLoad.DestinationState, Is.EqualTo(formattedState));
            Assert.That(createdLoad.Distance, Is.EqualTo(calculatedDistance));
            Assert.That(createdLoad.Price, Is.EqualTo(loadViewModel.PostedPrice));
            Assert.That(createdLoad.Weight, Is.EqualTo(loadViewModel.Weight));
            Assert.That(createdLoad.BrokerId, Is.EqualTo(brokerId));
            Assert.That(createdLoad.Status, Is.EqualTo(LoadStatus.Created));
        }

        [Test]
        public void CreateLoadAsync_ShouldThrowException_WhenDistanceCalculationFails()
        {
            Guid brokerId = Guid.NewGuid();
            var loadViewModel = new LoadViewModel
            {
                OriginCity = "Sacramento",
                OriginState = "CA",
                DestinationCity = "Kent",
                DestinationState = "WA",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(3),
                PostedPrice = 500.0m,
                Weight = 1200
            };

            var mockDistanceCalculatorService = new Mock<IDistanceCalculatorService>();
            var mockLoadHelperService = new Mock<ILoadHelperService>();
            var mockHtmlSanitizerService = new Mock<IHtmlSanitizerService>();

            mockHtmlSanitizerService.Setup(x => x.Sanitize(It.IsAny<string>()))
                .Returns((string input) => input);

            mockDistanceCalculatorService.Setup(x =>
                    x.GetDistanceBetweenCitiesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .ThrowsAsync(new Exception("Distance calculation failed"));

            var loadStatusService = new LoadStatusService(
                _profileService,
                _dbContext,
                mockDistanceCalculatorService.Object,
                mockLoadHelperService.Object,
                mockHtmlSanitizerService.Object
            );

            var ex = Assert.ThrowsAsync<Exception>(async () => await loadStatusService.CreateLoadAsync(loadViewModel, brokerId));
            Assert.That(ex.Message, Is.EqualTo("Distance calculation failed"));
        }

        [Test]
        public async Task CreateLoadAsync_ShouldFormatLocationsCorrectly()
        {
            Guid brokerId = Guid.NewGuid();

            var loadViewModel = new LoadViewModel
            {
                OriginCity = "sacramento",
                OriginState = "ca",
                DestinationCity = "kent",
                DestinationState = "wa",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(3),
                PostedPrice = 500.0m,
                Weight = 1200
            };

            var formattedOriginCity = "Sacramento";
            var formattedOriginState = "CA";
            var formattedDestinationCity = "Kent";
            var formattedDestinationState = "WA";

            var mockDistanceCalculatorService = new Mock<IDistanceCalculatorService>();
            var mockHtmlSanitizerService = new Mock<IHtmlSanitizerService>();
            var loadHelperService = new LoadHelperService(_dbContext);

            var calculatedDistance = 666.0;

            mockHtmlSanitizerService.Setup(x => x.Sanitize(It.IsAny<string>()))
                .Returns((string input) => input);

            mockDistanceCalculatorService.Setup(x =>
                    x.GetDistanceBetweenCitiesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .ReturnsAsync(calculatedDistance);

            var loadStatusService = new LoadStatusService(
                _profileService,
                _dbContext,
                mockDistanceCalculatorService.Object,
                loadHelperService,
                mockHtmlSanitizerService.Object
            );

            var result = await loadStatusService.CreateLoadAsync(loadViewModel, brokerId);

            var createdLoad = await _dbContext.Loads.FindAsync(result);

            Assert.That(createdLoad, Is.Not.Null);
            Assert.That(createdLoad.OriginCity, Is.EqualTo(formattedOriginCity));
            Assert.That(createdLoad.OriginState, Is.EqualTo(formattedOriginState));
            Assert.That(createdLoad.DestinationCity, Is.EqualTo(formattedDestinationCity));
            Assert.That(createdLoad.DestinationState, Is.EqualTo(formattedDestinationState));

        }

        [Test]
        public async Task CreateLoadAsync_ShouldCalculateDistanceCorrectly()
        {
            Guid brokerId = Guid.NewGuid();

            var loadViewModel = new LoadViewModel
            {
                OriginCity = "Sacramento",
                OriginState = "CA",
                DestinationCity = "Kent",
                DestinationState = "WA",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(3),
                PostedPrice = 500.0m,
                Weight = 1200
            };

            var sanitizedCity = "Sacramento";
            var sanitizedState = "CA";
            var formattedCity = "Kent";
            var formattedState = "WA";
            var expectedDistance = 800.0;

            var mockLoadHelperService = new Mock<ILoadHelperService>();
            var mockHtmlSanitizerService = new Mock<IHtmlSanitizerService>();
            var mockDistanceCalculatorService = new Mock<IDistanceCalculatorService>();

            mockDistanceCalculatorService.Setup(x =>
                    x.GetDistanceBetweenCitiesAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .ReturnsAsync(expectedDistance);

            mockHtmlSanitizerService.Setup(x => x.Sanitize(It.IsAny<string>()))
                .Returns(sanitizedCity);

            mockLoadHelperService.Setup(x => x.FormatLocation(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((formattedCity, formattedState));

            var loadStatusService = new LoadStatusService(
                _profileService,
                _dbContext,
                mockDistanceCalculatorService.Object,
                mockLoadHelperService.Object,
                mockHtmlSanitizerService.Object
            );

            var result = await loadStatusService.CreateLoadAsync(loadViewModel, brokerId);

            var createdLoad = await _dbContext.Loads.FindAsync(result);

            Assert.That(createdLoad, Is.Not.Null);
            Assert.That(createdLoad.Distance, Is.EqualTo(expectedDistance));
        }

        [Test]
        public async Task EditLoadAsync_ShouldUpdateLoad_WhenValidChangesAreMade()
        {
            Guid loadId = Guid.NewGuid();

            var originalLoad = new Load
            {
                Id = loadId,
                OriginCity = "Sacramento",
                OriginState = "CA",
                DestinationCity = "Kent",
                DestinationState = "WA",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(3),
                Price = 500.0m,
                Weight = 1200,
                Distance = 800.0
            };

            await _dbContext.Loads.AddAsync(originalLoad);
            await _dbContext.SaveChangesAsync();

            var updatedModel = new LoadViewModel
            {
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                PickupTime = DateTime.Now.AddHours(2),
                DeliveryTime = DateTime.Now.AddHours(4),
                PostedPrice = 600.0m,
                Weight = 1400
            };

            var sanitizedCity = "SanitizedCity";
            var sanitizedState = "SanitizedState";
            var formattedCity = "FormattedCity";
            var formattedState = "FormattedState";
            var recalculatedDistance = 900.0;

            var mockDistanceCalculatorService = new Mock<IDistanceCalculatorService>();
            var mockLoadHelperService = new Mock<ILoadHelperService>();
            var mockHtmlSanitizerService = new Mock<IHtmlSanitizerService>();

            mockHtmlSanitizerService.Setup(x => x.Sanitize(It.IsAny<string>()))
                .Returns(sanitizedCity);

            mockLoadHelperService.Setup(x => x.FormatLocation(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((formattedCity, formattedState));

            mockDistanceCalculatorService.Setup(x =>
                    x.GetDistanceBetweenCitiesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .ReturnsAsync(recalculatedDistance);

            var loadStatusService = new LoadStatusService(
                _profileService,
                _dbContext,
                mockDistanceCalculatorService.Object,
                mockLoadHelperService.Object,
                mockHtmlSanitizerService.Object
            );

            var result = await loadStatusService.EditLoadAsync(loadId, updatedModel);

            var updatedLoad = await _dbContext.Loads.FindAsync(loadId);

            Assert.That(result, Is.True);
            Assert.That(updatedLoad.OriginCity, Is.EqualTo(formattedCity));
            Assert.That(updatedLoad.OriginState, Is.EqualTo(formattedState));
            Assert.That(updatedLoad.DestinationCity, Is.EqualTo(formattedCity));
            Assert.That(updatedLoad.DestinationState, Is.EqualTo(formattedState));
            Assert.That(updatedLoad.PickupTime, Is.EqualTo(updatedModel.PickupTime));
            Assert.That(updatedLoad.DeliveryTime, Is.EqualTo(updatedModel.DeliveryTime));
            Assert.That(updatedLoad.Price, Is.EqualTo(updatedModel.PostedPrice));
            Assert.That(updatedLoad.Weight, Is.EqualTo(updatedModel.Weight));
            Assert.That(updatedLoad.Distance, Is.EqualTo(recalculatedDistance));
        }

        [Test]
        public async Task EditLoadAsync_ShouldReturnFalse_WhenLoadNotFound()
        {
            var nonExistentLoadId = Guid.NewGuid();
            var updatedModel = new LoadViewModel
            {
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                PickupTime = DateTime.Now.AddHours(2),
                DeliveryTime = DateTime.Now.AddHours(4),
                PostedPrice = 600.0m,
                Weight = 1400
            };

            var loadStatusService = new LoadStatusService(
                _profileService,
                _dbContext,
                _mockDistanceCalculatorService,
                _mockLoadHelperService.Object,
                _mockHtmlSanitizerService.Object
            );

            var result = await loadStatusService.EditLoadAsync(nonExistentLoadId, updatedModel);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditLoadAsync_ShouldRecalculateDistance_WhenOriginOrDestinationChanges()
        {
            Guid loadId = Guid.NewGuid();

            var originalLoad = new Load
            {
                Id = loadId,
                OriginCity = "Sacramento",
                OriginState = "CA",
                DestinationCity = "Kent",
                DestinationState = "WA",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(3),
                Price = 500.0m,
                Weight = 1200,
                Distance = 800.0
            };

            await _dbContext.Loads.AddAsync(originalLoad);
            await _dbContext.SaveChangesAsync();

            var updatedModel = new LoadViewModel
            {
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                PickupTime = DateTime.Now.AddHours(2),
                DeliveryTime = DateTime.Now.AddHours(4),
                PostedPrice = 600.0m,
                Weight = 1400
            };

            var mockDistanceCalculator = new Mock<IDistanceCalculatorService>();
            var mockHtmlSanitizerService = new Mock<IHtmlSanitizerService>();
            var recalculatedDistance = 900.0;

            mockDistanceCalculator
                .Setup(x => x.GetDistanceBetweenCitiesAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(recalculatedDistance);


            mockHtmlSanitizerService.Setup(x => x.Sanitize(It.IsAny<string>()))
                .Returns((string input) => input);


            var loadStatusService = new LoadStatusService(
                _profileService,
                _dbContext,
                mockDistanceCalculator.Object,
                _mockLoadHelperService.Object,
                mockHtmlSanitizerService.Object
            );

            var result = await loadStatusService.EditLoadAsync(loadId, updatedModel);
            var updatedLoad = await _dbContext.Loads.FindAsync(loadId);

            Assert.That(result, Is.True);
            Assert.That(updatedLoad.Distance, Is.EqualTo(recalculatedDistance));
        }

        [Test]
        public async Task EditLoadAsync_ShouldReturnFalse_WhenNoChangesAreMade()
        {
            Guid loadId = Guid.NewGuid();

            var originalLoad = new Load
            {
                Id = loadId,
                OriginCity = "Sacramento",
                OriginState = "CA",
                DestinationCity = "Kent",
                DestinationState = "WA",
                PickupTime = DateTime.Now.AddHours(1),
                DeliveryTime = DateTime.Now.AddHours(3),
                Price = 500.0m,
                Weight = 1200,
                Distance = 800.0
            };

            await _dbContext.Loads.AddAsync(originalLoad);
            await _dbContext.SaveChangesAsync();

            var identicalModel = new LoadViewModel
            {
                OriginCity = "Sacramento",
                OriginState = "CA",
                DestinationCity = "Kent",
                DestinationState = "WA",
                PickupTime = originalLoad.PickupTime,
                DeliveryTime = originalLoad.DeliveryTime,
                PostedPrice = originalLoad.Price,
                Weight = originalLoad.Weight
            };


            var mockDistanceCalculatorService = new Mock<IDistanceCalculatorService>();
            var mockHtmlSanitizerService = new Mock<IHtmlSanitizerService>();
            var loadHelperService = new Mock<ILoadHelperService>();


            mockHtmlSanitizerService.Setup(x => x.Sanitize(It.IsAny<string>()))
                .Returns((string input) => input);

            mockDistanceCalculatorService.Setup(x =>
                    x.GetDistanceBetweenCitiesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .ReturnsAsync((string originCity, string originState, string destinationCity, string destinationState) =>
                {
                    return 100.0;
                });

            loadHelperService.Setup(s => s.FormatLocation(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string city, string state) => (city, state));

            var loadStatusService = new LoadStatusService(
                _profileService,
                _dbContext,
                mockDistanceCalculatorService.Object,
                loadHelperService.Object,
                mockHtmlSanitizerService.Object
            );


            var result = await loadStatusService.EditLoadAsync(loadId, identicalModel);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CancelLoadAsync_ShouldUpdateLoadStatusToCancelled_WhenLoadExists()
        {
            Guid loadId = Guid.NewGuid();
            Guid brokerId = Guid.NewGuid();

            var bookedLoad = new BookedLoad
            {
                LoadId = loadId,
                DispatcherId = Guid.NewGuid()
            };

            var existingLoad = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BrokerId = brokerId,
                Status = LoadStatus.Created,
                BookedLoad = bookedLoad
            };

            await _dbContext.Loads.AddAsync(existingLoad);
            await _dbContext.SaveChangesAsync();

            var mockLoadStatusService = new Mock<ILoadStatusService>();

            mockLoadStatusService.Setup(x => x.CancelLoadBookingAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));


            var result = await _mockLoadStatusService.CancelLoadAsync(loadId);
            var updatedLoad = await _dbContext.Loads.FirstOrDefaultAsync(l => l.Id == loadId);

            Assert.That(result, Is.True);
            Assert.That(updatedLoad, Is.Not.Null);
            Assert.That(updatedLoad.Status, Is.EqualTo(LoadStatus.Cancelled));
        }

        [Test]
        public async Task CancelLoadAsync_ShouldReturnFalse_WhenLoadDoesNotExist()
        {
            Guid nonExistentLoadId = Guid.NewGuid();

            var result = await _mockLoadStatusService.CancelLoadAsync(nonExistentLoadId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task PostLoadAsync_ShouldUpdateStatusToAvailableAndAddPostedLoad_WhenLoadExistsAndStatusIsCreated()
        {
            Guid loadId = Guid.NewGuid();

            var existingLoad = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Status = LoadStatus.Created,
                PostedLoad = null
            };

            await _dbContext.Loads.AddAsync(existingLoad);
            await _dbContext.SaveChangesAsync();

            var result = await _mockLoadStatusService.PostLoadAsync(loadId);
            var updatedLoad = await _dbContext.Loads
                .Include(l => l.PostedLoad)
                .FirstOrDefaultAsync(l => l.Id == loadId);

            Assert.That(result, Is.True);
            Assert.That(updatedLoad, Is.Not.Null);
            Assert.That(updatedLoad.Status, Is.EqualTo(LoadStatus.Available));
            Assert.That(updatedLoad.PostedLoad, Is.Not.Null);
            Assert.That(updatedLoad.PostedLoad.PostedDate, Is.EqualTo(DateTime.Now).Within(1).Seconds);
        }

        [Test]
        public async Task PostLoadAsync_ShouldReturnFalse_WhenLoadDoesNotExist()
        {
            Guid nonExistentLoadId = Guid.NewGuid();

            var result = await _mockLoadStatusService.PostLoadAsync(nonExistentLoadId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task PostLoadAsync_ShouldReturnFalse_WhenLoadStatusIsNotCreated()
        {
            Guid loadId = Guid.NewGuid();

            var existingLoad = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Status = LoadStatus.Booked
            };

            await _dbContext.Loads.AddAsync(existingLoad);
            await _dbContext.SaveChangesAsync();

            var result = await _mockLoadStatusService.PostLoadAsync(loadId);
            var unchangedLoad = await _dbContext.Loads.FirstOrDefaultAsync(l => l.Id == loadId);

            Assert.That(result, Is.False);
            Assert.That(unchangedLoad, Is.Not.Null);
            Assert.That(unchangedLoad.Status, Is.EqualTo(LoadStatus.Booked));
            Assert.That(unchangedLoad.PostedLoad, Is.Null);
        }

        [Test]
        public async Task UnpostLoadAsync_ShouldUnpostLoadAndRemovePostedLoad_WhenLoadExistsAndStatusIsAvailable()
        {
            Guid loadId = Guid.NewGuid();

            var existingLoad = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Status = LoadStatus.Available,
                PostedLoad = new PostedLoad
                {
                    LoadId = loadId,
                    PostedDate = DateTime.Now
                }
            };

            await _dbContext.Loads.AddAsync(existingLoad);
            await _dbContext.SaveChangesAsync();


            var result = await _mockLoadStatusService.UnpostLoadAsync(loadId);
            var updatedLoad = await _dbContext.Loads
                .Include(l => l.PostedLoad)
                .FirstOrDefaultAsync(l => l.Id == loadId);

            Assert.That(result, Is.True);
            Assert.That(updatedLoad, Is.Not.Null);
            Assert.That(updatedLoad.Status, Is.EqualTo(LoadStatus.Created));
            Assert.That(updatedLoad.PostedLoad, Is.Null);
        }

        [Test]
        public async Task UnpostLoadAsync_ShouldReturnFalse_WhenLoadDoesNotExist()
        {

            Guid nonExistentLoadId = Guid.NewGuid();

            var result = await _mockLoadStatusService.UnpostLoadAsync(nonExistentLoadId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UnpostLoadAsync_ShouldReturnFalse_WhenLoadStatusIsNotAvailable()
        {
            Guid loadId = Guid.NewGuid();
            var existingLoad = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Status = LoadStatus.Created,
                PostedLoad = null
            };

            await _dbContext.Loads.AddAsync(existingLoad);
            await _dbContext.SaveChangesAsync();


            var result = await _mockLoadStatusService.UnpostLoadAsync(loadId);
            var unchangedLoad = await _dbContext.Loads.FirstOrDefaultAsync(l => l.Id == loadId);

            Assert.That(result, Is.False);
            Assert.That(unchangedLoad, Is.Not.Null);
            Assert.That(unchangedLoad.Status, Is.EqualTo(LoadStatus.Created));
            Assert.That(unchangedLoad.PostedLoad, Is.Null);
        }

        [Test]
        public async Task UnpostAllLoadsAsync_ShouldUnpostAllLoadsForBroker_WhenLoadsExistAndStatusIsAvailable()
        {
            Guid brokerId = Guid.NewGuid();

            var loads = new List<Load>
            {
                new Load
                {

                    OriginCity = "Los Angeles",
                    OriginState = "CA",
                    DestinationCity = "Seattle",
                    DestinationState = "WA",
                    BrokerId = brokerId,
                    Status = LoadStatus.Available,
                    PostedLoad = new PostedLoad
                    {
                        LoadId = Guid.NewGuid(),
                        PostedDate = DateTime.Now
                    }
                },
                new Load
                {
                    OriginCity = "Indianapolis",
                    OriginState = "IN",
                    DestinationCity = "Kent",
                    DestinationState = "WA",
                    BrokerId = brokerId,
                    Status = LoadStatus.Available,
                    PostedLoad = new PostedLoad
                    {
                        LoadId = Guid.NewGuid(),
                        PostedDate = DateTime.Now
                    }
                }
            };

            await _dbContext.Loads.AddRangeAsync(loads);
            await _dbContext.SaveChangesAsync();


            var result = await _mockLoadStatusService.UnpostAllLoadsAsync(brokerId);
            var updatedLoads = await _dbContext.Loads
                .Where(l => l.BrokerId == brokerId)
                .ToListAsync();

            Assert.That(result, Is.True);

            foreach (var load in updatedLoads)
            {
                Assert.That(load.Status, Is.EqualTo(LoadStatus.Created));
                Assert.That(load.PostedLoad, Is.Null);
            }
        }

        [Test]
        public async Task UnpostAllLoadsAsync_ShouldReturnFalse_WhenNoLoadsExistForBroker()
        {
            Guid brokerId = Guid.NewGuid();

            var result = await _mockLoadStatusService.UnpostAllLoadsAsync(brokerId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UnpostAllLoadsAsync_ShouldReturnFalse_WhenNoAvailableLoadsForBroker()
        {
            // Arrange
            Guid brokerId = Guid.NewGuid();

            var loads = new List<Load>
            {
                new Load
                {
                    BrokerId = brokerId,
                    OriginCity = "Los Angeles",
                    OriginState = "CA",
                    DestinationCity = "Seattle",
                    DestinationState = "WA",
                    Status = LoadStatus.Created,
                    PostedLoad = new PostedLoad
                    {
                        LoadId = Guid.NewGuid(),
                        PostedDate = DateTime.Now
                    }
                }
            };

            await _dbContext.Loads.AddRangeAsync(loads);
            await _dbContext.SaveChangesAsync();


            var result = await _mockLoadStatusService.UnpostAllLoadsAsync(brokerId);
            var unchangedLoad = await _dbContext.Loads
                .FirstOrDefaultAsync(l => l.BrokerId == brokerId);

            Assert.That(result, Is.False);
            Assert.That(unchangedLoad, Is.Not.Null);
            Assert.That(unchangedLoad.Status, Is.EqualTo(LoadStatus.Created));
            Assert.That(unchangedLoad.PostedLoad, Is.Not.Null);
        }

        [Test]
        public async Task BookLoadAsync_ShouldReturnTrue_WhenLoadIsAvailable()
        {
            Guid loadId = Guid.NewGuid();
            Guid dispatcherId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Status = LoadStatus.Available,
                BrokerId = Guid.NewGuid(),
                PostedLoad = new PostedLoad
                {
                    LoadId = loadId,
                    PostedDate = DateTime.Now
                }
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _mockLoadStatusService.BookLoadAsync(loadId, dispatcherId);
            var updatedLoad = await _dbContext.Loads
                .Include(l => l.BookedLoad)
                .FirstOrDefaultAsync(l => l.Id == loadId);

            Assert.That(result, Is.True);
            Assert.That(updatedLoad.Status, Is.EqualTo(LoadStatus.Booked));
            Assert.That(updatedLoad.BookedLoad, Is.Not.Null);
            Assert.That(updatedLoad.BookedLoad.DispatcherId, Is.EqualTo(dispatcherId));
            Assert.That(updatedLoad.BookedLoad.DriverId, Is.Null);
        }

        [Test]
        public async Task BookLoadAsync_ShouldReturnFalse_WhenLoadIsNotAvailable()
        {
            Guid loadId = Guid.NewGuid();
            Guid dispatcherId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Status = LoadStatus.Created,
                BrokerId = Guid.NewGuid()
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();


            var result = await _mockLoadStatusService.BookLoadAsync(loadId, dispatcherId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task BookLoadAsync_ShouldReturnFalse_WhenLoadDoesNotExist()
        {
            Guid loadId = Guid.NewGuid();
            Guid dispatcherId = Guid.NewGuid();

            var result = await _mockLoadStatusService.BookLoadAsync(loadId, dispatcherId);

            Assert.That(result, Is.False);


        }

        [Test]
        public async Task CancelLoadBookingAsync_ShouldReturnTrue_WhenBrokerCancelsBooking()
        {
            Guid loadId = Guid.NewGuid();
            Guid brokerId = Guid.NewGuid();
            Guid dispatcherId = Guid.NewGuid();

            var driver = new Driver
            {
                FirstName = "Bruce",
                LastName = "wayne",
                LicenseNumber = "BAT666",
                DriverId = Guid.NewGuid(),
                IsBusy = true
            };

            var bookedLoad = new BookedLoad
            {
                LoadId = loadId,
                DispatcherId = dispatcherId,
                DriverId = driver.DriverId,
                Driver = driver,
                BookedDate = DateTime.UtcNow
            };

            var load = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BrokerId = brokerId,
                Status = LoadStatus.Booked,
                BookedLoad = bookedLoad
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();


            var result = await _mockLoadStatusService.CancelLoadBookingAsync(loadId, brokerId);
            var updatedLoad = await _dbContext.Loads
                .Include(l => l.BookedLoad)
                .FirstOrDefaultAsync(l => l.Id == loadId);

            Assert.That(result, Is.True);
            Assert.That(updatedLoad.Status, Is.EqualTo(LoadStatus.Available));
            Assert.That(updatedLoad.BookedLoad, Is.Null);
            Assert.That(driver.IsBusy, Is.False);
        }

        [Test]
        public async Task CancelLoadBookingAsync_ShouldThrowUnauthorizedAccessException_WhenBrokerDoesNotOwnLoad()
        {
            Guid loadId = Guid.NewGuid();
            Guid brokerId = Guid.NewGuid();
            Guid otherBrokerId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BrokerId = brokerId,
                Status = LoadStatus.Booked
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();


            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _mockLoadStatusService.CancelLoadBookingAsync(loadId, otherBrokerId));

            Assert.That(ex.Message, Is.EqualTo(NoPermissionToCancel)); // Ensure the correct exception message
        }

        [Test]
        public async Task CancelLoadBookingAsync_ShouldThrowInvalidOperationException_WhenLoadIsNotBooked()
        {
            Guid loadId = Guid.NewGuid();
            Guid brokerId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BrokerId = brokerId,
                Status = LoadStatus.Created
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();


            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _mockLoadStatusService.CancelLoadBookingAsync(loadId, brokerId));

            Assert.That(ex.Message, Is.EqualTo(LoadNotInBookedStatus));
        }

        [Test]
        public async Task CancelLoadBookingAsync_ShouldThrowException_WhenLoadIsNotBooked()
        {
            Guid loadId = Guid.NewGuid();
            Guid brokerId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BrokerId = brokerId,
                Status = LoadStatus.Created
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();


            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _mockLoadStatusService.CancelLoadBookingAsync(loadId, brokerId));
        }

        [Test]
        public async Task ReturnLoadBackToBroker_ShouldReturnTrue_WhenLoadIsReturnedSuccessfully()
        {
            var loadId = Guid.NewGuid();
            var brokerId = Guid.NewGuid();
            var dispatcherId = Guid.NewGuid();

            var bookedLoad = new BookedLoad
            {
                Id = Guid.NewGuid(),
                DispatcherId = dispatcherId,
                LoadId = loadId,
                BookedDate = DateTime.UtcNow
            };

            var load = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BrokerId = brokerId,
                Status = LoadStatus.Booked,
                BookedLoad = bookedLoad
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _mockLoadStatusService.ReturnLoadBackToBroker(loadId, dispatcherId);

            var updatedLoad = await _dbContext.Loads.FindAsync(loadId);

            Assert.That(result, Is.EqualTo(true));
            Assert.That(LoadStatus.Available, Is.EqualTo(updatedLoad.Status));
            Assert.That(updatedLoad.BookedLoad, Is.EqualTo(null));
        }

        [Test]
        public async Task ReturnLoadBackToBroker_ShouldThrowUnauthorizedAccessException_WhenUserIsNotDispatcher()
        {
            var loadId = Guid.NewGuid();
            var brokerId = Guid.NewGuid();
            var wrongDispatcherId = Guid.NewGuid();

            var bookedLoad = new BookedLoad
            {
                Id = Guid.NewGuid(),
                DispatcherId = Guid.NewGuid(),
                LoadId = loadId,
                BookedDate = DateTime.UtcNow
            };

            var load = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BrokerId = brokerId,
                Status = LoadStatus.Booked,
                BookedLoad = bookedLoad
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _mockLoadStatusService.ReturnLoadBackToBroker(loadId, wrongDispatcherId));
        }

        [Test]
        public async Task ReturnLoadBackToBroker_ShouldThrowException_WhenLoadNotFound()
        {
            Assert.ThrowsAsync<Exception>(async () =>
                await _mockLoadStatusService.ReturnLoadBackToBroker(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Test]
        public async Task ReturnLoadBackToBroker_ShouldThrowUnauthorizedAccessException_WhenLoadIsNotBooked()
        {
            var loadId = Guid.NewGuid();
            var brokerId = Guid.NewGuid();
            var dispatcherId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BrokerId = brokerId,
                Status = LoadStatus.Available,
                BookedLoad = null
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _mockLoadStatusService.ReturnLoadBackToBroker(loadId, dispatcherId));
        }

        [Test]
        public async Task ReturnLoadBackToBroker_ShouldReturnTrue_WhenLoadHasNoDriverAssigned()
        {
            var loadId = Guid.NewGuid();
            var brokerId = Guid.NewGuid();
            var dispatcherId = Guid.NewGuid();

            var bookedLoad = new BookedLoad
            {
                Id = Guid.NewGuid(),
                DispatcherId = dispatcherId,
                LoadId = loadId,
                BookedDate = DateTime.UtcNow,
                DriverId = null
            };

            var load = new Load
            {
                Id = loadId,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BrokerId = brokerId,
                Status = LoadStatus.Booked,
                BookedLoad = bookedLoad
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _mockLoadStatusService.ReturnLoadBackToBroker(loadId, dispatcherId);

            var updatedLoad = await _dbContext.Loads.FindAsync(loadId);

            Assert.That(result, Is.EqualTo(true));
            Assert.That(LoadStatus.Available, Is.EqualTo(updatedLoad.Status));
            Assert.That(updatedLoad.BookedLoad, Is.EqualTo(null));


        }

        [Test]
        public async Task LoadDeliveredAsync_ShouldReturnTrue_WhenLoadIsSuccessfullyMarkedAsDelivered()
        {
            var loadId = Guid.NewGuid();
            var driverId = Guid.NewGuid();
            var bookedLoadId = Guid.NewGuid();
            var dispatcherId = Guid.NewGuid();
            var brokerId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                Status = LoadStatus.Booked,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BookedLoad = new BookedLoad
                {
                    Id = bookedLoadId,
                    DriverId = driverId,
                    DispatcherId = dispatcherId,
                    LoadId = loadId,
                },
                BrokerId = brokerId
            };

            var driver = new Driver
            {
                FirstName = "Bruce",
                LastName = "Wayne",
                LicenseNumber = "BAT666",
                DriverId = driverId,
                IsBusy = true
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.Drivers.AddAsync(driver);
            await _dbContext.SaveChangesAsync();

            var result = await _mockLoadStatusService.LoadDeliveredAsync(loadId);

            Assert.That(result, Is.True);

            var updatedLoad = await _dbContext.Loads.FindAsync(loadId);
            Assert.That(updatedLoad.Status, Is.EqualTo(LoadStatus.Delivered));

            var updatedDriver = await _dbContext.Drivers.FindAsync(driverId);
            Assert.That(updatedDriver.IsBusy, Is.False);

            var deliveredLoad = await _dbContext.DeliveredLoads
                .FirstOrDefaultAsync(dl => dl.LoadId == loadId);
            Assert.That(deliveredLoad, Is.Not.Null);

            Assert.That(updatedLoad.OriginCity, Is.EqualTo("Los Angeles"));
            Assert.That(updatedLoad.OriginState, Is.EqualTo("CA"));
            Assert.That(updatedLoad.DestinationCity, Is.EqualTo("Seattle"));
            Assert.That(updatedLoad.DestinationState, Is.EqualTo("WA"));
        }

        [Test]
        public async Task LoadDeliveredAsync_ShouldReturnFalse_WhenLoadDoesNotExist()
        {
            var result = await _mockLoadStatusService.LoadDeliveredAsync(Guid.NewGuid());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task LoadDeliveredAsync_ShouldReturnFalse_WhenLoadStatusIsNotBooked()
        {
            var loadId = Guid.NewGuid();
            var load = new Load
            {
                Id = loadId,
                Status = LoadStatus.Available,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BookedLoad = new BookedLoad { Id = Guid.NewGuid() }
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _mockLoadStatusService.LoadDeliveredAsync(loadId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task LoadDeliveredAsync_ShouldReturnFalse_WhenBookedLoadIsMissing()
        {
            var loadId = Guid.NewGuid();
            var load = new Load
            {
                Id = loadId,
                Status = LoadStatus.Booked,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BookedLoad = null
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _mockLoadStatusService.LoadDeliveredAsync(loadId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task LoadDeliveredAsync_ShouldThrowArgumentException_WhenDriverIsNotAssigned()
        {
            var loadId = Guid.NewGuid();

            var load = new Load
            {
                Id = loadId,
                Status = LoadStatus.Booked,
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                BookedLoad = new BookedLoad
                {
                    Id = Guid.NewGuid(),
                    DriverId = null,
                    LoadId = loadId
                },
                BrokerId = Guid.NewGuid()
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            Assert.That(async () =>
                    await _mockLoadStatusService.LoadDeliveredAsync(loadId), Throws.ArgumentException.With.Message.EqualTo(CannotMarkLoadDeliveredWithoutADriver));
        }


    }

}

