using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Core.Models.Truck;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;


namespace LoadVantage.Tests.Core.Services
{
	public class TruckServiceTests
	{
		private Mock<IUserService> _mockUserService;
		private Mock<IProfileService> _mockProfileService; 
		private LoadVantageDbContext _dbContext;
		private Mock<IHtmlSanitizerService> _mockSanitizer; 


		[SetUp]
		public void SetUp()
		{
			_mockUserService = new Mock<IUserService>();
			_mockProfileService = new Mock<IProfileService>();
			_mockSanitizer = new Mock<IHtmlSanitizerService>(); 

			var options = new DbContextOptionsBuilder<LoadVantageDbContext>()
				.UseInMemoryDatabase(databaseName: "LoadVantageTestInMemoryDB")
				.Options;

			_dbContext = new LoadVantageDbContext(options);


		}

		[TearDown]
		public void TearDown()
		{
			_dbContext.Database.EnsureDeleted();
			_dbContext.Dispose();
		}


		[Test]
		public async Task GetAllTrucksAsync_ShouldReturnCorrectTrucksViewModel()
		{
			var userId = Guid.NewGuid();

			var user = new User
			{
				Id = userId,
				UserName = "batman",
				Email = "batman@gmail.com",
				FirstName = "Bruce",
				LastName = "Wayne",
				UserImageId = Guid.Empty
			};

			_mockUserService.Setup(us => us.GetUserByIdAsync(userId))
				.ReturnsAsync(user);

			var profile = new ProfileViewModel
			{
				FirstName = "Jack",
				LastName = "Dispatcher",
				Email = "dispatcher@google.com"
			};

			_mockProfileService.Setup(ps => ps.GetUserInformation(user.Id))
				.ReturnsAsync(profile);

			var truckDriver = new Driver
			{
				DriverId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
				FirstName = "John",
				LastName = "Doe",
				LicenseNumber = "WTF654977"
			};

			var trucks = new List<Truck>
			{
				new Truck
				{
					Id = Guid.NewGuid(),
					TruckNumber = "TRK123",
					Make = "Volvo",
					Model = "VNL 760",
					Year = 2024,
					Driver = truckDriver,
					DriverId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
					DispatcherId = userId,
					IsActive = true,
					IsAvailable = true
				},
				new Truck
				{
					Id = Guid.NewGuid(),
					TruckNumber = "TRK456",
					Make = "Freighliner",
					Model = "Cascadia",
					Year = 2023,
					Driver = null,
					DispatcherId = userId,
					IsActive = true,
					IsAvailable = false
				}
			};

			await _dbContext.Trucks.AddRangeAsync(trucks);
			await _dbContext.SaveChangesAsync();


			var truckService = new TruckService(
				_dbContext,
				_mockProfileService.Object,
				_mockUserService.Object,
				_mockSanitizer.Object
			);

			var result = await truckService.GetAllTrucksAsync(userId);

			_mockUserService.Verify(us => us.GetUserByIdAsync(userId), Times.Once);
			_mockProfileService.Verify(ps => ps.GetUserInformation(user.Id), Times.Once);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Profile, Is.EqualTo(profile));
			Assert.That(result.Trucks.Count, Is.EqualTo(2));

			Assert.That(result.Trucks.First().TruckNumber, Is.EqualTo("TRK456"));
			Assert.That(result.Trucks.First().DriverName, Is.EqualTo("N/A"));
			Assert.That(result.Trucks.Last().TruckNumber, Is.EqualTo("TRK123"));
			Assert.That(result.Trucks.Last().DriverName, Is.EqualTo("John Doe"));
			Assert.That(result.NewTruck, Is.Not.Null);
		}

