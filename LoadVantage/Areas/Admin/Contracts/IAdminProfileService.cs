using LoadVantage.Areas.Admin.Models.Profile;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Areas.Admin.Contracts
{
    public interface IAdminProfileService
    {
	    /// <summary>
	    /// Retrieves all the information for an Administrator 
	    /// </summary>
		Task<AdminProfileViewModel?> GetAdminInformation(Guid adminId);
	    /// <summary>
	    /// Updates the profile information for an Administrator
	    /// </summary>
		Task<AdminProfileViewModel> UpdateProfileInformation(AdminProfileViewModel model, Guid adminId);
	    /// <summary>
	    /// Changes the password for an Administrator 
	    /// </summary>
		Task<IdentityResult> ChangePasswordAsync(Administrator admin, string currentPassword, string newPassword);

    }
}
