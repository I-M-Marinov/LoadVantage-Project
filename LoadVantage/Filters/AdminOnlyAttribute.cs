namespace LoadVantage.Filters
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using static Common.GeneralConstants.UserRoles;


    public class AdminOnlyAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated ||
                !user.IsInRole(AdminRoleName) ||
                user.FindFirst("Position")?.Value != AdminPositionName)
            {
                context.Result = new ForbidResult();
            }
        }
    }

}