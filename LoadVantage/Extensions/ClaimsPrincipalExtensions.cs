using System.Security.Claims;

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
    }
}
