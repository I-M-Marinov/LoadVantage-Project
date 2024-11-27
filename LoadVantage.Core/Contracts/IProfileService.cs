using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.Profile;
using System.Threading.Tasks;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Core.Contracts
{
    public interface IProfileService
    {
	    /// <summary>
	    /// Retrieves the user information.
	    /// </summary>
		Task<ProfileViewModel?> GetUserInformation(Guid userId);
	    /// <summary>
	    /// Updates the user information.
	    /// </summary>
		Task<ProfileViewModel> UpdateProfileInformation(ProfileViewModel model, Guid userId);
		/// <summary>
		/// Changes the user's password.
		/// </summary>
		Task<IdentityResult> ChangePasswordAsync(BaseUser user, string currentPassword, string newPassword);
    }
}
