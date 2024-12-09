using System.Security.Claims;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using LoadVantage.Common.Enums;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data.Contracts;

using static LoadVantage.Common.GeneralConstants;
using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.GeneralConstants.SecretString;

using UserImage = LoadVantage.Infrastructure.Data.Models.UserImage;


namespace LoadVantage.Infrastructure.Data.SeedData
{
	public static class SeedData
	{
		public static async Task InitializeRoles(IServiceProvider serviceProvider)
		{
			var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

			foreach (var roleName in UserRoles.ValidRoles)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName);

				if (!roleExist)
				{
					await roleManager.CreateAsync(new Role { Name = roleName });
				} 
			}
		}

		public static async Task SeedAdminUser(IServiceProvider serviceProvider, IConfiguration configuration)
		{
			using var scope = serviceProvider.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<BaseUser>>();
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();


			var adminUserName = configuration["AdminCredentials:AdminUserName"];
			var adminEmail = configuration["AdminCredentials:AdminEmail"];
			var adminPassword = configuration["AdminCredentials:AdminPassword"];
			var adminFirstName = configuration["AdminCredentials:AdminFirstName"];
			var adminLastName = configuration["AdminCredentials:AdminLastName"];
			var adminPhoneNumber = configuration["AdminCredentials:AdminPhoneNumber"];
			var adminCompany = configuration["AdminCredentials:AdminCompany"];

			if (await userManager.FindByNameAsync(adminUserName!) == null)
			{
				Role? role = await roleManager.FindByNameAsync(AdminRoleName);
				var adminUser = new Administrator()
				{
					UserName = adminUserName,
					Email = adminEmail,
					FirstName = adminFirstName!,
					LastName = adminLastName!,
					Position = AdminPositionName,
                    PhoneNumber = adminPhoneNumber!,
                    CompanyName = adminCompany,
                    Role = role!,
                    UserImageId = DefaultImageId
				};

				var result = await userManager.CreateAsync(adminUser, adminPassword!);

				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, AdminRoleName);
                    await userManager.AddClaimAsync(adminUser, new Claim("Position", adminUser.Position ?? ""));
				}
                else
				{
					var errorMessages = result.Errors.Select(e => e.Description);
					throw new Exception($"Failed to create admin user: {string.Join(", ", errorMessages)}");
				}
			}
			
		}

        public static async Task SeedDispatchers(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<BaseUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            Role? userRole = await roleManager.FindByNameAsync(UserRoleName);

            var dispatchers = new List<User>
            {
                new User()
                {
                    UserName = "dispatcher1",
                    Email = "dispatcher1@gmail.com",
                    FirstName = "Wade",
                    LastName = "Wilson",
                    CompanyName = "Maximum Effort Corp.",
                    PhoneNumber = "+1-800-654-1234",
                    Position = DispatcherPositionName,
                    Role = userRole!,
                    UserImageId = DefaultImageId

                },
                new User()
                {
                    UserName = "dispatcher2",
                    Email = "dispatcher2@gmail.com",
                    FirstName = "Tony",
                    LastName = "Stark",
                    CompanyName = "Stark Industries",
                    PhoneNumber = "+1-225-968-4692",
                    Position = DispatcherPositionName,
                    Role = userRole!,
                    UserImageId = DefaultImageId

				}
            };

            var counter = 1;
            foreach (var dispatcher in dispatchers)
            {

                var userExists = await userManager.FindByNameAsync(dispatcher.UserName)
                                   ?? await userManager.FindByEmailAsync(dispatcher.Email);

                if (userExists != null)
                { 
                    continue;
                }
                string password = PasswordSecretWord + counter;
                counter++;

                var result = await userManager.CreateAsync(dispatcher, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(dispatcher, UserRoleName);
                    await userManager.AddClaimAsync(dispatcher, new Claim("Position", dispatcher.Position ?? ""));
                }
                else
                {
                    var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create dispatcher {dispatcher.UserName}: {errorMessages}");
                }

            }
        }

        public static async Task SeedBrokers(IServiceProvider serviceProvider, IConfiguration configuration)
        { 
            var dbContext = serviceProvider.GetRequiredService<LoadVantageDbContext>();

            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<BaseUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            // Check if any brokers already exist
            if (dbContext.Brokers.Any())
            {
                return; // do not seed any brokers again
            }

            Role? userRole = await roleManager.FindByNameAsync(UserRoleName);

            var brokers = new List<User>
            {
                new User()
                {
                    UserName = "broker1",
                    Email = "broker1@gmail.com",
                    FirstName = "Bugs",
                    LastName = "Bunny",
                    CompanyName = "What's Up Dock Inc",
                    PhoneNumber = "+1-708-953-7412",
                    Position = BrokerPositionName,
                    Role = userRole!,
                    UserImageId = DefaultImageId

				},
                new User()
                {
                    UserName = "broker2",
                    Email = "broker2@gmail.com",
                    FirstName = "Daffy",
                    LastName = "Duck",
                    CompanyName = "Rabbit Season Ltd.",
                    PhoneNumber = "+1-300-852-7391",
                    Position = BrokerPositionName,
                    Role = userRole!,
                    UserImageId = DefaultImageId

				}
            };


            var counter = 1;
            foreach (var broker in brokers)
            {

                var userExists = await userManager.FindByNameAsync(broker.UserName)
                                   ?? await userManager.FindByEmailAsync(broker.Email);

                if (userExists != null)
                {
                    continue;
                }
                string password = PasswordSecretWord + counter;
                counter++;

                var result = await userManager.CreateAsync(broker, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(broker, UserRoleName);
                    await userManager.AddClaimAsync(broker, new Claim("Position", broker.Position ?? ""));
                }
                else
                {
                    var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create broker {broker.UserName}: {errorMessages}");
                }

            }

        }

		public static async Task SeedDefaultUserImage(UserManager<BaseUser> userManager, IServiceProvider serviceProvider)
		{
			var dbContext = serviceProvider.GetRequiredService<LoadVantageDbContext>();

			var defaultImage = await dbContext.UsersImages
				.FirstOrDefaultAsync(img => img.Id == DefaultImageId);

			if (defaultImage == null)
			{
				defaultImage = new UserImage
				{
					Id = DefaultImageId,
					ImageUrl = DefaultImagePath, 
					PublicId = DefaultPublicId 
				};

				dbContext.UsersImages.Add(defaultImage);
				await dbContext.SaveChangesAsync();
			}

			await dbContext.SaveChangesAsync();
		}

		public static async Task SeedLoads(UserManager<BaseUser> userManager, IServiceProvider serviceProvider)
        {
	        var context = serviceProvider.GetRequiredService<LoadVantageDbContext>();

			var distanceCalculatorService = serviceProvider.GetRequiredService<IDistanceCalculatorService>();

            // Check if any loads already exist
            if (context.Loads.Any())
            {
                return; // Database has already been seeded
            }
            var random = new Random();

            var locations = new List<(string City, string State)>
            {
                ("Chicago", "IL"),
                ("New York City", "NY"),
                ("Atlanta", "GA"),
                ("Houston", "TX"),
                ("Los Angeles", "CA"),
                ("Denver", "CO"),
                ("Miami", "FL"),
                ("Seattle", "WA"),
                ("Phoenix", "AZ"),
                ("Reno", "NV"),
                ("Indianapolis", "IN"),
                ("Lincoln", "NE"),
                ("Charlotte", "NC"),
                ("Philadelphia", "PA"),
                ("Fort Worth", "TX"),
                ("Akron", "OH"),
                ("Portland", "OR"),
                ("Boston", "MA"),
                ("Detroit", "MI"),
                ("Albuquerque", "NM"),
                ("Las Vegas", "NV"),
                ("Montgomery", "AL"),
                ("Rochester", "NY")
            };

            List<Load> GenerateRandomLoads()
            {
                var loads = new List<Load>();

                for (int i = 0; i < 20; i++) // Generate twenty random loads per broker
                {
                    // Pick random origin and destination that are not the same
                    var origin = locations[random.Next(locations.Count)];
                    (string City, string State) destination;
                    do
                    {
                        destination = locations[random.Next(locations.Count)];
                    } while (origin == destination); // Ensure origin and destination are different

                    var now = DateTime.Now;
                    var createdDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0); // specifically set seconds to 00

					loads.Add(new Load
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = createdDate,
                        OriginCity = origin.City,
                        OriginState = origin.State,
                        DestinationCity = destination.City,
                        DestinationState = destination.State,
						PickupTime = createdDate.AddDays(random.Next(1, 7)),
						DeliveryTime = createdDate.AddDays(random.Next(8, 15)),
						Distance = 0,
                        Price = random.Next(1000, 7500), // Random price 
                        Weight = random.Next(500, 48000), // Random weight 
                        Status = LoadStatus.Created
                    });
                }

                return loads;
            }


            var brokersList = await userManager.Users.Where(u => u is Broker).ToListAsync(); // get all Users that are Brokers 
            var brokerIds = brokersList.Select(u => u.Id).ToList();

            var brokersWithLoadList = await context.Brokers
                .Include(b => b.Loads) // Include the Loads collection
                .Where(b => brokerIds.Contains(b.Id))
                .ToListAsync(); // Get the list of brokers

            var allLoads = new List<Load>();

            foreach (var broker in brokersWithLoadList)
            {
                var randomLoads = GenerateRandomLoads();

                foreach (var load in randomLoads)
                {
                    // Calculate the distance for the load
                    load.Distance = await distanceCalculatorService.GetDistanceBetweenCitiesAsync(
                        load.OriginCity, load.OriginState,
                        load.DestinationCity, load.DestinationState
                    );

                    // Add the load to the broker's collection
                    load.BrokerId = broker.Id;
                    allLoads.Add(load);
                }
            }

            await context.Loads.AddRangeAsync(allLoads);
            await context.SaveChangesAsync();
            
        }

		public static async Task SeedTrucks(UserManager<BaseUser> userManager, IServiceProvider serviceProvider)
		{
			var context = serviceProvider.GetRequiredService<LoadVantageDbContext>();

			// Check if trucks are already seeded
			if (context.Trucks.Any())
			{
				return; // Trucks are already seeded
			}


			var dispatchers = await userManager.Users
				.OfType<Dispatcher>()
				.ToListAsync();

			// Dictionary of American Truck Make and Models
			var makeModelDictionary = new Dictionary<string, List<string>>
			{
				{ "Freightliner", new List<string> { "Cascadia", "M2", "Columbia", "Xperience" } },
				{ "Peterbilt", new List<string> { "389", "579", "567", "337" } },
				{ "Kenworth", new List<string> { "T680", "W900", "T800", "T880" } },
				{ "Mack", new List<string> { "Anthem", "Granite", "Pinnacle", "TerraPro" } },
				{ "International", new List<string> { "LT Series", "ProStar", "Lonestar", "DuraStar" } },
				{ "Western Star", new List<string> { "5700XE", "4900", "6900", "4700" } },
				{ "Volvo", new List<string> { "VNL 300", "VNR 400", "VNX", "VHD" } },  
	            { "Sterling", new List<string> { "Acterra", "L8500", "A9500", "Bullet" } },
				{ "Navistar", new List<string> { "International CV Series", "ProStar", "LT", "MXT" } },
			};

			var trucksToAdd = new List<Truck>();

			foreach (var dispatcher in dispatchers) // For each dispatcher
			{
				for (int i = 0; i < 5; i++) // add 5 trucks
				{
					var randomMake = makeModelDictionary.Keys.ElementAt(new Random().Next(makeModelDictionary.Count));
					var models = makeModelDictionary[randomMake];

					var randomModel = models[new Random().Next(models.Count)];

					int truckCounter = 1; 

					var truck = new Truck
					{
						Id = Guid.NewGuid(),
						TruckNumber = $"T000{truckCounter++}", 
						Make = randomMake,
						Model = randomModel,
						Year = new Random().Next(2000, 2024),
						DispatcherId = dispatcher.Id,
						IsAvailable = true,
						IsActive = true
					};

					trucksToAdd.Add(truck);

				}
			}

			await context.Trucks.AddRangeAsync(trucksToAdd);
			await context.SaveChangesAsync();
		}

		public static async Task SeedDrivers(UserManager<BaseUser> userManager, IServiceProvider serviceProvider)
		{
			await using var context = serviceProvider.GetRequiredService<LoadVantageDbContext>();

			// Check if drivers are already seeded
			if (context.Drivers.Any())
			{
				return; // Drivers are already seeded
			}


			var dispatchers = await userManager.Users
				.OfType<Dispatcher>()
				.ToListAsync();

			var driversToAdd = new List<Driver>();

			var firstNames = new[] { "John", "Mike", "James", "Bob", "Anakin", "Thomas", "Emily", "Daniel", "Luke", "Ted" };
			var lastNames = new[] { "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Skywalker", "Moore", "Bundy", "Doe" };
			var licenseNumbers = new[] { "ABC123456", "XYZ569978", "LMN986676", "DEF345619", "QRS678910" };

			foreach (var dispatcher in dispatchers) // for every dispatcher 
			{
				for (int i = 0; i < 5; i++) // add 5 drivers 
				{
					// Generate a random first name, last name, and license number
					var randomFirstName = firstNames[new Random().Next(firstNames.Length)];
					var randomLastName = lastNames[new Random().Next(lastNames.Length)];
					var randomLicenseNumber = licenseNumbers[new Random().Next(licenseNumbers.Length)];

					var driver = new Driver
					{
						DriverId = Guid.NewGuid(),
						FirstName = randomFirstName,
						LastName = randomLastName,
						LicenseNumber = randomLicenseNumber,
						DispatcherId = dispatcher.Id,
						IsAvailable = true,
						IsFired = false,
						IsBusy = false
					};

					driversToAdd.Add(driver);
				}
			}

			await context.Drivers.AddRangeAsync(driversToAdd);
			await context.SaveChangesAsync();
		}

	}
}
