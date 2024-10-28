namespace LoadVantage.Filters
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using static Common.GeneralConstants.UserRoles;


    public class DispatcherOnlyAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated ||
                !user.IsInRole(UserRoleName) ||
                user.FindFirst("Position")?.Value != DispatcherPositionName)
            {
                context.Result = new ForbidResult(); 
            }
        }
    }

}
