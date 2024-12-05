using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Account;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data.Models;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using static LoadVantage.Common.ValidationConstants;
using static LoadVantage.Common.GeneralConstants.UserRoles;

namespace LoadVantage.Core.Services
{
	public class AccountService : IAccountService
	{
		private readonly SignInManager<BaseUser> signInManager;
		private readonly RoleManager<Role> roleManager;
		private readonly IHtmlSanitizerService htmlSanitizer;
		private readonly IUserService userService;
		private readonly IProfileHelperService profileHelperService;

		public AccountService(
			SignInManager<BaseUser> _signInManager,
			RoleManager<Role> _roleManager,
			IHtmlSanitizerService _htmlSanitizer,
			IUserService _userService,
			IProfileHelperService _profileHelperService)
		{
			signInManager = _signInManager;
			roleManager = _roleManager;
			htmlSanitizer = _htmlSanitizer;
			userService = _userService;
			profileHelperService = _profileHelperService;
		}

		public async Task<(IdentityResult Result, string? Error)> RegisterUserAsync(RegisterViewModel model)
		{
			if (!ValidPositions.Contains(model.Position))
			{
				return (IdentityResult.Failed(new IdentityError { Description = InvalidPositionSelected }), "Position");
			}

			var existingEmail = await userService.FindUserByEmailAsync(model.Email);

			if (existingEmail != null)
			{
				return (IdentityResult.Failed(new IdentityError { Description = EmailAlreadyExists }), "Email");
			}

			var existingUser = await userService.FindUserByUsernameAsync(model.UserName);

			if (existingUser != null)
			{
				return (IdentityResult.Failed(new IdentityError { Description = UserNameAlreadyExists }), "Username");
			}

			var defaultImageId = await userService.GetOrCreateDefaultImageAsync();

			var sanitizedFirstName = htmlSanitizer.Sanitize(model.FirstName);
			var sanitizedLastName = htmlSanitizer.Sanitize(model.LastName);
			var sanitizedCompany = htmlSanitizer.Sanitize(model.Company);
			var sanitizedPosition = htmlSanitizer.Sanitize(model.Position);
			var sanitizedEmail = htmlSanitizer.Sanitize(model.Email);
			var sanitizedUsername = htmlSanitizer.Sanitize(model.UserName);
			var sanitizedRole = htmlSanitizer.Sanitize(model.Role);

			Role? role = await roleManager.FindByNameAsync(sanitizedRole);

			if (role?.Name == nameof(Administrator) || string.IsNullOrWhiteSpace(role?.Name))
			{
				return (IdentityResult.Failed(new IdentityError { Description = InvalidRoleSelected }), "Role");
			}

			User user = new User
			{
				FirstName = sanitizedFirstName,
				LastName = sanitizedLastName,
				CompanyName = sanitizedCompany,
				Position = sanitizedPosition,
				Email = sanitizedEmail,
				UserName = sanitizedUsername,
				Role = role!,
				UserImageId = defaultImageId
			};

			var result = await userService.CreateUserAsync(user, model.Password);

			if (!result.Succeeded)
			{
				return (result, null);
			}

			await userService.AssignUserRoleAsync(user, UserRoleName);
			await userService.AddUserClaimAsync(user, new Claim("Position", user.Position ?? ""));
			await userService.AddUserClaimAsync(user, new Claim("FirstName", user.FirstName ?? ""));
			await userService.AddUserClaimAsync(user, new Claim("LastName", user.LastName ?? ""));
			await userService.AddUserClaimAsync(user, new Claim("UserName", user.UserName ?? ""));

			return (IdentityResult.Success, null);
		}

		public async Task<(SignInResult result, string message)> LoginAsync(string username, string password)
		{
			var sanitizedUserName = htmlSanitizer.Sanitize(username);
			var sanitizedPassword = htmlSanitizer.Sanitize(password);

			var user = await userService.FindUserByUsernameAsync(sanitizedUserName);

			if (user == null)
			{
				return (SignInResult.Failed, InvalidUserNameOrPassword);
			}

			if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
			{
				var lockoutLiftDateTime = user.LockoutEnd.Value.ToLocalTime().ToString("hh:mm tt");
				return (SignInResult.LockedOut, string.Format(TooManyFailedLoginAttempts, lockoutLiftDateTime));
			}

			if (!user.IsActive)
			{
				return (SignInResult.NotAllowed, ThisAccountIsInactive);
			}

			var result = await signInManager.PasswordSignInAsync(user, sanitizedPassword, false, true);

			if (result.Succeeded)
			{
				await AddMissingClaimsAsync(user);
				await signInManager.SignInAsync(user, isPersistent: false);

				return (SignInResult.Success, null);
			}

			return (SignInResult.Failed, InvalidUserNameOrPassword);

		}

		public async Task LogOutAsync()
		{
			await signInManager.SignOutAsync();
		}

		private async Task AddMissingClaimsAsync(BaseUser user)
		{
			var userClaims = await profileHelperService.GetClaimsAsync(user);

			var claims = profileHelperService.GetMissingClaims(userClaims, user.FirstName, user.LastName,
				user.UserName, user.Position);
			if (claims.Count != 0)
			{
				await userService.AddUserClaimsAsync(user, claims);
			}
		}

	}
}