		[Test]
		public async Task GetTruckByIdAsync_ShouldReturnTruck_WhenTruckExistsAndBelongsToCurrentUser()
		{
			// Arrange
			var truckId = Guid.NewGuid();
			var userId = Guid.NewGuid();

			var mockUser = new User
			{
				Id = userId,
				UserName = "batman",
				Email = "batman@gmail.com",
				FirstName = "Bruce",
				LastName = "Wayne",
				UserImageId = Guid.Empty
			};


			await _dbContext.Users.AddAsync(mockUser);

			var truck = new Truck
			{
				Id = truckId,
				DispatcherId = userId,
				IsActive = true,
				TruckNumber = "12345",
				Make = "Ford",
				Model = "F-150",
				Year = 2021,
				IsAvailable = true,
				Driver = new Driver
				{
					DriverId = Guid.NewGuid(), 
					FirstName = "John",
					LastName = "Doe",
					LicenseNumber = "4545664565"
				}
			};

			await _dbContext.Trucks.AddAsync(truck);
			await _dbContext.SaveChangesAsync();

			_mockUserService.Setup(us => us.GetCurrentUserAsync())
				.ReturnsAsync(mockUser);

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			var result = await truckService.GetTruckByIdAsync(truckId);

			Assert.That(result, Is.Not.Null);
			Assert.That(result!.Id, Is.EqualTo(truck.Id));
			Assert.That(result.TruckNumber, Is.EqualTo(truck.TruckNumber));
			Assert.That(result.Make, Is.EqualTo(truck.Make));
			Assert.That(result.Model, Is.EqualTo(truck.Model));
			Assert.That(result.Year, Is.EqualTo(truck.Year.ToString()));
			Assert.That(result.IsAvailable, Is.EqualTo(truck.IsAvailable));
		}

