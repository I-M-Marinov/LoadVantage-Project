using LoadVantage.Core.Models.Account;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Core.Contracts
{
	public interface IAccountService
	{
		Task<(IdentityResult Result, string? Error)> RegisterUserAsync(RegisterViewModel model);
		Task<(SignInResult result, string message)> LoginAsync(string username, string password);
		Task LogOutAsync();

	}
}
