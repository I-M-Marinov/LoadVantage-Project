﻿using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.Profile;
using System.Threading.Tasks;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Core.Contracts
{
    public interface IProfileService
    {
        Task<ProfileViewModel?> GetUserInformation(Guid userId);
        Task<ProfileViewModel> UpdateProfileInformation(ProfileViewModel model, Guid userId);
        Task<bool> IsUsernameTakenAsync(string username, Guid currentUserId);
        Task UpdateUserClaimsAsync(User user, ProfileViewModel model);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

    }
}