		[Test]
		public async Task GetTruckByIdAsync_ShouldReturnNull_WhenTruckDoesNotExist()
		{
			// Arrange
			var truckId = Guid.NewGuid(); 
			var userId = Guid.NewGuid();

			var mockUser = new User
			{
				Id = userId,
				UserName = "batman",
				Email = "batman@gmail.com",
				FirstName = "Bruce",
				LastName = "Wayne",
				UserImageId = Guid.Empty
			}; 
			
			await _dbContext.Users.AddAsync(mockUser);
			await _dbContext.SaveChangesAsync();

			_mockUserService.Setup(us => us.GetCurrentUserAsync())
				.ReturnsAsync(mockUser);

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			var result = await truckService.GetTruckByIdAsync(truckId);

			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task AddTruckAsync_ShouldAddTruckToDatabase_WhenModelIsValid()
		{
			var userId = Guid.NewGuid();

			var truckModel = new TruckViewModel
			{
				TruckNumber = " 12345  ", 
				Make = "<b>Ford</b>",   
				Model = "F-150",
				Year = "2021"
			};

			_mockSanitizer.Setup(s => s.Sanitize(" 12345  ")).Returns("12345");
			_mockSanitizer.Setup(s => s.Sanitize("<b>Ford</b>")).Returns("Ford");
			_mockSanitizer.Setup(s => s.Sanitize("F-150")).Returns("F-150");
			_mockSanitizer.Setup(s => s.Sanitize("2021")).Returns("2021");

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			await truckService.AddTruckAsync(truckModel, userId);

			var addedTruck = await _dbContext.Trucks.FirstOrDefaultAsync();

			Assert.That(addedTruck, Is.Not.Null);
			Assert.That(addedTruck!.TruckNumber, Is.EqualTo("12345"));
			Assert.That(addedTruck.Make, Is.EqualTo("Ford"));
			Assert.That(addedTruck.Model, Is.EqualTo("F-150"));
			Assert.That(addedTruck.Year, Is.EqualTo(2021));
			Assert.That(addedTruck.DispatcherId, Is.EqualTo(userId));
			Assert.That(addedTruck.DriverId, Is.Null);
			Assert.That(addedTruck.IsAvailable, Is.True);
			Assert.That(addedTruck.IsActive, Is.True);

			_mockSanitizer.Verify(s => s.Sanitize(It.IsAny<string>()), Times.Exactly(4));
		}

		[Test]
		public async Task UpdateTruckAsync_ShouldUpdateTruck_WhenTruckExists()
		{
			var userId = Guid.NewGuid();
			var truckId = Guid.NewGuid();

			var existingTruck = new Truck
			{
				Id = truckId,
				TruckNumber = "OldTruckNumber",
				Make = "OldMake",
				Model = "OldModel",
				Year = 1991,
				DispatcherId = userId,
				IsActive = true,
				IsAvailable = true
			};

			_dbContext.Trucks.Add(existingTruck);
			await _dbContext.SaveChangesAsync();

			var truckModel = new TruckViewModel
			{
				Id = truckId,
				TruckNumber = "  NewTruckNumber  ",  
				Make = "<i>Mitsubishi</i>",           
				Model = "Lancer",
				Year = "2023"
			};

			_mockSanitizer.Setup(s => s.Sanitize("  NewTruckNumber  ")).Returns("NewTruckNumber");
			_mockSanitizer.Setup(s => s.Sanitize("<i>Mitsubishi</i>")).Returns("Mitsubishi");
			_mockSanitizer.Setup(s => s.Sanitize("Lancer")).Returns("Lancer");
			_mockSanitizer.Setup(s => s.Sanitize("2023")).Returns("2023");

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			await truckService.UpdateTruckAsync(truckModel);

			var updatedTruck = await _dbContext.Trucks.FirstOrDefaultAsync(t => t.Id == truckId);

			Assert.That(updatedTruck, Is.Not.Null);
			Assert.That(updatedTruck.TruckNumber, Is.EqualTo("NewTruckNumber"));
			Assert.That(updatedTruck.Make, Is.EqualTo("Mitsubishi"));
			Assert.That(updatedTruck.Model, Is.EqualTo("Lancer"));
			Assert.That(updatedTruck.Year, Is.EqualTo(2023));

			_mockSanitizer.Verify(s => s.Sanitize(It.IsAny<string>()), Times.Exactly(4));
		}

		[Test]
		public void UpdateTruckAsync_ShouldThrowKeyNotFoundException_WhenTruckDoesNotExist()
		{
	
			var userId = Guid.NewGuid();
			var truckId = Guid.NewGuid(); 

			var truckModel = new TruckViewModel
			{
				Id = truckId,
				TruckNumber = "TRUCK666",
				Make = "Peterbilt",
				Model = "Something",
				Year = "2023"
			};

			_mockSanitizer.Setup(s => s.Sanitize(It.IsAny<string>())).Returns<string>(x => x);

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			Assert.ThrowsAsync<KeyNotFoundException>(async () => await truckService.UpdateTruckAsync(truckModel));
		}

		[Test]
		public async Task DeleteTruckAsync_ShouldDeactivateTruck_WhenTruckExistsAndIsAvailable()
		{
			var userId = Guid.NewGuid();
			var truckId = Guid.NewGuid();

			var truck = new Truck
			{
				Id = truckId,
				TruckNumber = "TruckNumberToDelete",
				Make = "Ford",
				Model = "F-150",
				Year = 2020,
				DispatcherId = userId,
				IsAvailable = true,
				IsActive = true
			};

			_dbContext.Trucks.Add(truck);
			await _dbContext.SaveChangesAsync();

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			await truckService.DeleteTruckAsync(truckId);

			var deletedTruck = await _dbContext.Trucks.FirstOrDefaultAsync(t => t.Id == truckId);

			Assert.That(deletedTruck, Is.Not.Null);
			Assert.That(deletedTruck.IsActive, Is.False);  // Truck should be deactivated (soft delete)
			Assert.That(deletedTruck.IsAvailable, Is.False);  // Truck should be marked as unavailable
		}

		[Test]
		public async Task DeleteTruckAsync_ShouldThrowInvalidOperationException_WhenTruckIsInUse()
		{
			var userId = Guid.NewGuid();
			var truckId = Guid.NewGuid();

			var truck = new Truck
			{
				Id = truckId,
				TruckNumber = "TruckInUse",
				Make = "Chevrolet",
				Model = "Silverado",
				Year = 2021,
				DispatcherId = userId,
				IsAvailable = false,  // This truck is in use
				IsActive = true
			};

			_dbContext.Trucks.Add(truck);
			await _dbContext.SaveChangesAsync();

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			Assert.ThrowsAsync<InvalidOperationException>(async () => await truckService.DeleteTruckAsync(truckId));
		}

		[Test]
		public async Task AssignDriverToTruckAsync_ShouldAssignDriver_WhenTruckAndDriverAreAvailable()
		{
			var truckId = Guid.NewGuid();
			var driverId = Guid.NewGuid();

			var truck = new Truck
			{
				Id = truckId,
				TruckNumber = "Truck1",
				Make = "Ford",
				Model = "F-150",
				Year = 2020,
				IsAvailable = true,
				IsActive = true
			};

			_dbContext.Trucks.Add(truck);

			var driver = new Driver
			{
				DriverId = driverId,
				FirstName = "Dwayne",
				LastName = "Johnson",
				IsAvailable = true,
				LicenseNumber = "WTF654977",
				TruckId = null, // Driver has no truck assigned
			};


			_dbContext.Drivers.Add(driver);

			await _dbContext.SaveChangesAsync();

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			var result = await truckService.AssignDriverToTruckAsync(truckId, driverId);

			var updatedTruck = await _dbContext.Trucks.FirstOrDefaultAsync(t => t.Id == truckId);
			var updatedDriver = await _dbContext.Drivers.FirstOrDefaultAsync(d => d.DriverId == driverId);

			Assert.That(result, Is.True); 
			Assert.That(updatedTruck.DriverId, Is.EqualTo(driverId)); 
			Assert.That(updatedDriver.TruckId, Is.EqualTo(truckId)); 
			Assert.That(updatedDriver.IsAvailable, Is.False); 
			Assert.That(updatedTruck.IsAvailable, Is.False); 
		}

		[Test]
		public async Task AssignDriverToTruckAsync_ShouldReturnFalse_WhenTruckNotFound()
		{
			var truckId = Guid.NewGuid(); 
			var driverId = Guid.NewGuid();

			var driver = new Driver
			{
				DriverId = driverId,
				FirstName = "Dwayne",
				LastName = "Johnson",
				LicenseNumber = "WTF654977",
				IsAvailable = true,
				TruckId = null
			};

			_dbContext.Drivers.Add(driver);
			await _dbContext.SaveChangesAsync();

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			var result = await truckService.AssignDriverToTruckAsync(truckId, driverId);

			Assert.That(result, Is.False); 
		}

		[Test]
		public async Task AssignDriverToTruckAsync_ShouldReturnFalse_WhenDriverNotFound()
		{
			var truckId = Guid.NewGuid();

			var truck = new Truck
			{
				Id = truckId,
				TruckNumber = "Truck13",
				Make = "Ford",
				Model = "F-150",
				Year = 2020,
				IsAvailable = true,
				IsActive = true
			};

			_dbContext.Trucks.Add(truck);
			await _dbContext.SaveChangesAsync();

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			var result = await truckService.AssignDriverToTruckAsync(truckId, Guid.NewGuid());

			Assert.That(result, Is.False); 
		}

		[Test]
		public async Task AssignDriverToTruckAsync_ShouldReturnFalse_WhenDriverOrTruckIsNotAvailable()
		{
			var truckId = Guid.NewGuid();
			var driverId = Guid.NewGuid();

			var truck = new Truck
			{
				Id = truckId,
				TruckNumber = "Truck666",
				Make = "Ford",
				Model = "F-150",
				Year = 2020,
				IsAvailable = false, // Truck is not available
				IsActive = true
			};

			var driver = new Driver
			{
				DriverId = driverId,
				FirstName = "Dwayne",
				LastName = "Johnson",
				LicenseNumber = "WTF654977",
				IsAvailable = true,
				TruckId = null
			};

			_dbContext.Trucks.Add(truck);
			_dbContext.Drivers.Add(driver);
			await _dbContext.SaveChangesAsync();

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			var result = await truckService.AssignDriverToTruckAsync(truckId, driverId);

			Assert.That(result, Is.False); 
		}

		[Test]
		public async Task ParkTruckAsync_ShouldReturnTrue_WhenTruckAndDriverAreAssignedAndAvailable()
		{
			var truckId = Guid.NewGuid();
			var driverId = Guid.NewGuid();

			var truck = new Truck
			{
				Id = truckId,
				TruckNumber = "Truck1",
				Make = "Ford",
				Model = "F-150",
				Year = 2020,
				IsAvailable = false, 
				DriverId = driverId,
				IsActive = true
			};

			var driver = new Driver
			{
				DriverId = driverId,
				FirstName = "Dwayne",
				LastName = "Johnson",
				LicenseNumber = "WTF654977",
				IsAvailable = false, 
				IsBusy = false, 
				TruckId = truckId 
			};


			_dbContext.Trucks.Add(truck);
			_dbContext.Drivers.Add(driver);
			await _dbContext.SaveChangesAsync();

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			var result = await truckService.ParkTruckAsync(truckId, driverId);

			var updatedTruck = await _dbContext.Trucks.FirstOrDefaultAsync(t => t.Id == truckId);
			var updatedDriver = await _dbContext.Drivers.FirstOrDefaultAsync(d => d.DriverId == driverId);

			Assert.That(result, Is.True);
			Assert.That(updatedTruck.DriverId, Is.Null); 
			Assert.That(updatedDriver.TruckId, Is.Null); 
			Assert.That(updatedDriver.IsAvailable, Is.True);
			Assert.That(updatedTruck.IsAvailable, Is.True);
		}

		[Test]
		public async Task ParkTruckAsync_ShouldReturnFalse_WhenTruckNotFound()
		{
			var truckId = Guid.NewGuid();
			var driverId = Guid.NewGuid();

			var driver = new Driver
			{
				DriverId = driverId,
				FirstName = "Dwayne",
				LastName = "Johnson",
				LicenseNumber = "WTF654977",
				IsAvailable = false,
				IsBusy = false,
				TruckId = truckId 
			};

			_dbContext.Drivers.Add(driver);
			await _dbContext.SaveChangesAsync();

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			var result = await truckService.ParkTruckAsync(truckId, driverId);

			Assert.That(result, Is.False); 
		}

		[Test]
		public async Task ParkTruckAsync_ShouldReturnFalse_WhenDriverNotFound()
		{
			var truckId = Guid.NewGuid();

			var truck = new Truck
			{
				Id = truckId,
				TruckNumber = "Truck1",
				Make = "Ford",
				Model = "F-150",
				Year = 2020,
				IsAvailable = false,
				DriverId = Guid.NewGuid(), 
				IsActive = true
			};

			_dbContext.Trucks.Add(truck);
			await _dbContext.SaveChangesAsync();

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			var result = await truckService.ParkTruckAsync(truckId, Guid.NewGuid()); 

			Assert.That(result, Is.False); 
		}

		[Test]
		public async Task ParkTruckAsync_ShouldReturnFalse_WhenTruckIsAlreadyAvailable()
		{
			var truckId = Guid.NewGuid();
			var driverId = Guid.NewGuid();

			var truck = new Truck
			{
				Id = truckId,
				TruckNumber = "Truck1",
				Make = "Ford",
				Model = "F-150",
				Year = 2020,
				IsAvailable = true, 
				DriverId = null, 
				IsActive = true
			};

			var driver = new Driver
			{
				DriverId = driverId,
				FirstName = "Dwayne",
				LastName = "Johnson",
				LicenseNumber = "WTF654977",
				IsAvailable = true, 
				IsBusy = false,
				TruckId = null 
			};

			_dbContext.Trucks.Add(truck);
			_dbContext.Drivers.Add(driver);
			await _dbContext.SaveChangesAsync();

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			var result = await truckService.ParkTruckAsync(truckId, driverId);

			Assert.That(result, Is.False); 
		}

		[Test]
		public async Task ParkTruckAsync_ShouldThrowException_WhenDriverIsBusy()
		{
			var truckId = Guid.NewGuid();
			var driverId = Guid.NewGuid();

			var truck = new Truck
			{
				Id = truckId,
				TruckNumber = "Truck1",
				Make = "Ford",
				Model = "F-150",
				Year = 2020,
				IsAvailable = false, 
				DriverId = driverId,
				IsActive = true
			};

			var driver = new Driver
			{
				DriverId = driverId,
				FirstName = "Dwayne",
				LastName = "Johnson",
				LicenseNumber = "WTF654977",
				IsAvailable = false, 
				IsBusy = true, 
				TruckId = truckId
			};

			_dbContext.Trucks.Add(truck);
			_dbContext.Drivers.Add(driver);
			await _dbContext.SaveChangesAsync();

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			var ex = Assert.ThrowsAsync<InvalidOperationException>(() => truckService.ParkTruckAsync(truckId, driverId));
			Assert.That(ex.Message, Is.EqualTo(DriverCurrentlyUnderALoad)); 
		}

		[Test]
		public async Task GetAllTrucksAsync_ShouldReturnAllTrucks()
		{
			var truck1 = new Truck
			{
				Id = Guid.NewGuid(), 
				TruckNumber = "Truck1",
				Make = "Ford",
				Model = "F-150"
			};
			var truck2 = new Truck
			{
				Id = Guid.NewGuid(),
				TruckNumber = "Truck2",
				Make = "Mazda",
				Model = "Miata"
			};

			var trucks = new List<Truck> { truck1, truck2 };

			_dbContext.Trucks.AddRange(truck1, truck2);
			await _dbContext.SaveChangesAsync();  

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);

			var result = await truckService.GetAllTrucksAsync();

			Assert.That(result, Has.Count.EqualTo(2)); 
			Assert.That(result.First().TruckNumber, Is.EqualTo("Truck1"));
			Assert.That(result.Last().TruckNumber, Is.EqualTo("Truck2"));
		}

