using LoadVantage.Areas.Admin.Models.User;
using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IUserManagementService
    {
	    /// <summary>
	    /// Retrieves all users with all their information and paginates the information based on the pageSize
	    /// </summary>
		Task<List<UserManagementViewModel>> GetUsersAsync(int pageNumber, int pageSize);
	    /// <summary>
	    /// Creates an Administrator 
	    /// </summary>
		Task<AdminCreateUserViewModel> CreateAdministratorAsync(AdminCreateUserViewModel model);
	    /// <summary>
	    /// Creates a User 
	    /// </summary>
		Task<AdminCreateUserViewModel> CreateUserAsync(AdminCreateUserViewModel userViewModel);
		/// <summary>
		/// Deactivates a User, anonymizing him ( deleting delicate information ) leaving a shell of an account with just a username. Locks the account. 
		/// </summary>
		Task<bool> DeactivateUserAsync(Guid userId);
		/// <summary>
		/// Reactivates a User, that was previously deactivated. Unlocks the account.
		/// </summary>
		Task<bool> ReactivateUserAsync(Guid userId);
		/// <summary>
		/// Updates the information of a User
		/// </summary>
		Task<bool> UpdateUserAsync(AdminEditUserViewModel model);
		/// <summary>
		/// Searches in the user table by FirstName,LastName,Email,PhoneNumber,Username,Position or CompanyName.
		/// </summary>
		Task<List<UserManagementViewModel>> SearchUsersAsync(string searchTerm);
		/// <summary>
		/// Resets the password for a user back to the predefined default password.
		/// </summary>
		Task<IdentityResult> ResetPasswordToDefaultAsync(Guid userId);
    }
}
