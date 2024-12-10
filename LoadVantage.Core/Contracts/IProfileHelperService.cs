using System.Security.Claims;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Core.Contracts
{
	public interface IProfileHelperService
	{
		/// <summary>
		/// Verifies if a username is already in use or not     
		/// </summary>
		Task<bool> IsUsernameTakenAsync(string username, Guid currentUserId);
		/// <summary>
		/// Verifies if an email is already in use or not     
		/// </summary>
		Task<bool> IsEmailTakenAsync(string email, Guid currentUserId);
		/// <summary>
		/// Retrieves all the claims for a User      
		/// </summary>
		Task<IEnumerable<Claim>> GetClaimsAsync(BaseUser user);
		/// <summary>
		/// Matches the User's claims against a list of claims and if there are any missing they are returned as a List      
		/// </summary>
		List<Claim> GetMissingClaims(IEnumerable<Claim> existingClaims, string firstName, string lastName, string userName, string userPosition);
		/// <summary>
		/// Finds the User by his username 
		/// </summary>
		Task<BaseUser?> FindUserByUsernameAsync(string username);
	}
}
