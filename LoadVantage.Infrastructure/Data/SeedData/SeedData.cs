using System.Security.Claims;
using LoadVantage.Common.Enums;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static LoadVantage.Common.GeneralConstants;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.GeneralConstants.SecretString;
using Microsoft.EntityFrameworkCore;
using System;
using LoadVantage.Infrastructure.Data.Contracts;

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
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
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
				var adminUser = new User
				{
					UserName = adminUserName,
					Email = adminEmail,
					FirstName = adminFirstName!,
					LastName = adminLastName!,
					Position = AdminPositionName,
                    PhoneNumber = adminPhoneNumber!,
                    CompanyName = adminCompany,
                    Role = role!
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
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            Role? userRole = await roleManager.FindByNameAsync(UserRoleName);

            var dispatchers = new List<User>
            {
                new User()
                {
                    UserName = "dispatcher1",
                    Email = "dispatcher1@gmail.com",
                    FirstName = "John",
                    LastName = "Johnson",
                    CompanyName = "Coyote Logistics",
                    PhoneNumber = "+1-800-654-1234",
                    Position = DispatcherPositionName,
                    Role = userRole!
                },
                new User()
                {
                    UserName = "dispatcher2",
                    Email = "dispatcher2@gmail.com",
                    FirstName = "Steven",
                    LastName = "Stevenson",
                    CompanyName = "CH Robinson Worldwide",
                    PhoneNumber = "+1-225-968-4692",
                    Position = DispatcherPositionName,
                    Role = userRole!
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
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            Role? userRole = await roleManager.FindByNameAsync(UserRoleName);

            var brokers = new List<User>
            {
                new User()
                {
                    UserName = "broker1",
                    Email = "broker1@gmail.com",
                    FirstName = "Richard",
                    LastName = "Richardson",
                    CompanyName = "Echo Global Logistics",
                    PhoneNumber = "+1-708-953-7412",
                    Position = BrokerPositionName,
                    Role = userRole!
                },
                new User()
                {
                    UserName = "broker2",
                    Email = "broker2@gmail.com",
                    FirstName = "Donald",
                    LastName = "Gardner",
                    CompanyName = "Pitbull Freight Co.",
                    PhoneNumber = "+1-300-852-7391",
                    Position = BrokerPositionName,
                    Role = userRole!
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

        public static async Task SeedCreatedLoads(IServiceProvider serviceProvider, IConfiguration configuration, UserManager<User> userManager)
        {
            // Get the DbContext
            await using var context = serviceProvider.GetRequiredService<LoadVantageDbContext>();

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
                ("New York", "NY"),
                ("Atlanta", "GA"),
                ("Houston", "TX"),
                ("Los Angeles", "CA"),
                ("Denver", "CO"),
                ("Miami", "FL"),
                ("Seattle", "WA"),
                ("Phoenix", "AZ"),
                ("Reno", "NV"),
                ("Indianapolis", "IN")
            };

            List<Load> GenerateRandomLoads()
            {
                var loads = new List<Load>();

                for (int i = 0; i < 6; i++) // Generate six random loads
                {
                    // Pick random origin and destination that are not the same
                    var origin = locations[random.Next(locations.Count)];
                    (string City, string State) destination;
                    do
                    {
                        destination = locations[random.Next(locations.Count)];
                    } while (origin == destination); // Ensure origin and destination are different

                    loads.Add(new Load
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        OriginCity = origin.City,
                        OriginState = origin.State,
                        DestinationCity = destination.City,
                        DestinationState = destination.State,
                        PickupTime = DateTime.Now.AddDays(random.Next(1, 10)), // Pickup date 1 to 10 days from now
                        DeliveryTime = DateTime.Now.AddDays(random.Next(11, 20)), // Delivery date 11 to 20 days from now
                        Distance = 0,
                        Price = random.Next(1000, 6000), // Random price 
                        Weight = random.Next(10000, 45000), // Random weight 
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
                    broker.Loads.Add(load);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
