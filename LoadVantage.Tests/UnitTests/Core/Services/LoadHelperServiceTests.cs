using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;

using Dispatcher = LoadVantage.Infrastructure.Data.Models.Dispatcher;
using LoadVantage.Common.Enums;

namespace LoadVantage.Tests.UnitTests.Core.Services
{
    public class LoadHelperServiceTests
    {
        private LoadVantageDbContext _dbContext;
        private ILoadHelperService _loadHelperService;


        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<LoadVantageDbContext>()
                .UseInMemoryDatabase(databaseName: "LoadVantageTestInMemoryDB")
                .Options;

            _dbContext = new LoadVantageDbContext(options);
            _loadHelperService = new LoadHelperService(_dbContext);

        }

		[TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetAllLoads_ShouldReturnAllLoads_WhenLoadsExist()
        {

            var broker = new Broker
            {
                Id = Guid.NewGuid(),
                FirstName = "Gosho",
                LastName = "The Broker",
                UserName = "goshko",
                Email = "goshko@brokers.com"
            };

            var driver = new Driver
            {
                DriverId = Guid.NewGuid(),
                FirstName = "Kolio",
                LastName = "The Driver",
                LicenseNumber = "Kolio666"
            };

            var dispatcher = new Dispatcher
            {
                Id = Guid.NewGuid(),
                FirstName = "Pesho",
                LastName = "The Dispatcher",
                UserName = "pesho_the_best",
                Email = "pesho@awesome-dispatcher.com"
            };

            var truck = new Truck
            {
                Id = Guid.NewGuid(),
                TruckNumber = "TRK1",
                Make = "Freighliner",
                Model = "Cascadia",
                Year = 2024,
                DriverId = driver.DriverId,
            };

            var load = new Load
            {
                Id = Guid.NewGuid(),
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Broker = broker,
                PostedLoad = new PostedLoad { Id = Guid.NewGuid() },
                BookedLoad = new BookedLoad
                {
                    Id = Guid.NewGuid(),
                    DriverId = driver.DriverId,
                    DispatcherId = dispatcher.Id,
                    Driver = driver,
                    Dispatcher = dispatcher
                },
                DeliveredLoad = new DeliveredLoad { Id = Guid.NewGuid() }
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.Brokers.AddAsync(broker);
            await _dbContext.Drivers.AddAsync(driver);
            await _dbContext.Dispatchers.AddAsync(dispatcher);
            await _dbContext.Trucks.AddAsync(truck);
            await _dbContext.SaveChangesAsync();

            var result = await _loadHelperService.GetAllLoads();


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));

            var returnedLoad = result.First();

            Assert.That(returnedLoad.Broker.FirstName, Is.EqualTo(broker.FirstName));
            Assert.That(returnedLoad.Broker.LastName, Is.EqualTo(broker.LastName));

            Assert.That(returnedLoad.PostedLoad, Is.Not.Null);
            Assert.That(returnedLoad.BookedLoad, Is.Not.Null);

            Assert.That(returnedLoad.BookedLoad.Driver.FirstName, Is.EqualTo(driver.FirstName));
            Assert.That(returnedLoad.BookedLoad.Driver.LastName, Is.EqualTo(driver.LastName));

            Assert.That(returnedLoad.BookedLoad.Dispatcher.FirstName, Is.EqualTo(dispatcher.FirstName));
            Assert.That(returnedLoad.BookedLoad.Dispatcher.LastName, Is.EqualTo(dispatcher.LastName));

            Assert.That(returnedLoad.DeliveredLoad, Is.Not.Null);
        }

