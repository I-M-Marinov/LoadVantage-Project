using System.Security.Claims;
using LoadVantage.Core.Models.Chat;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Core.Contracts
{
    public interface IUserService
    {
	    /// <summary>
	    /// Retrieves a user by his id
	    /// </summary>
		Task<BaseUser?> GetUserByIdAsync(Guid userId);
	    /// <summary>
	    /// Retrieves the currently authorized User or returns null if not authorized.
	    /// </summary>
		Task<BaseUser?> GetCurrentUserAsync();
	    /// <summary>
	    /// Finds the User by his email address
	    /// </summary>
		Task<BaseUser?> FindUserByEmailAsync(string email);
	    /// <summary>
	    /// Retrieve all the claims for a user 
	    /// </summary>
	    Task<IEnumerable<Claim>> GetUserClaimsAsync(BaseUser user);
	    /// <summary>
	    /// Adds a User Claim  
	    /// </summary>
		Task<IdentityResult> AddUserClaimAsync(BaseUser user, Claim claim);
		/// <summary>
		/// Adds multiple User Claims 
		/// </summary>
		Task<IdentityResult> AddUserClaimsAsync(BaseUser user, IEnumerable<Claim> claims);
		/// <summary>
		/// Gets the User information and maps it to the model for the profile section
		/// </summary>
		Task<ProfileViewModel> GetUserInformation(Guid userId);
		/// <summary>
		/// Gets the User information and maps some of it the model for the chat section
		/// </summary>
		Task<UserChatViewModel> GetChatUserInfoAsync(Guid brokerId);
		/// <summary>
		/// Updates the User's image  
		/// </summary>
		Task UpdateUserImageAsync(Guid userId, IFormFile file);
		/// <summary>
		/// Removes the User's image and sets the default user image in its place
		/// </summary>
		Task DeleteUserImageAsync(Guid userId, Guid imageUrl);
		/// <summary>
		/// Retrieves the user image url string by user Id
		/// </summary>
		Task<string> GetUserImageUrlAsync(Guid userId);
		Task<Guid> GetOrCreateDefaultImageAsync();
		Task<BaseUser> FindUserByUsernameAsync(string username);
		Task<IdentityResult> CreateUserAsync(BaseUser user, string password);
		Task<IdentityResult> AssignUserRoleAsync(BaseUser user, string role);






    }

}
