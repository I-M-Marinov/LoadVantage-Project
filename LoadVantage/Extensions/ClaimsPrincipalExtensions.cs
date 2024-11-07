using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                return null;
            }
            
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdString, out Guid userGuidId))
            {
                return userGuidId;
            }
            
            return null;
        }

		public static async Task<User> GetUserAsync(this ClaimsPrincipal user, UserManager<User> userManager)
		{
			var userId = user.GetUserId();

			if (userId == null)
			{
				return null; 
			}

			return await userManager.FindByIdAsync(userId.Value.ToString());
		}
	}
}
