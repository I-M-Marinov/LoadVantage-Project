using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.User;
using LoadVantage.Extensions;
using LoadVantage.Filters;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.GeneralConstants.AdministratorManagement;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using LoadVantage.Core.Models.Truck;
using static LoadVantage.Common.GeneralConstants;


namespace LoadVantage.Areas.Admin.Controllers
{
	[Authorize]
	[AdminOnly]
	[Area("Admin")]
	public class UserManagementController : Controller
	{
		private readonly IUserManagementService userManagementService;
		private readonly IAdminProfileService adminProfileService;
		private readonly UserManager<BaseUser> userManager;
		private readonly RoleManager<Role> roleManager;

		public UserManagementController(IUserManagementService _userManagementService, IAdminProfileService _adminProfileService, UserManager<BaseUser> _userManager, RoleManager<Role> _roleManager)
		{
			userManagementService = _userManagementService;
			adminProfileService = _adminProfileService;
			userManager = _userManager;
			roleManager = _roleManager;

		}

		[HttpGet]
		public async Task<IActionResult> UserManagement(Guid adminId, int pageNumber = 1, int pageSize = 5, string searchTerm = "")
        {
            IEnumerable<UserManagementViewModel> users;
            int totalUsers;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                users = await userManagementService.SearchUsersAsync(searchTerm);
			}
            else
            {
                users = await userManagementService.GetUsersAsync(pageNumber, pageSize);
            }

            totalUsers = await userManagementService.GetTotalUsersCountAsync();

			var adminProfile = await adminProfileService.GetAdminInformation(adminId);

            var model = new UsersListModel
            {
                Users = users.ToList(),
                TotalUsers = totalUsers,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                SearchTerm = searchTerm,
                AdminProfile = adminProfile
            };

            return View("~/Areas/Admin/Views/Admin/UserManagement/UserManagement.cshtml", model);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateUser(UsersListModel model)
		{
			var adminId = User.GetAdminId();

			ModelState.Clear(); // Clear any other model state errors found 
			TryValidateModel(model.NewUser); // Validate just the truck  

			if (!ModelState.IsValid)
			{
				var validationErrors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.ToList();

				var errors = string.Join(Environment.NewLine, validationErrors);


				TempData.SetErrorMessage(errors);
				return RedirectToAction("UserManagement", new { adminId = adminId });

			}

			try
			{
				await userManagementService.CreateUserAsync(model.NewUser);
			}
			catch (InvalidOperationException e)
			{
				TempData.SetErrorMessage(e.Message);
				return RedirectToAction("UserManagement", new { adminId = adminId });
			}
			catch (ArgumentNullException ex)
			{
				TempData.SetErrorMessage(InvalidModelOrRoleAdded);
				return RedirectToAction("UserManagement", new { adminId = adminId });
			}

			TempData.SetSuccessMessage(string.Format(NewUserCreatedSuccessfully, model.NewUser.UserName, model.NewUser.FullName));
			return RedirectToAction("UserManagement", new { adminId = adminId });

		}

		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> DeleteUser(Guid userId)
		//{

		//}


	}
}
