using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using LoadVantage.Infrastructure.Data.Models;

using static LoadVantage.Common.GeneralConstants.UserRoles;

namespace LoadVantage.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal? user)
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

		public static async Task<User?> GetUserAsync(this ClaimsPrincipal user, UserManager<User> userManager)
		{
			var userId = user.GetUserId();

			if (userId == null)
			{
				return null; 
			}

			return await userManager.FindByIdAsync(userId.Value.ToString());
		}

		public static Guid? GetAdminId(this ClaimsPrincipal? user)
		{
			if (user == null)
			{
				return null;
			}

			var isAdmin = user.IsInRole(AdminRoleName) || user.HasClaim("Position", AdminRoleName);

			if (!isAdmin)
			{
				return null;
			}

			return user.GetUserId();
		}

		public static async Task<Administrator?> GetAdminAsync(this ClaimsPrincipal user, UserManager<BaseUser> userManager)
		{
			var adminId = user.GetAdminId();
			if (adminId == null)
			{
				return null;
			}

			var admin = await userManager.FindByIdAsync(adminId.Value.ToString());
			return admin as Administrator; 
		}
	}
}
