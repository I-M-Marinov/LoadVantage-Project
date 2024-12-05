using Microsoft.EntityFrameworkCore;

using Moq;
using NUnit.Framework;

using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;


using static LoadVantage.Common.GeneralConstants.ErrorMessages;



namespace LoadVantage.Tests.Core.Services
{
	public class LoadBoardServiceTests
	{
		private LoadVantageDbContext _dbContext;
		private Mock<IProfileService> _profileService;
		private Mock<ILoadHelperService> _loadHelperService;
		private ILoadBoardService _loadBoardService;

		[SetUp]
		public void SetUp()
		{
			var options = new DbContextOptionsBuilder<LoadVantageDbContext>()
				.UseInMemoryDatabase(databaseName: "LoadVantageTestInMemoryDB")
				.Options;

			_dbContext = new LoadVantageDbContext(options);

			_profileService = new Mock<IProfileService>();
			_loadHelperService = new Mock<ILoadHelperService>();

			_loadBoardService = new LoadBoardService(_dbContext, _profileService.Object, _loadHelperService.Object);
		}

		[TearDown]
		public void TearDown()
		{
			_dbContext.Database.EnsureDeleted();
			_dbContext.Dispose();
		}

		[Test]
		public async Task GetAllPostedLoadsAsync_ShouldReturnCorrectPostedLoads_WhenLoadsExist()
		{
			var brokerId = Guid.NewGuid();
			var dispatcherId = Guid.NewGuid();

			var loads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					OriginCity = "Los Angeles",
					OriginState = "CA",
					DestinationCity = "Seattle",
					DestinationState = "WA",
					PickupTime = new DateTime(2024, 12, 10, 8, 0, 0),
					DeliveryTime = new DateTime(2024, 12, 11, 18, 0, 0),
					Price = 1500.00M,
					Distance = 1135.0,
					Weight = 1200.5,
					Status = LoadStatus.Available,
					BrokerId = brokerId,
					PostedLoad = new PostedLoad
					{
						PostedDate = new DateTime(2024, 12, 01)
					}
				},
				new Load
				{
					Id = Guid.NewGuid(),
					OriginCity = "San Francisco",
					OriginState = "CA",
					DestinationCity = "Portland",
					DestinationState = "OR",
					PickupTime = new DateTime(2024, 12, 12, 9, 0, 0),
					DeliveryTime = new DateTime(2024, 12, 13, 17, 0, 0),
					Price = 1000.00M,
					Distance = 635.0,
					Weight = 800.0,
					Status = LoadStatus.Available,
					BrokerId = brokerId,
					PostedLoad = new PostedLoad
					{
						PostedDate = new DateTime(2024, 12, 02)
					}
				},
				new Load
				{
					Id = Guid.NewGuid(),
					OriginCity = "Phoenix",
					OriginState = "AZ",
					DestinationCity = "Chicago",
					DestinationState = "IL",
					PickupTime = new DateTime(2024, 12, 15, 8, 0, 0),
					DeliveryTime = new DateTime(2024, 12, 16, 18, 0, 0),
					Price = 1200.00M,
					Distance = 850.0,
					Weight = 1100.0,
					Status = LoadStatus.Created, 
		            BrokerId = brokerId,
					PostedLoad = null
				}
			};

			_dbContext.Loads.AddRange(loads);
			await _dbContext.SaveChangesAsync();

			var result = await _loadBoardService.GetAllPostedLoadsAsync(dispatcherId);

