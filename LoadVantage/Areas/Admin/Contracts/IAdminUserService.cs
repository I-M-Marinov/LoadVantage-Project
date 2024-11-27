using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IAdminUserService
	{
		Task<BaseUser> GetCurrentAdminAsync();
		Task<BaseUser> GetAdminByIdAsync(Guid adminId);
		Task<BaseUser> GetCurrentAdministratorAsync();
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
		/// Updates the User's position  
		/// </summary>
		Task UpdateUserPositionAsync(Guid userId, string position);
		Task<IEnumerable<Claim>> GetAdminClaimsAsync(BaseUser administrator);
		Task UpdateUserImageAsync(Guid userId, IFormFile file);
		Task DeleteUserImageAsync(Guid userId, Guid imageId);

	}
}
