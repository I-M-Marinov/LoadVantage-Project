using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data.Contracts;

namespace LoadVantage.Tests.Core.Services
{
	public class TruckServiceTests
	{
		private Mock<IUserService> _mockUserService;
		private Mock<IProfileService> _mockProfileService; 
		private LoadVantageDbContext _dbContext;
		private Mock<IHtmlSanitizerService> _mockSanitizer; // Mock for the sanitizer service


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



	}
}
