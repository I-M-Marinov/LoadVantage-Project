using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProfileService(LoadVantageDbContext context, UserManager<User> userManager) : IProfileService
    {
        public async Task<ProfileViewModel> GetUserInformation(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new Exception(UserNotFound);
            }

            var userClaims = await userManager.GetClaimsAsync(user);

            var profile = new ProfileViewModel
            {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Position = user.Position,
                CompanyName = user.CompanyName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email
                
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

            await UpdateUserClaimsAsync(user, model);

            // Now update the user's properties
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.Username;
            user.CompanyName = model.CompanyName;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;

            user.NormalizedUserName = model.Username.ToUpperInvariant();  
            user.NormalizedEmail = model.Email.ToUpperInvariant();       

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

        public async Task<bool> IsUsernameTakenAsync(string username, Guid currentUserId)
        {
            var existingUser = await userManager.FindByNameAsync(username);
            return existingUser != null && existingUser.Id != currentUserId;
        }

    }
}
