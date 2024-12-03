using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Core.Services
{
	public class ProfileHelperService : IProfileHelperService
	{
		private readonly IUserService userService;
		private readonly LoadVantageDbContext context;
		private readonly UserManager<BaseUser> userManager;



		public ProfileHelperService(
			LoadVantageDbContext _context, 
			IUserService _userService, 
			UserManager<BaseUser> _userManager)
		{
			userService = _userService;
			context = _context;
			userManager = _userManager;
		}

		public async Task<bool> IsUsernameTakenAsync(string username, Guid currentUserId)
		{
			var existingUser = await FindUserByUsernameAsync(username);

			return existingUser != null && existingUser.Id != currentUserId;
		}
		public async Task<bool> IsEmailTakenAsync(string email, Guid currentUserId)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				throw new ArgumentException(EmailCannotBeNull, nameof(email));
			}

			var isEmailTaken = await context.Users.AnyAsync(user => user.Email == email && user.Id != currentUserId);

			return isEmailTaken;
		}
		public async Task<IEnumerable<Claim>> GetClaimsAsync(BaseUser user)
		{
			return await userService.GetUserClaimsAsync(user);
		}
		public List<Claim> GetMissingClaims(IEnumerable<Claim> existingClaims, string firstName, string lastName, string userName, string userPosition)
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
		public async Task<BaseUser?> FindUserByUsernameAsync(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentException(UserNameCannotBeNull, nameof(username));
			}

			return await userManager.FindByNameAsync(username);
		}

	}
}
