using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Core.Contracts
{
	public interface IProfileHelperService
	{
		Task<bool> IsUsernameTakenAsync(string username, Guid currentUserId);
		Task<bool> IsEmailTakenAsync(string email, Guid currentUserId);
		Task<IEnumerable<Claim>> GetClaimsAsync(BaseUser user);
		List<Claim> GetMissingClaims(IEnumerable<Claim> existingClaims, string firstName, string lastName, string userName, string userPosition);
		/// <summary>
		/// Finds the User by his username 
		/// </summary>
		Task<BaseUser?> FindUserByUsernameAsync(string username);
	}
}
