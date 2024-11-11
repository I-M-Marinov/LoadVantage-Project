using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;

namespace LoadVantage.Core.Contracts
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(Guid userId);
        Task<ProfileViewModel> GetUserInformation(Guid userId);
		Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetDispatchersAsync();
        Task<IEnumerable<User>> GetBrokersAsync();
        Task UpdateUserPositionAsync(Guid userId, string position);
        Task AssignUserRoleAsync(Guid userId, string role);
        Task UpdateUserImageAsync(Guid userId, IFormFile file);
        Task DeleteUserImageAsync(Guid userId, Guid imageUrl);


    }

}
