using LoadVantage.Areas.Admin.Models.Profile;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Areas.Admin.Contracts
{
    public interface IAdminProfileService
    {
        Task<AdminProfileViewModel?> GetAdminInformation(Guid adminId);
        Task<AdminProfileViewModel> UpdateProfileInformation(AdminProfileViewModel model, Guid adminId);
        Task<IdentityResult> ChangePasswordAsync(Administrator admin, string currentPassword, string newPassword);

    }
}
