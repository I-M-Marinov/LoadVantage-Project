using LoadVantage.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LoadVantage.Common.GeneralConstants.UserRoles;

namespace LoadVantage.Areas.Admin.Controllers
{
    [AdminOnly]
    [Area("Admin")]
    [Route("Admin")]

    public class AdminController : Controller
    {
        [HttpGet]
        [Route("Profile")]
        public IActionResult AdminDashboard()
        {
            return View();
        }
    }
}
