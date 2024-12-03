using Microsoft.AspNetCore.Identity;

using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.Profile;
using LoadVantage.Infrastructure.Data.Contracts;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Areas.Admin.Services
{
    public class AdminProfileService : IAdminProfileService
    {
        private readonly IUserService userService;
        private readonly IAdminUserService adminUserService;
        private readonly UserManager<BaseUser> adminUserManager;
        private readonly SignInManager<BaseUser> adminSignInManager;
        private readonly IProfileHelperService profileHelperService;
        private readonly IHtmlSanitizerService htmlSanitizer;

        public AdminProfileService(
	        IUserService _userService,
	        IAdminUserService _adminUserService,
			UserManager<BaseUser> _adminUserManager,
			SignInManager<BaseUser> _adminSignInManager, 
	        IProfileHelperService _profileHelperService, 
	        IHtmlSanitizerService _htmlSanitizer)
        {
            userService = _userService;
            adminUserService = _adminUserService;
            adminUserManager = _adminUserManager;
            adminSignInManager = _adminSignInManager;
            profileHelperService = _profileHelperService;
            htmlSanitizer = _htmlSanitizer;
        }

        public async Task<AdminProfileViewModel?> GetAdminInformation(Guid adminId)
        {
            var user = await userService.GetUserByIdAsync(adminId);

            if (user == null)
            {
                throw new Exception(UserNotFound);
            }

            var adminImageUrl = await userService.GetUserImageUrlAsync(adminId);

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
                UserImageUrl = adminImageUrl
			};

            adminProfile.AdminImageFileUploadModel = new AdminImageFileUploadModel();
            adminProfile.AdminChangePasswordViewModel = new AdminChangePasswordViewModel();


            return adminProfile;
        }
        public async Task<AdminProfileViewModel> UpdateProfileInformation(AdminProfileViewModel model, Guid adminId)
        {
            var admin = await adminUserService.GetAdminByIdAsync(adminId);

            var sanitizedFirstName = htmlSanitizer.Sanitize(model.FirstName);
            var sanitizedLastName = htmlSanitizer.Sanitize(model.LastName);
            var sanitizedUserName = htmlSanitizer.Sanitize(model.Username);
            var sanitizedCompanyName = htmlSanitizer.Sanitize(model.CompanyName);
            var sanitizedPhoneNumber = htmlSanitizer.Sanitize(model.PhoneNumber);
            var sanitizedEmail = htmlSanitizer.Sanitize(model.Email);

			if (admin == null)
            {
                throw new Exception(UserNotFound);
            }

            if (admin.Id != Guid.Parse(model.Id) || admin.Position != model.Position) // If the user tries to change position or id from the hidden fields returns the same model 
            {
                return model;
            }

            if (await profileHelperService.IsEmailTakenAsync(sanitizedEmail, admin.Id))
            {
                throw new InvalidOperationException(EmailIsAlreadyTaken);
            }

            if (await profileHelperService.IsUsernameTakenAsync(sanitizedUserName, admin.Id))
            {
                throw new InvalidDataException(UserNameIsAlreadyTaken);
            }

            var sanitizedModel = new AdminProfileViewModel()
            {
	            Id = model.Id,
	            Username = sanitizedUserName,
	            Email = sanitizedEmail,
	            FirstName = sanitizedFirstName,
	            LastName = sanitizedLastName,
	            CompanyName = sanitizedCompanyName,
	            Position = model.Position,
	            PhoneNumber = sanitizedPhoneNumber
            };

			if (AreUserPropertiesEqual(admin, sanitizedModel))
            {
                throw new Exception(NoChangesMadeToProfile);
            }

            await UpdateAdminClaimsAsync(admin, sanitizedModel);

            // Update the user's properties ( NOT THE ID ) 

            admin.FirstName = sanitizedFirstName;
            admin.LastName = sanitizedLastName;
            admin.UserName = sanitizedUserName;
            admin.CompanyName = sanitizedCompanyName;
            admin.PhoneNumber = sanitizedPhoneNumber;
            admin.Email = sanitizedEmail;

            admin.NormalizedUserName = sanitizedModel.Username.ToUpperInvariant(); // normalized username
            admin.NormalizedEmail = sanitizedModel.Email.ToUpperInvariant(); // normalized email address

            var result = await adminUserManager.UpdateAsync(admin);

            if (!result.Succeeded)
            {
                throw new Exception(UserProfileUpdateFailed);
            }

            var adminImageUrl = await userService.GetUserImageUrlAsync(adminId);

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
                UserImageUrl = adminImageUrl
			};
        }
        public async Task<IdentityResult> ChangePasswordAsync(Administrator admin, string currentPassword, string newPassword)
        {
	        var sanitizedCurrentPassword = htmlSanitizer.Sanitize(currentPassword);
	        var sanitizedNewPassword = htmlSanitizer.Sanitize(newPassword);

            if (admin == null)
            {
                throw new ArgumentNullException(nameof(admin), UserCannotBeNull);
            }

            if (sanitizedCurrentPassword == sanitizedNewPassword)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = CurrentAndNewPasswordCannotMatch // this happened to me before :) 
                });
            }

            var result = await adminUserManager.ChangePasswordAsync(admin, sanitizedCurrentPassword, sanitizedNewPassword);

            if (result.Succeeded)
            {
                await adminSignInManager.SignOutAsync(); // Log Out 
                await adminSignInManager.PasswordSignInAsync(admin, sanitizedNewPassword, false,
                    false); // Log back in again with the new password
            }

            return result;
        }
        private async Task UpdateAdminClaimsAsync(BaseUser admin, AdminProfileViewModel model)
        {
	        var sanitizedFirstName = htmlSanitizer.Sanitize(model.FirstName);
	        var sanitizedLastName = htmlSanitizer.Sanitize(model.LastName);
	        var sanitizedUserName = htmlSanitizer.Sanitize(model.Username);

			var existingClaims = await adminUserService.GetAdminClaimsAsync(admin);
	        var claimsToAddOrUpdate = profileHelperService.GetMissingClaims(existingClaims, sanitizedFirstName, sanitizedLastName, sanitizedUserName, model.Position);

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
