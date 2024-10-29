using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Core.Contracts
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetDispatchersAsync();
        Task<IEnumerable<User>> GetBrokersAsync();
        Task UpdateUserPositionAsync(Guid userId, string position);
        Task AssignUserRoleAsync(Guid userId, string role);
    }

}
