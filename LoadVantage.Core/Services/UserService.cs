using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using static LoadVantage.Common.GeneralConstants.UserRoles;


#nullable disable

namespace LoadVantage.Core.Services
{
    [Authorize(Roles = nameof(Administrator))]
    public class UserService(UserManager<User> userManager, RoleManager<Role> roleManager) : IUserService
    {
        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return  await userManager.Users
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetDispatchersAsync()
        {
            return await userManager.Users
                .Where(u => u is Dispatcher)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetBrokersAsync()
        {
            return await userManager.Users
                .Where(u => u is Broker)
                .ToListAsync();
        }
        public async Task UpdateUserPositionAsync(Guid userId, string position)
        {
            var user = await GetUserByIdAsync(userId);

            if (user != null)
            {
                user.Position = position;
                await userManager.UpdateAsync(user);
            }
        }

        public async Task AssignUserRoleAsync(Guid userId, string role)
        {
            var user = await GetUserByIdAsync(userId);
            var isAlreadyInRole = await userManager.IsInRoleAsync(user, role);

            if (user != null && !isAlreadyInRole)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }

}
