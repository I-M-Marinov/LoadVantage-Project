using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IAdminUserService
	{
		/// <summary>
		/// Retrieves the current Administrator 
		/// </summary>
		Task<BaseUser> GetCurrentAdminAsync();
		/// <summary>
		/// Retrieves all the information for an Administrator by Guid Id
		/// </summary>
		Task<BaseUser> GetAdminByIdAsync(Guid adminId);
		/// <summary>
		/// Retrieves the current Administrator 
		/// </summary>
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
		/// <summary>
		/// Retrieves the current Administrator's claims
		/// </summary>
		Task<IEnumerable<Claim>> GetAdminClaimsAsync(BaseUser administrator);
		/// <summary>
		/// Updates a user's image 
		/// </summary>
		Task UpdateUserImageAsync(Guid userId, IFormFile file);
		/// <summary>
		/// Deletes a user's image 
		/// </summary>
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