		[Test]
		public async Task GetTruckCount_ShouldReturnCorrectCount()
		{
			var userId = Guid.NewGuid(); 

			var truck1 = new Truck
			{
				Id = Guid.NewGuid(), 
				TruckNumber = "Truck1", 
				Make = "Make1", 
				Model = "Model1",
				Year = 2020, 
				IsAvailable = true, 
				IsActive = true, 
				DispatcherId = userId
			};

			var truck2 = new Truck
			{
				Id = Guid.NewGuid(), 
				TruckNumber = "Truck2", 
				Make = "Make2", 
				Model = "Model2",
				Year = 2021, 
				IsAvailable = true, 
				IsActive = true, 
				DispatcherId = userId
			};

			var truck3 = new Truck
			{
				Id = Guid.NewGuid(), 
				TruckNumber = "Truck3", 
				Make = "Make3", 
				Model = "Model3", 
				Year = 2022, 
				IsAvailable = true, 
				IsActive = true, 
				DispatcherId = userId
			};

			_dbContext.Trucks.AddRange(truck1, truck2, truck3);
			await _dbContext.SaveChangesAsync();  

			var truckService = new TruckService(_dbContext, _mockProfileService.Object, _mockUserService.Object, _mockSanitizer.Object);


			var result = await truckService.GetTruckCount(userId);

			Assert.That(result, Is.EqualTo(3));
		}

	}
}