        [Test]
        public async Task GetAllLoads_ShouldReturnEmptyList_WhenNoLoadsExist()
        {
            var result = await _loadHelperService.GetAllLoads();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllLoads_ShouldIncludeAllRelatedEntities()
        {
            var broker = new Broker
            {
                Id = Guid.NewGuid(),
                FirstName = "Gosho",
                LastName = "The Broker",
                UserName = "goshko",
                Email = "goshko@brokers.com"
            };

            var driver = new Driver
            {
                DriverId = Guid.NewGuid(),
                FirstName = "Kolio",
                LastName = "The Driver",
                LicenseNumber = "Kolio666"
            };

            var dispatcher = new Dispatcher
            {
                Id = Guid.NewGuid(),
                FirstName = "Pesho",
                LastName = "The Dispatcher",
                UserName = "pesho_the_best",
                Email = "pesho@awesome-dispatcher.com"
            };

            var truck = new Truck
            {
                Id = Guid.NewGuid(),
                TruckNumber = "TRK1",
                Make = "Freighliner",
                Model = "Cascadia",
                Year = 2024,
                DriverId = driver.DriverId,
            };

            var load = new Load
            {
                Id = Guid.NewGuid(),
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Broker = broker,
                BrokerId = broker.Id,
                PostedLoad = new PostedLoad { Id = Guid.NewGuid() },
                BookedLoad = new BookedLoad
                {
                    Id = Guid.NewGuid(),
                    DriverId = driver.DriverId,
                    DispatcherId = dispatcher.Id,
                    Driver = driver,
                    Dispatcher = dispatcher
                },
                DeliveredLoad = new DeliveredLoad { Id = Guid.NewGuid() }
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.Brokers.AddAsync(broker);
            await _dbContext.Drivers.AddAsync(driver);
            await _dbContext.Dispatchers.AddAsync(dispatcher);
            await _dbContext.Trucks.AddAsync(truck);
            await _dbContext.SaveChangesAsync();

            var result = await _loadHelperService.GetAllLoads();

            var returnedLoad = result.First();

            Assert.That(returnedLoad.Broker.FirstName, Is.EqualTo(broker.FirstName));
            Assert.That(returnedLoad.Broker.LastName, Is.EqualTo(broker.LastName));
            Assert.That(returnedLoad.Broker.Id, Is.EqualTo(broker.Id)); Assert.That(returnedLoad.PostedLoad, Is.Not.Null);
            Assert.That(returnedLoad.BookedLoad, Is.Not.Null);
            Assert.That(returnedLoad.BookedLoad.Driver.FirstName, Is.EqualTo(driver.FirstName));
            Assert.That(returnedLoad.BookedLoad.Driver.LastName, Is.EqualTo(driver.LastName));
            Assert.That(returnedLoad.BookedLoad.Dispatcher.FirstName, Is.EqualTo(dispatcher.FirstName));
            Assert.That(returnedLoad.BookedLoad.Dispatcher.LastName, Is.EqualTo(dispatcher.LastName));
            Assert.That(returnedLoad.DeliveredLoad, Is.Not.Null);
        }

        [Test]
        public async Task GetAllLoadCountsAsync_ShouldReturnCorrectCount_WhenLoadsExist()
        {
            var load1 = new Load
            {
                Id = Guid.NewGuid(),
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
            };
            var load2 = new Load
            {
                Id = Guid.NewGuid(),
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Kent",
                DestinationState = "WA",
            };

            await _dbContext.Loads.AddAsync(load1);
            await _dbContext.Loads.AddAsync(load2);
            await _dbContext.SaveChangesAsync();

            var result = await _loadHelperService.GetAllLoadCountsAsync();

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllLoadCountsAsync_ShouldReturnZero_WhenNoLoadsExist()
        {
            var result = await _loadHelperService.GetAllLoadCountsAsync();

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task CanUserViewLoadAsync_ShouldReturnTrue_WhenUserIsBrokerAndOwnsLoad()
        {
            var brokerId = Guid.NewGuid();

            var load = new Load
            {
                Id = Guid.NewGuid(),
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Status = LoadStatus.Booked,
                BrokerId = brokerId
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _loadHelperService.CanUserViewLoadAsync(brokerId, load.Id);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CanUserViewLoadAsync_ShouldReturnTrue_WhenUserIsDispatcherAndLoadIsBooked()
        {
            var dispatcherId = Guid.NewGuid();

            var load = new Load
            {
                Id = Guid.NewGuid(),
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Status = LoadStatus.Booked,
                BookedLoad = new BookedLoad
                {
                    DispatcherId = dispatcherId
                }
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _loadHelperService.CanUserViewLoadAsync(dispatcherId, load.Id);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CanUserViewLoadAsync_ShouldReturnFalse_WhenLoadIsCancelled()
        {
            var dispatcherId = Guid.NewGuid();

            var load = new Load
            {
                Id = Guid.NewGuid(),
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Status = LoadStatus.Cancelled,
                BookedLoad = new BookedLoad
                {
                    DispatcherId = dispatcherId
                }
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _loadHelperService.CanUserViewLoadAsync(dispatcherId, load.Id);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CanUserViewLoadAsync_ShouldReturnTrue_WhenLoadIsAvailable()
        {
            var userId = Guid.NewGuid();
            var load = new Load
            {
                Id = Guid.NewGuid(),
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Status = LoadStatus.Available,
                BrokerId = Guid.NewGuid()
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _loadHelperService.CanUserViewLoadAsync(userId, load.Id);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CanUserViewLoadAsync_ShouldReturnFalse_WhenUserIsNeitherBrokerNorDispatcher()
        {
            var brokerId = Guid.NewGuid();
            var dispatcherId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var load = new Load
            {
                Id = Guid.NewGuid(),
                OriginCity = "Los Angeles",
                OriginState = "CA",
                DestinationCity = "Seattle",
                DestinationState = "WA",
                Status = LoadStatus.Booked,
                BrokerId = brokerId,
                BookedLoad = new BookedLoad
                {
                    DispatcherId = dispatcherId
                }
            };

            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();

            var result = await _loadHelperService.CanUserViewLoadAsync(userId, load.Id);

            Assert.That(result, Is.False);
        }

        [TestCase("new york", "ny")]
        [TestCase("san francisco", "ca")]
        [TestCase("   los angeles", "   ca   ")]  
        [TestCase("chicago", "il")]
        [TestCase("boston", "ma")]
		public void FormatLocation_ShouldReturnFormattedLocation(string city, string state)
        {
	        var result = _loadHelperService.FormatLocation(city, state);

	        Assert.That(result.FormattedCity, Is.EqualTo(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(city.Trim().ToLower())));
	        Assert.That(result.FormattedState, Is.EqualTo(state.Trim().ToUpper()));
        }

		[Test]
		public async Task CanUserViewLoadAsync_ShouldReturnFalse_WhenLoadIsNull()
		{
			Guid userId = Guid.NewGuid();
			Guid loadId = Guid.NewGuid();

			var result = await _loadHelperService.CanUserViewLoadAsync(userId, loadId);

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task CanUserViewLoadAsync_ShouldReturnTrue_WhenUserIsBroker()
		{
			Guid userId = Guid.NewGuid();
			Guid loadId = Guid.NewGuid();

			var load = new Load 
				{ 
					Id = loadId, 
                    OriginCity = "Sacramento",
                    OriginState = "CA",
                    DestinationCity = "Chicago",
                    DestinationState = "IL",
					Status = LoadStatus.Available, 
					BrokerId = userId
				};

			await _dbContext.Loads.AddAsync(load);
			await _dbContext.SaveChangesAsync();

			var result = await _loadHelperService.CanUserViewLoadAsync(userId, loadId);

			Assert.That(result, Is.True);
		}

		[Test]
		public async Task CanUserViewLoadAsync_ShouldReturnTrue_WhenUserIsDispatcherAndLoadIsBookedOrDelivered()
		{
			Guid userId = Guid.NewGuid();
			Guid loadId = Guid.NewGuid();

			var load = new Load
			{
				Id = loadId,
				OriginCity = "Sacramento",
				OriginState = "CA",
				DestinationCity = "Chicago",
				DestinationState = "IL",
				Status = LoadStatus.Booked,
				BookedLoad = new BookedLoad { DispatcherId = userId }
			};

			await _dbContext.Loads.AddAsync(load);
			await _dbContext.SaveChangesAsync();

			var result = await _loadHelperService.CanUserViewLoadAsync(userId, loadId);

			Assert.That(result, Is.True);
		}

		[Test]
		public async Task CanUserViewLoadAsync_ShouldReturnFalse_WhenUserIsDispatcherAndLoadIsAvailable()
		{
			Guid userId = Guid.NewGuid();
			Guid loadId = Guid.NewGuid();

			var load = new Load
			{
				Id = loadId,
				OriginCity = "Sacramento",
				OriginState = "CA",
				DestinationCity = "Chicago",
				DestinationState = "IL",
				Status = LoadStatus.Available,
				BookedLoad = new BookedLoad { DispatcherId = userId }
			};

			await _dbContext.Loads.AddAsync(load);
			await _dbContext.SaveChangesAsync();

			var result = await _loadHelperService.CanUserViewLoadAsync(userId, loadId);

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task CanUserViewLoadAsync_ShouldReturnTrue_WhenLoadIsAvailableAndUserIsNotBrokerOrDispatcher()
		{
			Guid userId = Guid.NewGuid();
			Guid loadId = Guid.NewGuid();

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

			var result = await _loadHelperService.CanUserViewLoadAsync(userId, loadId);

			Assert.That(result, Is.True);
		}

		[Test]
		public void CreateBrokerInfo_ShouldReturnNullValues_WhenBrokerIdIsEmpty()
		{
			var load = new Load
			{
				BrokerId = Guid.Empty,
				OriginCity = "Sacramento",
				OriginState = "CA",
				DestinationCity = "Chicago",
				DestinationState = "IL",
				Broker = null 
			};

			var result = _loadHelperService.CreateBrokerInfo(load);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.BrokerName, Is.Null);
			Assert.That(result.BrokerEmail, Is.Null);
			Assert.That(result.BrokerPhone, Is.Null);
		}

		[Test]
		public void CreateBrokerInfo_ShouldReturnCorrectBrokerInfo_WhenBrokerIdIsValid()
		{
			var broker = new Broker
			{
                FirstName = "Bruce",
                LastName = "Wayne",
				Email = "batman@gotham.com",
                UserName = "batman",
				PhoneNumber = "123-456-7890"
			};

			var load = new Load
			{
				BrokerId = Guid.NewGuid(),
				OriginCity = "Sacramento",
				OriginState = "CA",
				DestinationCity = "Chicago",
				DestinationState = "IL",
				Broker = broker
			};

			var result = _loadHelperService.CreateBrokerInfo(load);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.BrokerName, Is.EqualTo("Bruce Wayne"));
			Assert.That(result.BrokerEmail, Is.EqualTo("batman@gotham.com"));
			Assert.That(result.BrokerPhone, Is.EqualTo("123-456-7890"));
		}

		[Test]
		public void CreateDispatcherInfo_ShouldReturnNullValues_WhenDispatcherIsNull()
		{
			var bookedLoad = new BookedLoad
			{
				Dispatcher = null 
			};

			var result = _loadHelperService.CreateDispatcherInfo(bookedLoad);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.DispatcherName, Is.Null);
			Assert.That(result.DispatcherEmail, Is.Null);
			Assert.That(result.DispatcherPhone, Is.Null);
		}

		[Test]
		public void CreateDispatcherInfo_ShouldReturnCorrectDispatcherInfo_WhenDispatcherIsValid()
		{
			var dispatcher = new Dispatcher
			{
				FirstName = "Bruce",
				LastName = "Wayne",
				Email = "batman@gotham.com",
				UserName = "batman",
				PhoneNumber = "987-654-3210"
			};

			var bookedLoad = new BookedLoad
			{
				Dispatcher = dispatcher
			};

			var result = _loadHelperService.CreateDispatcherInfo(bookedLoad);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.DispatcherName, Is.EqualTo("Bruce Wayne"));
			Assert.That(result.DispatcherEmail, Is.EqualTo("batman@gotham.com"));
			Assert.That(result.DispatcherPhone, Is.EqualTo("987-654-3210"));
		}

		[Test]
		public void CreateDriverInfo_ShouldReturnNullValues_WhenDriverIsNull()
		{
			var bookedLoad = new BookedLoad
			{
				Driver = null // Driver is null
			};

			var result = _loadHelperService.CreateDriverInfo(bookedLoad);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.DriverName, Is.Null);
			Assert.That(result.DriverLicenseNumber, Is.Null);
		}

		[Test]
		public void CreateDriverInfo_ShouldReturnCorrectDriverInfo_WhenDriverIsValid()
		{
			var driver = new Driver
			{
				FirstName = "John",
                LastName ="Doe",
				LicenseNumber = "ABC123456"
			};

			var bookedLoad = new BookedLoad
			{
				Driver = driver
			};

			var result = _loadHelperService.CreateDriverInfo(bookedLoad);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.DriverName, Is.EqualTo("John Doe"));
			Assert.That(result.DriverLicenseNumber, Is.EqualTo("ABC123456"));
		}

	}
}
