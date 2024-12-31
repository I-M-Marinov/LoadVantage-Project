using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Image;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data.Models;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Core.Services
{
	public class ProfileService : IProfileService
	{
		private readonly IUserService userService;
		private readonly UserManager<BaseUser> userManager;
		private readonly SignInManager<BaseUser> signInManager;
		private readonly IProfileHelperService profileHelperService;
		private readonly IHtmlSanitizerService htmlSanitizer;


		public ProfileService(
			IUserService _userService, 
			UserManager<BaseUser> _userManager,
			SignInManager<BaseUser> _signInManager, 
			IProfileHelperService _profileHelperService,
			IHtmlSanitizerService _sanitizerService)
		{
			userService = _userService;
			userManager = _userManager;
			signInManager = _signInManager;
			profileHelperService = _profileHelperService;
			htmlSanitizer = _sanitizerService;
		}

		public async Task<ProfileViewModel?> GetUserInformation(Guid userId)
		{
			var user = await userService.GetUserByIdAsync(userId);

			if (user == null)
			{
				throw new Exception(UserNotFound);
			}

			var userImageUrl = await userService.GetUserImageUrlAsync(userId);


			var profile = new ProfileViewModel()
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

			profile.ImageFileUploadModel = new ImageFileUploadModel();
			profile.ChangePasswordViewModel = new ChangePasswordViewModel();


			return profile;
		}
		public async Task<ProfileViewModel> UpdateProfileInformation(ProfileViewModel model, Guid userId)
		{
			var user = await userService.GetUserByIdAsync(userId);

			var sanitizedFirstName = htmlSanitizer.Sanitize(model.FirstName);
			var sanitizedLastName = htmlSanitizer.Sanitize(model.LastName);
			var sanitizedUserName = htmlSanitizer.Sanitize(model.Username);
			var sanitizedCompanyName = htmlSanitizer.Sanitize(model.CompanyName);
			var sanitizedPhoneNumber = htmlSanitizer.Sanitize(model.PhoneNumber);
			var sanitizedEmail = htmlSanitizer.Sanitize(model.Email);



			if (user.Id != Guid.Parse(model.Id) || user.Position != model.Position) // If the user tries to change position or id from the hidden fields returns the same model 
			{
				return model;
			}

			if (await profileHelperService.IsEmailTakenAsync(sanitizedEmail, user.Id))
			{
				throw new InvalidOperationException(EmailIsAlreadyTaken);
			}

			if (await profileHelperService.IsUsernameTakenAsync(sanitizedUserName, user.Id))
			{
				throw new InvalidDataException(UserNameIsAlreadyTaken);
			}

			var sanitizedModel = new ProfileViewModel
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

			if (AreUserPropertiesEqual(user, sanitizedModel))
			{
				throw new Exception(NoChangesMadeToProfile);
			}

			await UpdateUserClaimsAsync(user, sanitizedModel);

			// Update the user's properties ( NOT THE ID ) 

			user.FirstName = sanitizedFirstName;
			user.LastName = sanitizedLastName;
			user.UserName = sanitizedUserName;
			user.CompanyName = sanitizedCompanyName;
			user.PhoneNumber = sanitizedPhoneNumber;
			user.Email = sanitizedEmail;

			user.NormalizedUserName = sanitizedUserName.ToUpperInvariant(); // normalized username
			user.NormalizedEmail = sanitizedEmail.ToUpperInvariant(); // normalized email address

			var result = await userManager.UpdateAsync(user);

			if (!result.Succeeded)
			{
				throw new Exception(UserProfileUpdateFailed);
			}

			var userImageUrl = await userService.GetUserImageUrlAsync(userId);

			return new ProfileViewModel
			{
				Id = user.Id.ToString(),
				Username = user.UserName,
				CompanyName = user.CompanyName,
				Position = user.Position,
				PhoneNumber = user.PhoneNumber,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				UserImageUrl = userImageUrl
			};
		}
		public async Task UpdateUserClaimsAsync(BaseUser user, ProfileViewModel model)
		{
			var sanitizedFirstName = htmlSanitizer.Sanitize(model.FirstName);
			var sanitizedLastName = htmlSanitizer.Sanitize(model.LastName);
			var sanitizedUserName = htmlSanitizer.Sanitize(model.Username);


			var existingClaims = await profileHelperService.GetClaimsAsync(user);
			var claimsToAddOrUpdate = profileHelperService.GetMissingClaims(existingClaims, sanitizedFirstName, sanitizedLastName, sanitizedUserName, model.Position) ?? new List<Claim>();

			if (claimsToAddOrUpdate.Any())
			{
				await userManager.RemoveClaimsAsync(user, existingClaims.Where(c => claimsToAddOrUpdate.Any(newClaim => newClaim.Type == c.Type)));

				await Task.Delay(500); // Ensuring removal happens first.

				await userManager.AddClaimsAsync(user, claimsToAddOrUpdate);
			}

		}
		public async Task<IdentityResult> ChangePasswordAsync(BaseUser user, string currentPassword, string newPassword)
		{
			var sanitizedCurrentPassword = htmlSanitizer.Sanitize(currentPassword);
			var sanitizedNewPassword = htmlSanitizer.Sanitize(newPassword);

			if (user == null)
			{
				throw new ArgumentNullException(nameof(user), UserCannotBeNull);
			}

			if (sanitizedCurrentPassword == sanitizedNewPassword)
			{
				return IdentityResult.Failed(new IdentityError
				{
					Description = CurrentAndNewPasswordCannotMatch
				});
			}

			var result = await userManager.ChangePasswordAsync(user, sanitizedCurrentPassword, sanitizedNewPassword);

			if (result.Succeeded)
			{
				await signInManager.SignOutAsync(); // Log Out 
				await signInManager.PasswordSignInAsync(user, sanitizedNewPassword, false,
					false); // Log back in again with the new password
			}

			return result;
		}
		private bool AreUserPropertiesEqual(BaseUser user, ProfileViewModel model)
		{
			return user.Id == Guid.Parse(model.Id) &&
			       user.Position == model.Position &&
			       user.FirstName == model.FirstName &&
			       user.LastName == model.LastName &&
			       user.UserName == model.Username &&
			       user.CompanyName == model.CompanyName &&
			       user.PhoneNumber == model.PhoneNumber &&
			       user.Email == model.Email;
		}

	}
}
