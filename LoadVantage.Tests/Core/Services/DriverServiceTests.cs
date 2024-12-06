using Microsoft.EntityFrameworkCore;

using Moq;
using NUnit.Framework;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Core.Models.Driver;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using LoadVantage.Common.Enums;


namespace LoadVantage.Tests.Core.Services
{
	public class DriverServiceTests
	{
		private LoadVantageDbContext _dbContext;
		private Mock<IProfileService> _profileService;
		private Mock<IUserService> _userService;
		private Mock<IHtmlSanitizerService> _htmlSanitizerService;
		private DriverService _driverService;


		[SetUp]
		public void SetUp()
		{
			var options = new DbContextOptionsBuilder<LoadVantageDbContext>()
				.UseInMemoryDatabase(databaseName: "LoadVantageTestInMemoryDB")
				.Options;

			_dbContext = new LoadVantageDbContext(options);
			_profileService = new Mock<IProfileService>();
			_userService = new Mock<IUserService>();
			_htmlSanitizerService = new Mock<IHtmlSanitizerService>();

			_driverService = new DriverService(_dbContext, _profileService.Object, _userService.Object, _htmlSanitizerService.Object);
		}

		[TearDown]
		public void TearDown()
		{
			_dbContext.Database.EnsureDeleted();
			_dbContext.Dispose();
		}

		[Test]
		public async Task GetAllDriversAsync_ShouldReturnDriversViewModel_WhenCalledWithValidUserId()
		{
			var userId = Guid.NewGuid();
			var user = new User
			{
				Id = userId, 
				UserName = "testUser"
			};

			_userService.Setup(us => us.GetUserByIdAsync(userId)).ReturnsAsync(user);
			_profileService.Setup(ps => ps.GetUserInformation(userId)).ReturnsAsync(new ProfileViewModel());

			await _dbContext.Drivers.AddAsync(new Driver
			{
				DriverId = Guid.NewGuid(),
				FirstName = "John",
				LastName = "Doe",
				LicenseNumber = "123456",
				Truck = new Truck
				{
					TruckNumber = "T123",
					Make = "Freightliner",
					Model = "Cascadia",
					Year = 2024,
					
				},
				IsAvailable = true,
				IsBusy = false,
				DispatcherId = userId
			});

			await _dbContext.SaveChangesAsync();

			var result = await _driverService.GetAllDriversAsync(userId);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Drivers, Is.Not.Empty);
			Assert.That(result.Drivers.Count, Is.EqualTo(1));  
			Assert.That(result.Profile, Is.InstanceOf<ProfileViewModel>());
			Assert.That(result.Drivers[0].FirstName, Is.EqualTo("John"));
			Assert.That(result.Drivers[0].TruckNumber, Is.EqualTo("T123"));
			Assert.That(result.Drivers[0].IsAvailable, Is.True);
			Assert.That(result.Drivers[0].IsBusy, Is.False);
		}

		[Test]
		public async Task GetAllDriversAsync_ShouldReturnEmptyList_WhenNoDriversMatchDispatcherId()
		{
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, UserName = "batman" };

			_userService.Setup(us => us.GetUserByIdAsync(userId)).ReturnsAsync(user);
			_profileService.Setup(ps => ps.GetUserInformation(userId)).ReturnsAsync(new ProfileViewModel());

