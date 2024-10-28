using System.Security.Claims;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static LoadVantage.Common.GeneralConstants;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.GeneralConstants.SecretString;

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
    }
}
