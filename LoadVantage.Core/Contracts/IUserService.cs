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
		Task<User?> GetUserByIdAsync(Guid userId);
	    /// <summary>
	    /// Retrieves the currently authorized User or returns null if not authorized.
	    /// </summary>
		Task<User?> GetCurrentUserAsync();
	    /// <summary>
	    /// Finds the User by his email address
	    /// </summary>
		Task<User?> FindUserByEmailAsync(string email);
	    /// <summary>
	    /// Finds the User by his username 
	    /// </summary>
		Task<User?> FindUserByUsernameAsync(string username);
	    /// <summary>
	    /// Creates a new User 
	    /// </summary>
		Task<IdentityResult> CreateUserAsync(User user, string password);
	    /// <summary>
	    /// Assigns the User role 
	    /// </summary>
		Task<IdentityResult> AssignUserRoleAsync(User user, string role);
	    /// <summary>
	    /// Retrieve all the claims for a user 
	    /// </summary>
	    Task<IEnumerable<Claim>> GetUserClaimsAsync(User user);
	    /// <summary>
	    /// Adds a User Claim  
	    /// </summary>
		Task<IdentityResult> AddUserClaimAsync(User user, Claim claim);
		/// <summary>
		/// Adds multiple User Claims 
		/// </summary>
		Task<IdentityResult> AddUserClaimsAsync(User user, IEnumerable<Claim> claims);
		/// <summary>
		/// Gets the User information and maps it to the model for the profile section
		/// </summary>
		Task<ProfileViewModel> GetUserInformation(Guid userId);
		/// <summary>
		/// Gets the User information and maps some of it the model for the chat section
		/// </summary>
		Task<UserChatViewModel> GetChatUserInfoAsync(Guid brokerId);
		/// <summary>
		/// Retrieves all registered Users
		/// </summary>
		Task<IEnumerable<User>> GetAllUsersAsync();
		/// <summary>
		/// Retrieves all registered Dispatchers 
		/// </summary>
		Task<IEnumerable<User>> GetDispatchersAsync();
		/// <summary>
		/// Retrieves all registered Brokers 
		/// </summary>
		Task<IEnumerable<User>> GetBrokersAsync();
		/// <summary>
		/// Updates the User's position  
		/// </summary>
		Task UpdateUserPositionAsync(Guid userId, string position);
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

    }

}
