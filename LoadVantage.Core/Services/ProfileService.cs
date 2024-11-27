using System.Security.Claims;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Image;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Core.Services
{
	public class ProfileService : IProfileService
	{

		private readonly IUserService userService;
		private readonly UserManager<BaseUser> userManager;
		private readonly LoadVantageDbContext context;
		private readonly SignInManager<BaseUser> signInManager;
		private readonly IProfileHelperService profileHelperService;

		public ProfileService(LoadVantageDbContext _context, IUserService _userService, UserManager<BaseUser> _userManager,
			SignInManager<BaseUser> _signInManager, IProfileHelperService _profileHelperService)
		{
			userService = _userService;
			userManager = _userManager;
			context = _context;
			signInManager = _signInManager;
			profileHelperService = _profileHelperService;
		}

		public async Task<ProfileViewModel?> GetUserInformation(Guid userId)
		{
			var user = await userService.GetUserByIdAsync(userId);

			if (user == null)
			{
				throw new Exception(UserNotFound);
			}

			var userImageUrl = await context.UsersImages
				.Where(ui => ui.Id == user.UserImageId)
				.Select(ui => ui.ImageUrl)
				.FirstOrDefaultAsync();

			var profile = new ProfileViewModel
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

			if (user == null)
			{
				throw new Exception("User not found.");
			}

			if (user.Id != Guid.Parse(model.Id) || user.Position != model.Position) // If the user tries to change position or id from the hidden fields returns the same model 
			{
				return model;
			}

			if (await profileHelperService.IsEmailTakenAsync(model.Email, user.Id))
			{
				throw new InvalidOperationException(EmailIsAlreadyTaken);
			}

			if (await profileHelperService.IsUsernameTakenAsync(model.Username, user.Id))
			{
				throw new InvalidDataException(UserNameIsAlreadyTaken);
			}

			if (AreUserPropertiesEqual(user, model))
			{
				throw new Exception(NoChangesMadeToProfile);
			}

			await UpdateUserClaimsAsync(user, model);

			// Update the user's properties ( NOT THE ID ) 

			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.UserName = model.Username;
			user.CompanyName = model.CompanyName;
			user.PhoneNumber = model.PhoneNumber;
			user.Email = model.Email;

			user.NormalizedUserName = model.Username.ToUpperInvariant(); // normalized username
			user.NormalizedEmail = model.Email.ToUpperInvariant(); // normalized email address

			var result = await userManager.UpdateAsync(user);

			if (!result.Succeeded)
			{
				throw new Exception(UserProfileUpdateFailed);
			}

			var userImage = await context.UsersImages.SingleOrDefaultAsync(u => u.Id == user.UserImageId);

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
				UserImageUrl = userImage.ImageUrl
			};
		}

		public async Task UpdateUserClaimsAsync(BaseUser user, ProfileViewModel model)
		{
			var existingClaims = await profileHelperService.GetClaimsAsync(user);
			var claimsToAddOrUpdate = profileHelperService.GetMissingClaims(existingClaims, model.FirstName, model.LastName, model.Username,
				model.Position);

			if (claimsToAddOrUpdate.Any())
			{
				await userManager.RemoveClaimsAsync(user,
					existingClaims.Where(c => claimsToAddOrUpdate.Any(newClaim => newClaim.Type == c.Type)));
				await Task.Delay(500); // Ensuring removal happens first.
				await userManager.AddClaimsAsync(user, claimsToAddOrUpdate);
			}

		}
		public async Task<IdentityResult> ChangePasswordAsync(BaseUser user, string currentPassword, string newPassword)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user), UserCannotBeNull);
			}

			if (currentPassword == newPassword)
			{
				return IdentityResult.Failed(new IdentityError
				{
					Description = CurrentAndNewPasswordCannotMatch
				});
			}

			var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

			if (result.Succeeded)
			{
				await signInManager.SignOutAsync(); // Log Out 
				await signInManager.PasswordSignInAsync(user, newPassword, false,
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
