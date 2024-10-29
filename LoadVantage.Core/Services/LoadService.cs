using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using LoadVantage.Core.Models;
using LoadVantage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Core.Services
{
    public class LoadService(LoadVantageDbContext context, UserManager<User> userManager) : ILoadService
    {

    }
}
