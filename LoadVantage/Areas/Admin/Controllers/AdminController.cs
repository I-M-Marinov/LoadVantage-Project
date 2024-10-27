using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LoadVantage.Common.GeneralConstants.UserRoles;

namespace LoadVantage.Areas.Admin.Controllers
{
    [Authorize(Roles = AdminRoleName)]
    [Area("Admin")]
    public class AdminController : Controller
    {
        public IActionResult AdminDashboard()
        {
            return View();
        }
    }
}