			var result = await _driverService.GetAllDriversAsync(userId);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Drivers, Is.Empty);
		}

		[Test]
		public async Task GetDriverByIdAsync_ShouldReturnDriverViewModel_WhenDriverFound()
		{
			var userId = Guid.NewGuid();
			var driverId = Guid.NewGuid();

			_userService.Setup(us => us.GetCurrentUserAsync()).ReturnsAsync(new User { Id = userId });

			var driver = new Driver
			{
				DriverId = driverId,
				FirstName = "John",
				LastName = "Doe",
				LicenseNumber = "12345",
				Truck = new Truck
				{
					TruckNumber = "T123",
					Make = "Freightliner",
					Model = "Cascadia",
					Year = 2024
				},
				IsAvailable = true,
				IsBusy = false,
				IsFired = false,
				DispatcherId = userId
			};

			await _dbContext.Drivers.AddAsync(driver);
			await _dbContext.SaveChangesAsync();

			var result = await _driverService.GetDriverByIdAsync(driverId);

			Assert.That(result, Is.Not.Null);
			Assert.That(result!.Id, Is.EqualTo(driverId));
			Assert.That(result.FirstName, Is.EqualTo("John"));
			Assert.That(result.LastName, Is.EqualTo("Doe"));
			Assert.That(result.LicenseNumber, Is.EqualTo("12345"));
			Assert.That(result.TruckNumber, Is.EqualTo("T123"));
			Assert.That(result.IsAvailable, Is.EqualTo(true));
			Assert.That(result.IsBusy, Is.EqualTo(false));
		}

		[Test]
		public async Task GetDriverByIdAsync_ShouldReturnNull_WhenDriverNotFound()
		{
			var userId = Guid.NewGuid();
			var driverId = Guid.NewGuid();

			_userService.Setup(us => us.GetCurrentUserAsync()).ReturnsAsync(new User { Id = userId });

			var result = await _driverService.GetDriverByIdAsync(driverId);

			Assert.That(result, Is.Null); 
		}

		[Test]
		public async Task GetDriverByIdAsync_ShouldReturnNull_WhenDriverIsFired()
		{
			var userId = Guid.NewGuid();
			var driverId = Guid.NewGuid();

			_userService.Setup(us => us.GetCurrentUserAsync()).ReturnsAsync(new User { Id = userId });

			var driver = new Driver
			{
				DriverId = driverId,
				FirstName = "John",
				LastName = "Doe",
				LicenseNumber = "12345",
				Truck = new Truck 
				{
					TruckNumber = "T123",
					Make = "Freightliner",
					Model = "Cascadia",
					Year = 2024,
				},
				IsAvailable = true,
				IsBusy = false,
				IsFired = true, 
				DispatcherId = userId
			};

			await _dbContext.Drivers.AddAsync(driver);
			await _dbContext.SaveChangesAsync();

		
			var result = await _driverService.GetDriverByIdAsync(driverId);

			Assert.That(result, Is.Null); 
		}

		[Test]
		public async Task GetDriverByIdAsync_ShouldReturnNull_WhenUserIsNotDispatcher()
		{
			var userId = Guid.NewGuid();
			var driverId = Guid.NewGuid();

			_userService.Setup(us => us.GetCurrentUserAsync()).ReturnsAsync(new User { Id = userId });

			var driver = new Driver
			{
				DriverId = driverId,
				FirstName = "John",
				LastName = "Doe",
				LicenseNumber = "12345",
				Truck = new Truck
				{
					TruckNumber = "T123",
					Make = "Freightliner",
					Model = "Cascadia",
					Year = 2024
				},
				IsAvailable = true,
				IsBusy = false,
				IsFired = false,
				DispatcherId = Guid.NewGuid() 
			};

			await _dbContext.Drivers.AddAsync(driver);
			await _dbContext.SaveChangesAsync();


			var result = await _driverService.GetDriverByIdAsync(driverId);

			Assert.That(result, Is.Null); 
		}

		[Test]
		public async Task AddDriverAsync_ShouldAddDriver_WhenValidInput()
		{
			var userId = Guid.NewGuid();

			var driverViewModel = new DriverViewModel
			{
				Id = Guid.NewGuid(),
				FirstName = "John",
				LastName = "Doe",
				LicenseNumber = "12345"
			};

			_htmlSanitizerService.Setup(h => h.Sanitize(It.IsAny<string>())).Returns<string>(input => input);

			await _driverService.AddDriverAsync(driverViewModel, userId);

			var driver = await _dbContext.Drivers.FirstOrDefaultAsync();

			Assert.That(driver, Is.Not.Null);
			Assert.That(driver!.DriverId, Is.EqualTo(driverViewModel.Id));
			Assert.That(driver.FirstName, Is.EqualTo(driverViewModel.FirstName));
			Assert.That(driver.LastName, Is.EqualTo(driverViewModel.LastName));
			Assert.That(driver.LicenseNumber, Is.EqualTo(driverViewModel.LicenseNumber));
			Assert.That(driver.DispatcherId, Is.EqualTo(userId));
			Assert.That(driver.IsAvailable, Is.True);
			Assert.That(driver.IsFired, Is.False);
			Assert.That(driver.IsBusy, Is.False);

			_htmlSanitizerService.Verify(s => s.Sanitize(driverViewModel.FirstName), Times.Once);
			_htmlSanitizerService.Verify(s => s.Sanitize(driverViewModel.LastName), Times.Once);
			_htmlSanitizerService.Verify(s => s.Sanitize(driverViewModel.LicenseNumber), Times.Once);
		}

		[Test]
		public async Task UpdateDriverAsync_ShouldUpdateDriver_WhenValidModelIsProvided()
		{
			var driverId = Guid.NewGuid();

			var existingDriver = new Driver
			{
				DriverId = driverId,
				FirstName = "Jane",
				LastName = "Smith",
				LicenseNumber = "7891011",
				DispatcherId = Guid.NewGuid(),
				IsAvailable = true,
				IsFired = false,
				IsBusy = false
			};

			await _dbContext.Drivers.AddAsync(existingDriver);
			await _dbContext.SaveChangesAsync();

			var updatedDriverViewModel = new DriverViewModel
			{
				Id = driverId,
				FirstName = "John",
				LastName = "Doe",
				LicenseNumber = "123456"
			};

			_htmlSanitizerService
				.Setup(s => s.Sanitize(It.IsAny<string>()))
				.Returns((string input) => input);

			await _driverService.UpdateDriverAsync(updatedDriverViewModel);

			var updatedDriver = await _dbContext.Drivers.FindAsync(driverId);
			Assert.That(updatedDriver, Is.Not.Null);
			Assert.That(updatedDriver!.FirstName, Is.EqualTo(updatedDriverViewModel.FirstName));
			Assert.That(updatedDriver.LastName, Is.EqualTo(updatedDriverViewModel.LastName));
			Assert.That(updatedDriver.LicenseNumber, Is.EqualTo(updatedDriverViewModel.LicenseNumber));

			_htmlSanitizerService.Verify(s => s.Sanitize(updatedDriverViewModel.FirstName), Times.Once);
			_htmlSanitizerService.Verify(s => s.Sanitize(updatedDriverViewModel.LastName), Times.Once);
			_htmlSanitizerService.Verify(s => s.Sanitize(updatedDriverViewModel.LicenseNumber), Times.Once);
		}

		[Test]
		public void UpdateDriverAsync_ShouldThrowKeyNotFoundException_WhenDriverDoesNotExist()
		{
			var nonExistentDriverViewModel = new DriverViewModel
			{
				Id = Guid.NewGuid(),
				FirstName = "Non",
				LastName = "Existent",
				LicenseNumber = "zero000000"
			};

			var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => _driverService.UpdateDriverAsync(nonExistentDriverViewModel));
			Assert.That(exception!.Message, Is.EqualTo(DriverWasNotFound));
		}

		[Test]
		public async Task FireDriverAsync_ShouldUpdateDriverToFired_WhenDriverExists()
		{
			var driverId = Guid.NewGuid();

			var existingDriver = new Driver
			{
				DriverId = driverId,
				FirstName = "John",
				LastName = "Doe",
				LicenseNumber = "BGH68243",
				IsAvailable = true,
				IsBusy = true,
				IsFired = false,
				DispatcherId = Guid.NewGuid()
			};

			await _dbContext.Drivers.AddAsync(existingDriver);
			await _dbContext.SaveChangesAsync();

			await _driverService.FireDriverAsync(driverId);

			var updatedDriver = await _dbContext.Drivers.FindAsync(driverId);
			Assert.That(updatedDriver, Is.Not.Null);
			Assert.That(updatedDriver!.IsFired, Is.True);
			Assert.That(updatedDriver.IsAvailable, Is.False);
			Assert.That(updatedDriver.IsBusy, Is.False);
		}

		[Test]
		public void FireDriverAsync_ShouldThrowKeyNotFoundException_WhenDriverDoesNotExist()
		{
			var nonExistentDriverId = Guid.NewGuid();

			var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => _driverService.FireDriverAsync(nonExistentDriverId));
			Assert.That(exception!.Message, Is.EqualTo(DriverWasNotFound));
		}

		[Test]
		public async Task GetAvailableDriversAsync_ShouldReturnOnlyAvailableDrivers_WhenDriversExist()
		{

			var dispatcherId = Guid.NewGuid();
			var currentUser = new User { Id = dispatcherId };

			_userService
				.Setup(u => u.GetCurrentUserAsync())
				.ReturnsAsync(currentUser);

			var drivers = new List<Driver>
			{
				new Driver
				{
					DriverId = Guid.NewGuid(),
					FirstName = "Alice",
					LastName = "Jackson",
					LicenseNumber = "DAF44646",
					IsAvailable = true,
					IsBusy = false,
					IsFired = false,
					DispatcherId = dispatcherId
				},
				new Driver
				{
					DriverId = Guid.NewGuid(),
					FirstName = "Bob",
					LastName = "Jackson",
					LicenseNumber = "DAF44648",
					IsAvailable = false,
					IsBusy = false,
					IsFired = false,
					DispatcherId = dispatcherId
				},
				new Driver
				{
					DriverId = Guid.NewGuid(),
					FirstName = "Charlie",
					LastName = "Jackson",
					LicenseNumber = "DAF44647",
					IsAvailable = true,
					IsBusy = true,
					IsFired = false,
					DispatcherId = dispatcherId
				}
			};

			await _dbContext.Drivers.AddRangeAsync(drivers);
			await _dbContext.SaveChangesAsync();

			var result = await _driverService.GetAvailableDriversAsync();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[0].FirstName, Is.EqualTo("Alice"));
			Assert.That(result[0].IsAvailable, Is.True);
			Assert.That(result[0].IsBusy, Is.False);
			Assert.That(result[0].IsFired, Is.False);
			Assert.That(result[0].DispatcherId, Is.EqualTo(dispatcherId));
		}

		[Test]
		public async Task GetAvailableDriversAsync_ShouldReturnEmptyList_WhenNoAvailableDriversExist()
		{
			var dispatcherId = Guid.NewGuid();

			var currentUser = new User { Id = dispatcherId };

			_userService
				.Setup(u => u.GetCurrentUserAsync())
				.ReturnsAsync(currentUser);

			var drivers = new List<Driver>
			{
				new Driver
				{
					DriverId = Guid.NewGuid(),
					FirstName = "Alice",
					LastName = "Jackson",
					LicenseNumber = "DAF44646",
					IsAvailable = false,
					IsBusy = true,
					IsFired = false,
					DispatcherId = dispatcherId
				},
				new Driver
				{
					DriverId = Guid.NewGuid(),
					FirstName = "Bob",
					LastName = "Jackson",
					LicenseNumber = "DAF44646",
					IsAvailable = true,
					IsBusy = true,
					IsFired = false,
					DispatcherId = dispatcherId
				},
				new Driver
				{
					DriverId = Guid.NewGuid(),
					FirstName = "Charlie",
					LastName = "Jackson",
					LicenseNumber = "DAF44646",
					IsAvailable = true,
					IsBusy = false,
					IsFired = true,
					DispatcherId = dispatcherId
				}
			};

			await _dbContext.Drivers.AddRangeAsync(drivers);
			await _dbContext.SaveChangesAsync();

			var result = await _driverService.GetAvailableDriversAsync();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(0));
		}

		[Test]
		public async Task AssignADriverToLoadAsync_ShouldReturnFalse_WhenLoadDoesNotExist()
		{
			var nonExistentLoadId = Guid.NewGuid();
			var driverId = Guid.NewGuid();
			var userId = Guid.NewGuid();

			var result = await _driverService.AssignADriverToLoadAsync(nonExistentLoadId, driverId, userId);

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task AssignADriverToLoadAsync_ShouldReturnFalse_WhenDriverDoesNotExist()
		{
			var load = new Load
			{
				OriginCity = "Boardman",
				OriginState = "OH",
				DestinationCity = "Chicago",
				DestinationState = "IL",
				Id = Guid.NewGuid(),
				Status = LoadStatus.Booked
			};

			await _dbContext.Loads.AddAsync(load);
			await _dbContext.SaveChangesAsync();

			var nonExistentDriverId = Guid.NewGuid();
			var userId = Guid.NewGuid();

			var result = await _driverService.AssignADriverToLoadAsync(load.Id, nonExistentDriverId, userId);

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task AssignADriverToLoadAsync_ShouldAssignDriverToLoad_WhenLoadAndDriverExist()
		{
			var userId = Guid.NewGuid();

			var driver = new Driver
			{
				FirstName = "Vasko",
				LastName = "Jabata",
				LicenseNumber = "THG6985",
				DriverId = Guid.NewGuid(),
				DispatcherId = userId,
				TruckId = Guid.NewGuid(),
				IsBusy = false,
				IsFired = false
			};

			var load = new Load
			{
				Id = Guid.NewGuid(),
				OriginCity = "Boardman",
				OriginState = "OH",
				DestinationCity = "Chicago",
				DestinationState = "IL",
				Status = LoadStatus.Booked,
				BookedLoad = new BookedLoad
				{
					DispatcherId = userId
				}
			};

			await _dbContext.Drivers.AddAsync(driver);
			await _dbContext.Loads.AddAsync(load);
			await _dbContext.SaveChangesAsync();

			var result = await _driverService.AssignADriverToLoadAsync(load.Id, driver.DriverId, userId);

			Assert.That(result, Is.True);
			Assert.That(load.BookedLoad!.DriverId, Is.EqualTo(driver.DriverId));
			Assert.That(driver.IsBusy, Is.True);
		}

		[Test]
		public async Task AssignADriverToLoadAsync_ShouldReassignDriver_WhenLoadAlreadyHasDriver()
		{
			var userId = Guid.NewGuid();

			var existingDriver = new Driver
			{
				FirstName = "Vasko",
				LastName = "Jabata",
				LicenseNumber = "THG6985",
				DriverId = Guid.NewGuid(),
				DispatcherId = userId,
				TruckId = Guid.NewGuid(),
				IsBusy = true,
				IsFired = false
			};

			var newDriver = new Driver
			{
				FirstName = "Ceca",
				LastName = "Mecata",
				LicenseNumber = "ciganiq123456",
				DriverId = Guid.NewGuid(),
				DispatcherId = userId,
				TruckId = Guid.NewGuid(),
				IsBusy = false,
				IsFired = false
			};

			var load = new Load
			{
				Id = Guid.NewGuid(),
				OriginCity = "Boardman",
				OriginState = "OH",
				DestinationCity = "Chicago",
				DestinationState = "IL",
				Status = LoadStatus.Booked,
				BookedLoad = new BookedLoad
				{
					DispatcherId = userId, 
					DriverId = existingDriver.DriverId, 
					Driver = existingDriver
				}
			};

			_dbContext.Drivers.Add(existingDriver);
			_dbContext.Drivers.Add(newDriver);
			_dbContext.Loads.Add(load);
			await _dbContext.SaveChangesAsync();

			var result = await _driverService.AssignADriverToLoadAsync(load.Id, newDriver.DriverId, userId);

			Assert.That(result, Is.True);
			Assert.That(load.BookedLoad!.DriverId, Is.EqualTo(newDriver.DriverId));
			Assert.That(existingDriver.IsBusy, Is.False);
			Assert.That(newDriver.IsBusy, Is.True);
		}

		[Test]
		public async Task GetDriversWithTrucksAsync_ShouldReturnEmptyList_WhenNoDriversHaveTrucks()
		{
			var user = new User
			{
				Id = Guid.NewGuid(),
				FirstName = "Bruce",
				LastName = "Wayne",
				UserName = "batman",
				Email = "batman@chucknorris.com"

			};

			await _dbContext.Users.AddAsync(user);
			await _dbContext.SaveChangesAsync();

			_userService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(user);

			var result = await _driverService.GetDriversWithTrucksAsync();

			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task GetDriversWithTrucksAsync_ShouldReturnDriversWithTrucks_WhenDriversHaveTrucksAndAreNotBusy()
		{

			var user = new User
			{
				Id = Guid.NewGuid(),
				FirstName = "Bruce",
				LastName = "Wayne",
				UserName = "batman",
				Email = "batman@chucknorris.com"

			};

			var driver1 = new Driver
			{
				FirstName = "Vasko",
				LastName = "Jabata",
				LicenseNumber = "THG6985",
				DriverId = Guid.NewGuid(),
				DispatcherId = user.Id,
				TruckId = Guid.NewGuid(),
				IsBusy = false
			};

			var driver2 = new Driver
			{
				FirstName = "Ceca",
				LastName = "Mecata",
				LicenseNumber = "ciganiq123456",
				DriverId = Guid.NewGuid(),
				DispatcherId = user.Id,
				TruckId = Guid.NewGuid(),
				IsBusy = false
			};

			var busyDriver = new Driver
			{
				FirstName = "Gosho",
				LastName = "NqkakvaMastiq",
				LicenseNumber = "prostotiq",
				DriverId = Guid.NewGuid(),
				DispatcherId = user.Id,
				TruckId = Guid.NewGuid(),
				IsBusy = true
			};

			await _dbContext.Users.AddAsync(user);
			await _dbContext.Drivers.AddRangeAsync(driver1, driver2, busyDriver);
			await _dbContext.SaveChangesAsync();

			_userService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(user);

			var result = await _driverService.GetDriversWithTrucksAsync();

			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result, Does.Contain(driver1));
			Assert.That(result, Does.Contain(driver2));
			Assert.That(result, Does.Not.Contain(busyDriver));
		}

		[Test]
		public async Task GetDriversWithTrucksAsync_ShouldReturnEmptyList_WhenNoDriversBelongToCurrentUser()
		{
			var user = new User
			{
				Id = Guid.NewGuid(),
				FirstName = "Bruce",
				LastName = "Wayne",
				UserName = "batman",
				Email = "batman@chucknorris.com"

			};

			var otherUserDriver = new Driver
			{
				FirstName = "Ceca",
				LastName = "Mecata",
				LicenseNumber = "ciganiq123456",
				DriverId = Guid.NewGuid(),
				DispatcherId = Guid.NewGuid(), 
				TruckId = Guid.NewGuid(),
				IsBusy = false
			};

			await _dbContext.Users.AddAsync(user);
			await _dbContext.Drivers.AddAsync(otherUserDriver);
			await _dbContext.SaveChangesAsync();

			_userService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(user);

			var result = await _driverService.GetDriversWithTrucksAsync();

			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task GetDriversWithTrucksAsync_ShouldReturnEmptyList_WhenAllDriversWithTrucksAreBusy()
		{
			var user = new User
			{
				Id = Guid.NewGuid(),
				FirstName = "Bruce",
				LastName = "Wayne",
				UserName = "batman",
				Email = "batman@chucknorris.com"

			};

			var busyDriver1 = new Driver
			{
				FirstName = "Ceca",
				LastName = "Mecata",
				LicenseNumber = "ciganiq123456",
				DriverId = Guid.NewGuid(),
				DispatcherId = user.Id,
				TruckId = Guid.NewGuid(),
				IsBusy = true
			};

			var busyDriver2 = new Driver
			{
				FirstName = "Vasko",
				LastName = "Jabata",
				LicenseNumber = "THG6985",
				DriverId = Guid.NewGuid(),
				DispatcherId = user.Id,
				TruckId = Guid.NewGuid(),
				IsBusy = true
			};

			await _dbContext.Users.AddAsync(user);
			await _dbContext.Drivers.AddRangeAsync(busyDriver1, busyDriver2);
			await _dbContext.SaveChangesAsync();

			_userService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(user);

			var result = await _driverService.GetDriversWithTrucksAsync();

			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task GetAllDrivers_ShouldReturnEmptyList_WhenNoDriversExist()
		{
			var result = await _driverService.GetAllDrivers();

			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task GetAllDrivers_ShouldReturnAllDrivers_WhenDriversExist()
		{
			var driver1 = new Driver
			{
				DriverId = Guid.NewGuid(),
				FirstName = "John",
				LastName = "Doe",
				LicenseNumber = "ciganiq123456",

			};

			var driver2 = new Driver
			{
				DriverId = Guid.NewGuid(),
				FirstName = "Jane",
				LastName = "Smith",
				LicenseNumber = "ciganiq123456",

			};

			await _dbContext.Drivers.AddRangeAsync(driver1, driver2);
			await _dbContext.SaveChangesAsync();

			var result = await _driverService.GetAllDrivers();

			Assert.That(result.Count(), Is.EqualTo(2));
			Assert.That(result, Does.Contain(driver1));
			Assert.That(result, Does.Contain(driver2));
		}

		[Test]
		public async Task GetAllDrivers_ShouldReturnDriversWithCorrectData()
		{
			var driver = new Driver
			{
				DriverId = Guid.NewGuid(),
				FirstName = "John",
				LastName = "Doe",
				LicenseNumber = "ABC123",
				DispatcherId = Guid.NewGuid()
			};

			await _dbContext.Drivers.AddAsync(driver);
			await _dbContext.SaveChangesAsync();

			var result = await _driverService.GetAllDrivers();

			var fetchedDriver = result.First();
			Assert.That(fetchedDriver.DriverId, Is.EqualTo(driver.DriverId));
			Assert.That(fetchedDriver.FirstName, Is.EqualTo(driver.FirstName));
			Assert.That(fetchedDriver.LastName, Is.EqualTo(driver.LastName));
			Assert.That(fetchedDriver.LicenseNumber, Is.EqualTo(driver.LicenseNumber));
			Assert.That(fetchedDriver.DispatcherId, Is.EqualTo(driver.DispatcherId));
		}

		[Test]
		public async Task GetAllDrivers_ShouldReturnAllDrivers_WhenManyDriversExist()
		{
			var drivers = Enumerable.Range(1, 1000).Select(i => new Driver
			{
				DriverId = Guid.NewGuid(),
				FirstName = $"Driver{i}",
				LastName = $"Test{i}",
				LicenseNumber = "ciganiq123456",

			}).ToList();

			await _dbContext.Drivers.AddRangeAsync(drivers);
			await _dbContext.SaveChangesAsync();

			var result = await _driverService.GetAllDrivers();

			Assert.That(result.Count(), Is.EqualTo(1000));
		}

		[Test]
		public async Task GetDriverCount_ShouldReturnCorrectDriverCount_WhenDriversExist()
		{
			var userId = Guid.NewGuid(); 

			var driver1 = new Driver
			{
				FirstName = "Ceca",
				LastName = "Mecata",
				LicenseNumber = "ciganiq123456",
				DriverId = Guid.NewGuid(), 
				DispatcherId = userId
			};
			var driver2 = new Driver
			{
				FirstName = "Vasko",
				LastName = "DaGama",
				LicenseNumber = "5656456",
				DriverId = Guid.NewGuid(),
				DispatcherId = userId
			};
			var driver3 = new Driver
			{
				FirstName = "WhoYou",
				LastName = "Putkey",
				LicenseNumber = "5656456",
				DriverId = Guid.NewGuid(),
				DispatcherId = userId
			};

			await _dbContext.Drivers.AddRangeAsync(driver1, driver2, driver3);
			await _dbContext.SaveChangesAsync();

			var result = await _driverService.GetDriverCount(userId);

			Assert.That(result, Is.EqualTo(3));
		}

		[Test]
		public async Task GetDriverCount_ShouldReturnZero_WhenNoDriversExist()
		{
			var userId = Guid.NewGuid(); 

			var result = await _driverService.GetDriverCount(userId);

			Assert.That(result, Is.EqualTo(0));
		}

	}
}
