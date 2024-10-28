using LoadVantage.Areas.Dispatcher.Contracts;
using LoadVantage.Areas.Dispatcher.Models;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace LoadVantage.Areas.Dispatcher.Services
{
    public class DispatcherService(UserManager<User> userManager) : IDispatcherService
    {
        public async Task<DispatcherViewModel> GetDispatcherInformationAsync(string userId)
        {
            var dispatcher = await userManager.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (dispatcher == null)
            {
                return null; // Or throw an exception
            }

            var foundDispatcher = new DispatcherViewModel
            {
                Username = dispatcher.UserName,
                Email = dispatcher.Email,
                FirstName = dispatcher.FirstName,
                LastName = dispatcher.LastName,
                CompanyName = dispatcher.CompanyName,
                PhoneNumber = dispatcher.PhoneNumber,
                Position = dispatcher.Position
            };

            return foundDispatcher;
        }
    }
}

