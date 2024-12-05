using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IAdminUserService
	{
		Task<BaseUser> GetCurrentAdminAsync();
		Task<BaseUser> GetAdminByIdAsync(Guid adminId);
		Task<Administrator> GetCurrentAdministratorAsync();
		/// <summary>
																/// Retrieves all registered Users
																/// </summary>
		Task<IEnumerable<BaseUser>> GetAllUsersAsync();
		/// <summary>
		/// Retrieves all registered Dispatchers 
		/// </summary>
		Task<IEnumerable<BaseUser>> GetDispatchersAsync();
		/// <summary>
		/// Retrieves all registered Brokers 
		/// </summary>
		Task<IEnumerable<BaseUser>> GetBrokersAsync();
		Task<IEnumerable<Claim>> GetAdminClaimsAsync(BaseUser administrator);
		Task UpdateUserImageAsync(Guid userId, IFormFile file);
		Task DeleteUserImageAsync(Guid userId, Guid imageId);
        /// <summary>
        /// Retrieve the all current users that have a company name added to their account as a list of BaseUser
        /// </summary>
        Task<IEnumerable<BaseUser>> GetAllUsersFromACompany();
        /// <summary>
        /// Deletes the current stored password for a user
        /// </summary>
        Task<IdentityResult> DeleteUserPassword(BaseUser user);
        /// <summary>
        /// Resets the password for a user to the default reset password
        /// </summary>
        Task<IdentityResult> AddUserDefaultPassword(BaseUser user);
        /// <summary>
        /// Retrieve the total user count 
        /// </summary>
        Task<int> GetUserCountAsync();
        /// <summary>
        /// Retrieve the total Dispatcher count
        /// </summary>
        Task<int> GetDispatcherCountAsync();
        /// <summary>
        /// Retrieve the total Broker count
        /// </summary>
        Task<int> GetBrokerCountAsync();
	}
}
