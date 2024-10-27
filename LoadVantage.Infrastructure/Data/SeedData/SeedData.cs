using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static LoadVantage.Common.GeneralConstants;
using static LoadVantage.Common.GeneralConstants.UserRoles;

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
					Role = role!
				};

				var result = await userManager.CreateAsync(adminUser, adminPassword!);

				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, AdminRoleName);
				}
				else
				{
					var errorMessages = result.Errors.Select(e => e.Description);
					throw new Exception($"Failed to create admin user: {string.Join(", ", errorMessages)}");
				}
			}
			
		}
	}
}
