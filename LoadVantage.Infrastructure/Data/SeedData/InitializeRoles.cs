using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using static LoadVantage.Common.GeneralConstants;

namespace LoadVantage.Infrastructure.Data.SeedData
{
    public static class InitializeRoles
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
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
    }
}
