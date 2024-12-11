using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using LoadVantage.Extensions;
using LoadVantage.Filters;
using LoadVantage.Core.Contracts;
using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.User;

using static LoadVantage.Common.GeneralConstants.AdministratorManagement;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;

namespace LoadVantage.Areas.Admin.Controllers
{
	[Authorize]
	[AdminOnly]
	[Area("Admin")]
	public class UserManagementController : Controller
	{
		private readonly IUserManagementService userManagementService;
		private readonly IAdminProfileService adminProfileService;
		private readonly IUserService userService;
        private readonly IAdminUserService adminUserService;


        public UserManagementController(
			IUserManagementService _userManagementService, 
			IAdminProfileService _adminProfileService, 
			 IUserService _userService,
             IAdminUserService _adminUserService)
		{
			userManagementService = _userManagementService;
			adminProfileService = _adminProfileService;
			userService = _userService;
            adminUserService = _adminUserService;

        }

        [HttpGet]
        [Route("UserManagement")]

		public async Task<IActionResult> UserManagement(Guid adminId, int pageNumber = 1, int pageSize = 5, string searchTerm = "")
        {
            IEnumerable<UserManagementViewModel> users;
            int totalUsers;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                users = await userManagementService.SearchUsersAsync(searchTerm);

                if (!users.Any())
                {
					TempData.SetErrorMessage(NoResultsFound);
				}
			}
            else
            {
                users = await userManagementService.GetUsersAsync(pageNumber, pageSize);
            }

            totalUsers = await adminUserService.GetUserCountAsync();

            var adminProfile = await adminProfileService.GetAdminInformation(adminId);


            var model = new UsersListModel
            {
	            Users = users.ToList(),
	            TotalUsers = totalUsers,
	            AdminProfile = adminProfile,
				PageSize = pageSize,
	            CurrentPage = pageNumber,
	            SearchTerm = searchTerm
            };