			Assert.That(result.Count(), Is.EqualTo(2)); 
			Assert.That(result.First().OriginCity, Is.EqualTo("San Francisco")); 
			Assert.That(result.First().PostedPrice, Is.EqualTo(1000.00M));
			Assert.That(result.Last().DestinationCity, Is.EqualTo("Seattle"));
			Assert.That(result.Last().Weight, Is.EqualTo(1200.5));
		}
		[Test]
		public async Task GetBrokerLoadBoardAsync_ShouldReturnValidBrokerLoadBoard()
		{
			var brokerId = Guid.NewGuid();

			var broker = new Broker()
			{
				Id = brokerId,
				FirstName = "John",
				LastName = "Doe",
				UserName = "johndoe123",
				Email = "johndoe@example.com",
				PhoneNumber = "123-456-7890"
			};


			var mockLoads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					BrokerId = brokerId,
					Status = LoadStatus.Created,
					OriginCity = "Chicago",
					OriginState = "IL",
					DestinationCity = "Dallas",
					DestinationState = "TX",
					Weight = 20000,
					Price = 3000,
					PickupTime = DateTime.UtcNow
				},
				new Load
				{
					Id = Guid.NewGuid(),
					BrokerId = brokerId,
					Status = LoadStatus.Available,
					OriginCity = "New York",
					OriginState = "NY",
					DestinationCity = "Los Angeles",
					DestinationState = "CA",
					Weight = 15000,
					Price = 2500,
					PickupTime = DateTime.UtcNow
				},
				
			};

			_loadHelperService.Setup(service => service.GetAllLoads()).ReturnsAsync(mockLoads);

			_profileService.Setup(service => service.GetUserInformation(brokerId))
				.ReturnsAsync(new ProfileViewModel
				{
					Username = "johndoe123",
					Email = "johndoe@example.com",
					FirstName = "John",
					LastName = "Doe",
					CompanyName = "LoadVantage Inc.",
					PhoneNumber = "123-456-7890"
				});


			var result = await _loadBoardService.GetBrokerLoadBoardAsync(brokerId);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.UserId, Is.EqualTo(brokerId));

			var createdLoads = result.CreatedLoads;

			Assert.That(createdLoads, Is.Not.Null);
			Assert.That(createdLoads, Has.Count.EqualTo(1)); 

			var createdLoad = createdLoads.First();

			Assert.That(createdLoad.OriginCity, Is.EqualTo("Chicago"));
			Assert.That(createdLoad.OriginState, Is.EqualTo("IL"));
			Assert.That(createdLoad.DestinationCity, Is.EqualTo("Dallas"));
			Assert.That(createdLoad.DestinationState, Is.EqualTo("TX"));
			Assert.That(createdLoad.Weight, Is.EqualTo(20000));
			Assert.That(createdLoad.PostedPrice, Is.EqualTo(3000));

			var postedLoads = result.PostedLoads;

			Assert.That(postedLoads, Is.Not.Null);
			Assert.That(postedLoads, Has.Count.EqualTo(1)); 

			var postedLoad = postedLoads.First();
			Assert.That(postedLoad.OriginCity, Is.EqualTo("New York"));
			Assert.That(postedLoad.OriginState, Is.EqualTo("NY"));
			Assert.That(postedLoad.DestinationCity, Is.EqualTo("Los Angeles"));
			Assert.That(postedLoad.DestinationState, Is.EqualTo("CA"));
			Assert.That(postedLoad.Weight, Is.EqualTo(15000));
			Assert.That(postedLoad.PostedPrice, Is.EqualTo(2500));

			var profile = result.Profile;

			Assert.That(profile, Is.Not.Null);
			Assert.That(profile.Username, Is.EqualTo("johndoe123"));
			Assert.That(profile.Email, Is.EqualTo("johndoe@example.com"));
			Assert.That(profile.FirstName, Is.EqualTo("John"));
			Assert.That(profile.LastName, Is.EqualTo("Doe"));
			Assert.That(profile.CompanyName, Is.EqualTo("LoadVantage Inc."));
			Assert.That(profile.PhoneNumber, Is.EqualTo("123-456-7890"));

		}

		[Test]
		public async Task GetDispatcherLoadBoardAsync_ShouldReturnValidDispatcherLoadBoard()
		{
			
			var dispatcherId = Guid.NewGuid();
			var brokerId = Guid.NewGuid();
			var driverId = Guid.NewGuid();

			var firstLoadId = Guid.NewGuid();
			var secondLoadId = Guid.NewGuid();
			var thirdLoadId = Guid.NewGuid();

			var broker = new Broker()
			{
				Id = brokerId,
				FirstName = "John",
				LastName = "Doe",
				UserName = "johndoe123",
				Email = "johndoe@broker.com",
				PhoneNumber = "123-456-7890"
			};

			var dispatcher = new Dispatcher
			{
				Id = dispatcherId,
				FirstName = "Michael",
				LastName = "Brown",
				UserName = "micky_mouse",
				Email = "mbrown@dispatcher.com",
				PhoneNumber = "123-456-7890"
			};

			var driver = new Driver
			{
				DriverId = driverId,
				FirstName = "Alice",
				LastName = "Smith",
				LicenseNumber = "TRG4666"
			};


			var mockLoads = new List<Load>
			{
				new Load
				{
					Id = firstLoadId,
					Status = LoadStatus.Available,
					OriginCity = "Chicago",
					OriginState = "IL",
					DestinationCity = "Dallas",
					DestinationState = "TX",
					PickupTime = DateTime.UtcNow.AddDays(1),
					DeliveryTime = DateTime.UtcNow.AddDays(2),
					Price = 2000,
					Distance = 800,
					BrokerId = brokerId,
					Broker = broker,
					PostedLoad = new PostedLoad
					{
						LoadId = firstLoadId
					}
				},
				new Load
				{
					Id = secondLoadId,
					Status = LoadStatus.Booked,
					OriginCity = "New York",
					OriginState = "NY",
					DestinationCity = "Los Angeles",
					DestinationState = "CA",
					PickupTime = DateTime.UtcNow.AddDays(3),
					DeliveryTime = DateTime.UtcNow.AddDays(5),
					Price = 4000,
					Distance = 2800,
					BrokerId = brokerId,
					Broker = broker,
					BookedLoad = new BookedLoad
					{
						DispatcherId = dispatcherId,
						Dispatcher = dispatcher,
						DriverId = driverId,
						Driver = driver
					}
				}
			};

			var mockProfile = new ProfileViewModel
			{
				Username = "dispatcher1",
				Email = "dispatcher1@example.com",
				FirstName = "Michael",
				LastName = "Brown",
				CompanyName = "Dispatch Co.",
				PhoneNumber = "123-456-7890"
			};

			_loadHelperService.Setup(service => service.GetAllLoads())
				.ReturnsAsync(mockLoads);

			_profileService.Setup(service => service.GetUserInformation(dispatcherId))
				.ReturnsAsync(mockProfile);


			var result = await _loadBoardService.GetDispatcherLoadBoardAsync(dispatcherId);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.UserId, Is.EqualTo(dispatcherId));

			var postedLoads = result.PostedLoads;
			Assert.That(postedLoads, Has.Count.EqualTo(1));

			var postedLoad = postedLoads.First();

			Assert.That(postedLoad.OriginCity, Is.EqualTo("Chicago"));
			Assert.That(postedLoad.OriginState, Is.EqualTo("IL"));
			Assert.That(postedLoad.DestinationCity, Is.EqualTo("Dallas"));
			Assert.That(postedLoad.DestinationState, Is.EqualTo("TX"));

			var bookedLoads = result.BookedLoads;

			Assert.That(bookedLoads, Has.Count.EqualTo(1));

			var bookedLoad = bookedLoads.First();

			Assert.That(bookedLoad.OriginCity, Is.EqualTo("New York"));
			Assert.That(bookedLoad.OriginState, Is.EqualTo("NY"));
			Assert.That(bookedLoad.DestinationCity, Is.EqualTo("Los Angeles"));
			Assert.That(bookedLoad.DestinationState, Is.EqualTo("CA"));
			Assert.That(bookedLoad.DispatcherId, Is.EqualTo(dispatcherId));
			Assert.That(bookedLoad.DriverId, Is.EqualTo(driverId));


		}

		[Test]
		public void GetLoadCountsForUserAsync_ShouldThrowArgumentException_WhenUserPositionIsInvalid()
		{
			var userId = Guid.NewGuid();

			var invalidUserPosition = "InvalidPosition";

			Assert.That(async () => await _loadBoardService.GetLoadCountsForUserAsync(userId, invalidUserPosition),
				Throws.TypeOf<ArgumentException>()
					.With.Message.EqualTo(InvalidUserType));
		}

	}
}
