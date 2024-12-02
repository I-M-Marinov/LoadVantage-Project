using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.Profile;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Areas.Admin.Services
{
    public class AdminProfileService : IAdminProfileService
    {
        private readonly IUserService userService;
        private readonly IAdminUserService adminUserService;
        private readonly UserManager<BaseUser> adminUserManager;
        private readonly LoadVantageDbContext context;
        private readonly SignInManager<BaseUser> adminSignInManager;
        private readonly IProfileHelperService profileHelperService;

        public AdminProfileService(LoadVantageDbContext _context, IUserService _userService, UserManager<BaseUser> _adminUserManager,

			SignInManager<BaseUser> _adminSignInManager, IProfileHelperService _profileHelperService, IAdminUserService _adminUserService)
        {
            userService = _userService;
            adminUserService = _adminUserService;
            adminUserManager = _adminUserManager;
            context = _context;
            adminSignInManager = _adminSignInManager;
            profileHelperService = _profileHelperService;
        }

        public async Task<AdminProfileViewModel?> GetAdminInformation(Guid adminId)
        {
            var user = await userService.GetUserByIdAsync(adminId);

            if (user == null)
            {
                throw new Exception(UserNotFound);
            }

            var userImageUrl = await context.UsersImages
                .Where(ui => ui.Id == user.UserImageId)
                .Select(ui => ui.ImageUrl)
                .FirstOrDefaultAsync();

            var adminProfile = new AdminProfileViewModel()
            {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName!,
                Position = user.Position!,
                CompanyName = user.CompanyName!,
                PhoneNumber = user.PhoneNumber!,
                Email = user.Email!,
                UserImageUrl = userImageUrl
            };

            adminProfile.AdminImageFileUploadModel = new AdminImageFileUploadModel();
            adminProfile.AdminChangePasswordViewModel = new AdminChangePasswordViewModel();


            return adminProfile;
        }
        public async Task<AdminProfileViewModel> UpdateProfileInformation(AdminProfileViewModel model, Guid adminId)
        {
            var admin = await adminUserService.GetAdminByIdAsync(adminId);

            if (admin == null)
            {
                throw new Exception(UserNotFound);
            }

            if (admin.Id != Guid.Parse(model.Id) || admin.Position != model.Position) // If the user tries to change position or id from the hidden fields returns the same model 
            {
                return model;
            }

            if (await profileHelperService.IsEmailTakenAsync(model.Email, admin.Id))
            {
                throw new InvalidOperationException(EmailIsAlreadyTaken);
            }

            if (await profileHelperService.IsUsernameTakenAsync(model.Username, admin.Id))
            {
                throw new InvalidDataException(UserNameIsAlreadyTaken);
            }

            if (AreUserPropertiesEqual(admin, model))
            {
                throw new Exception(NoChangesMadeToProfile);
            }

            await UpdateAdminClaimsAsync(admin, model);

            // Update the user's properties ( NOT THE ID ) 

            admin.FirstName = model.FirstName;
            admin.LastName = model.LastName;
            admin.UserName = model.Username;
            admin.CompanyName = model.CompanyName;
            admin.PhoneNumber = model.PhoneNumber;
            admin.Email = model.Email;

            admin.NormalizedUserName = model.Username.ToUpperInvariant(); // normalized username
            admin.NormalizedEmail = model.Email.ToUpperInvariant(); // normalized email address

            var result = await adminUserManager.UpdateAsync(admin);

            if (!result.Succeeded)
            {
                throw new Exception(UserProfileUpdateFailed);
            }

            var userImage = await context.UsersImages.SingleOrDefaultAsync(u => u.Id == admin.UserImageId);

            return new AdminProfileViewModel
			{
                Id = admin.Id.ToString(),
                Username = admin.UserName,
                CompanyName = admin.CompanyName,
                Position = admin.Position,
                PhoneNumber = admin.PhoneNumber,
                Email = admin.Email,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                UserImageUrl = userImage.ImageUrl
            };
        }

        public async Task<IdentityResult> ChangePasswordAsync(Administrator admin, string currentPassword, string newPassword)
        {
            if (admin == null)
            {
                throw new ArgumentNullException(nameof(admin), UserCannotBeNull);
            }

            if (currentPassword == newPassword)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = CurrentAndNewPasswordCannotMatch
                });
            }

            var result = await adminUserManager.ChangePasswordAsync(admin, currentPassword, newPassword);

            if (result.Succeeded)
            {
                await adminSignInManager.SignOutAsync(); // Log Out 
                await adminSignInManager.PasswordSignInAsync(admin, newPassword, false,
                    false); // Log back in again with the new password
            }

            return result;
        }

        private async Task UpdateAdminClaimsAsync(BaseUser admin, AdminProfileViewModel model)
        {
	        var existingClaims = await adminUserService.GetAdminClaimsAsync(admin);
	        var claimsToAddOrUpdate = profileHelperService.GetMissingClaims(existingClaims, model.FirstName, model.LastName, model.Username,
		        model.Position);

	        if (claimsToAddOrUpdate.Any())
	        {
		        await adminUserManager.RemoveClaimsAsync(admin, existingClaims.Where(c => claimsToAddOrUpdate.Any(newClaim => newClaim.Type == c.Type)));

		        await Task.Delay(500); // Ensuring removal happens first.

		        await adminUserManager.AddClaimsAsync(admin, claimsToAddOrUpdate);
	        }

        }

        private bool AreUserPropertiesEqual(BaseUser admin, AdminProfileViewModel model)
        {
            return admin.Id == Guid.Parse(model.Id) &&
                   admin.Position == model.Position &&
                   admin.FirstName == model.FirstName &&
                   admin.LastName == model.LastName &&
                   admin.UserName == model.Username &&
                   admin.CompanyName == model.CompanyName &&
                   admin.PhoneNumber == model.PhoneNumber &&
                   admin.Email == model.Email;
        }
    }


}