            return View("~/Areas/Admin/Views/Admin/UserManagement/UserManagement.cshtml", model);
        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateUser(UsersListModel model)
		{
			var adminId = User.GetAdminId();

			model.NewUser.Id = Guid.NewGuid().ToString();
			ModelState.Clear(); // Clear any other model state errors found 
			TryValidateModel(model.NewUser); // Validate just the new user  

			if (!ModelState.IsValid)
			{
				var validationErrors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.ToList();

				var errors = string.Join(Environment.NewLine, validationErrors);


				TempData.SetErrorMessage(errors);
				return RedirectToAction("UserManagement", new { adminId });

			}

			try
			{
				await userManagementService.CreateUserAsync(model.NewUser);
			}
			catch (InvalidOperationException e)
			{
				TempData.SetErrorMessage(e.Message);
				return RedirectToAction("UserManagement", new { adminId });
			}
			catch (ArgumentNullException ex)
			{
				TempData.SetErrorMessage(InvalidUserModelOrRoleAdded);
				return RedirectToAction("UserManagement", new { adminId });
			}

			TempData.SetSuccessMessage(string.Format(NewUserCreatedSuccessfully, model.NewUser.UserName, model.NewUser.FullName));
			return RedirectToAction("UserManagement", new { adminId });

		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateAdministrator(UsersListModel model)
		{
			var adminId = User.GetAdminId();

			model.NewUser.Id = Guid.NewGuid().ToString();
			ModelState.Clear(); // Clear any other model state errors found 
			TryValidateModel(model.NewUser); // Validate just the new user  

			if (!ModelState.IsValid)
			{
				var validationErrors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.ToList();

				var errors = string.Join(Environment.NewLine, validationErrors);


				TempData.SetErrorMessage(errors);
				return RedirectToAction("UserManagement", new { adminId });

			}

			try
			{
				await userManagementService.CreateAdministratorAsync(model.NewUser);
			}
			catch (InvalidOperationException e)
			{
				TempData.SetErrorMessage(e.Message);
				return RedirectToAction("UserManagement", new { adminId });
			}
			catch (ArgumentNullException ex)
			{
				TempData.SetErrorMessage(InvalidAdminModelOrRoleAdded);
				return RedirectToAction("UserManagement", new { adminId });
			}

			TempData.SetSuccessMessage(string.Format(NewAdministratorCreatedSuccessfully, model.NewUser.UserName, model.NewUser.FullName));
			return RedirectToAction("UserManagement", new { adminId });

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeactivateUser(Guid userId)
		{
			var adminId = User.GetAdminId();

			try
			{
				var isDeactivated = await userManagementService.DeactivateUserAsync(userId);

				if (!isDeactivated)
				{
					TempData.SetErrorMessage(FailedToDeactivateTheUser);
					return RedirectToAction("UserManagement", new { adminId });
				}

				TempData.SetSuccessMessage(UserDeactivatedSuccessfully);
				return RedirectToAction("UserManagement", new { adminId });
			}
			catch (ArgumentException ex)
			{
				TempData.SetErrorMessage(ex.Message); // "User not found"
				return RedirectToAction("UserManagement", new { adminId });
			}
			catch (InvalidOperationException ex)
			{
				TempData.SetErrorMessage(ex.Message); // Cannot deactivate Dispatcher or Broker ( active drivers or active loads )
				return RedirectToAction("UserManagement", new { adminId });
			}
			catch (Exception ex)
			{
				TempData.SetErrorMessage($"An error occurred: {ex.Message}");
				return RedirectToAction("UserManagement", new { adminId });
			}
			
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ReactivateUser(Guid userId)
		{
			var adminId = User.GetAdminId();

			try
			{
				var isRestored = await userManagementService.ReactivateUserAsync(userId);

				if (!isRestored)
				{
					TempData.SetErrorMessage(FailedToReactivateThisAccount);
					return RedirectToAction("UserManagement", new { adminId });
				}

				TempData.SetSuccessMessage(UserActivatedSuccessfully);
				return RedirectToAction("UserManagement", new { adminId });
			}
			catch (ArgumentException ex)
			{
				TempData.SetErrorMessage(ex.Message); // "User not found"
				return RedirectToAction("UserManagement", new { adminId });
			}
			catch (Exception ex)
			{
				TempData.SetErrorMessage(ErrorTryLater);
				return RedirectToAction("UserManagement", new { adminId });
			}
		}


		[HttpGet]
		public async Task<IActionResult> GetUserRoleDetails(Guid userId)
		{
			var user = await userService.GetUserByIdAsync(userId);

			if (user == null)
			{
				return NotFound(UserNotFound);
			}

			var model = new AdminEditUserViewModel
			{
				Id = user.Id.ToString(),
				UserName = user.UserName,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				PhoneNumber = user.PhoneNumber ?? "",
				CompanyName = user.CompanyName ?? ""
			};

			return Json(model); // Return the model as JSON
		}

		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> EditUser(UsersListModel model)
		{
			var adminId = User.GetAdminId();


			ModelState.Clear(); // Clear any other model state errors found 
			TryValidateModel(model.EditedUser); // Validate just the Edited User  

			if (!ModelState.IsValid)
			{
				return BadRequest("Invalid user data provided.");
			}
			try
			{
				await userManagementService.UpdateUserAsync(model.EditedUser);
				TempData.SetSuccessMessage(UserUpdatedSuccessfully);

				return RedirectToAction("UserManagement", new { adminId });
			}
			catch (InvalidOperationException ex)
			{
				TempData.SetErrorMessage(ex.Message);

				return RedirectToAction("UserManagement", new { adminId });
			}
			catch (Exception ex)
			{
				TempData.SetErrorMessage(ErrorTryLater);
				return RedirectToAction("UserManagement", new { adminId });
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> ResetUserPassword(Guid userId)
		{
			var adminId = User.GetAdminId();

			var result = await userManagementService.ResetPasswordToDefaultAsync(userId);

			if (!result.Succeeded)
			{
				TempData.SetErrorMessage(ResetPasswordFailed);
				return RedirectToAction("UserManagement", new { adminId });
			}

			TempData.SetSuccessMessage(UserPasswordResetSuccessfully);
			return RedirectToAction("UserManagement", new { adminId });
		}

	}
}
