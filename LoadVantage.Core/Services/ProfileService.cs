using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Core.Services
{
    public class ProfileService(
	    LoadVantageDbContext context, 
	    UserManager<User> userManager,
		 SignInManager<User> signInManager) : IProfileService
    {


        public async Task<ProfileViewModel?> GetUserInformation(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new Exception(UserNotFound);
            }

            var profile = new ProfileViewModel
            {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName!,
                Position = user.Position!,
                CompanyName = user.CompanyName!,
                PhoneNumber = user.PhoneNumber!,
                Email = user.Email!
                
            };

            return profile;
        }

        public async Task<ProfileViewModel> UpdateProfileInformation(ProfileViewModel model, Guid userId)
        {
            var user = await context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new Exception(UserNotFound);
            }

            if (user.Id != Guid.Parse(model.Id))
            {
                throw new UnauthorizedAccessException(IdCannotBeChanged);
            }

            if (user.Position != model.Position)
            {
                throw new UnauthorizedAccessException(PositionCannotBeChanged);
            }

            if (userManager.Users.Any(u => u.Email == model.Email) && model.Email != user.Email)
            {
                throw new Exception(EmailIsAlreadyTaken);
            }

            if (AreUserPropertiesEqual(user,model))
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

            user.NormalizedUserName = model.Username.ToUpperInvariant();  // normalized username
            user.NormalizedEmail = model.Email.ToUpperInvariant();       // normalized email address

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception(UserProfileUpdateFailed);
            }

            return new ProfileViewModel
            {
                Id = user.Id.ToString(),
                Username = user.UserName,
                CompanyName = user.CompanyName,
                Position = user.Position,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task UpdateUserClaimsAsync(User user, ProfileViewModel model)
        {
            var existingClaims = await GetUserClaimsAsync(user);
            var claimsToAddOrUpdate = GetMissingClaims(existingClaims, model.FirstName, model.LastName, model.Username, model.Position);

            if (claimsToAddOrUpdate.Any())
            {
                await userManager.RemoveClaimsAsync(user, existingClaims.Where(c => claimsToAddOrUpdate.Any(newClaim => newClaim.Type == c.Type)));
                await Task.Delay(500); // Ensuring removal happens first.
                await userManager.AddClaimsAsync(user, claimsToAddOrUpdate);
            }

        }

        public async Task<bool> IsUsernameTakenAsync(string username, Guid currentUserId)
        {
            var existingUser = await userManager.FindByNameAsync(username);
            return existingUser != null && existingUser.Id != currentUserId;
        }

        private bool AreUserPropertiesEqual(User user, ProfileViewModel model)
        {
            
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Model cannot be null.");
            }

            return user.Id == Guid.Parse(model.Id) &&
                   user.Position == model.Position &&
                   user.FirstName == model.FirstName &&
                   user.LastName == model.LastName &&
                   user.UserName == model.Username &&
                   user.CompanyName == model.CompanyName &&
                   user.PhoneNumber == model.PhoneNumber &&
                   user.Email == model.Email;
        }
        private async Task<IEnumerable<Claim>> GetUserClaimsAsync(User user)
        {
	        return await userManager.GetClaimsAsync(user);
        }

        private List<Claim> GetMissingClaims(IEnumerable<Claim> existingClaims, string firstName, string lastName, string userName, string userPosition)
        {
	        var claims = new List<Claim>
	        {
		        new Claim("FirstName", firstName),
		        new Claim("LastName", lastName),
		        new Claim("UserName", userName),
		        new Claim("Position", userPosition),
	        };

	        var missingClaims = claims.Where(claim =>
		        !existingClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value)
	        ).ToList();

	        return missingClaims;
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
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
                await signInManager.PasswordSignInAsync(user, newPassword, false, false); // Log in again with the new password
			}

	        return result;
        }
	}
}
