using LoadVantage.Core.Models.Account;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Core.Contracts
{
	public interface IAccountService
	{
		/// <summary>
		/// Registers a User
		/// </summary>
		Task<(IdentityResult Result, string? Error)> RegisterUserAsync(RegisterViewModel model);
		/// <summary>
		/// Logs in a User
		/// </summary>
		Task<(SignInResult result, string message)> LoginAsync(string username, string password);
		/// <summary>
		/// Logs Out a User
		/// </summary>
		Task LogOutAsync();

	}
}
