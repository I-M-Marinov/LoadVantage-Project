using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Models.User;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.GeneralConstants.SecretString;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using static LoadVantage.Common.GeneralConstants.UserImage;
using CloudinaryDotNet.Actions;
using Role = LoadVantage.Infrastructure.Data.Models.Role;
using System.Data;

namespace LoadVantage.Areas.Admin.Services
{
	public class UserManagementService : IUserManagementService
	{
		private readonly LoadVantageDbContext context;
		private readonly UserManager<BaseUser> userManager;
		private readonly RoleManager<Role> roleManager;

		public UserManagementService(LoadVantageDbContext _context, UserManager<BaseUser> _userManager, RoleManager<Role> _roleManager)
		{
			context = _context;
			userManager = _userManager;
			roleManager = _roleManager;
		}

        public async Task<List<UserManagementViewModel>> GetUsersAsync(int pageNumber , int pageSize)
        {
            var query = context.Users
                .Include(u => u.UserImage)
                .Include(u => u.Role)
                .Where(u => u is Dispatcher || u is Broker)
                .OrderBy(u => u.Role)
                .AsQueryable();

            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
           

            var users = await query.ToListAsync();

            return users.Select(u => new UserManagementViewModel()
            {
                Id = u.Id.ToString(),
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Position = u.Position!,
                CompanyName = u.CompanyName!,
                UserName = u.UserName,
                Role = u.Role.ToString(),
                UserImageUrl = u.UserImage!.ImageUrl,
				Password = null
            }).ToList();
        }

        public async Task<List<UserManagementViewModel>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
				var allUsers = await context.Users
                    .Include(u => u.UserImage)
                    .Include(u => u.Role)
                    .Where(u => u is Dispatcher || u is Broker)
                    .OrderBy(u => u.Role)
                    .Select(u => new UserManagementViewModel
                    {
                        Id = u.Id.ToString(),
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Position = u.Position!,
                        CompanyName = u.CompanyName!,
                        UserName = u.UserName,
                        Role = u.Role.ToString(),
                        UserImageUrl = u.UserImage!.ImageUrl
                    })
                    .ToListAsync();

                return allUsers;
            }

            var normalizedSearchTerm = searchTerm.ToUpper(); 

            var users = await context.Users
                .Include(u => u.UserImage)
                .Include(u => u.Role)
                .Where(u => u is Dispatcher || u is Broker)
                .Where(u =>
                    (u.FirstName + " " + u.LastName).ToUpper().Contains(normalizedSearchTerm) || 
                    u.Email.ToUpper().Contains(normalizedSearchTerm) ||                        
                    u.PhoneNumber.Contains(normalizedSearchTerm) ||                          
                    u.UserName.ToUpper().Contains(normalizedSearchTerm) ||
                    u.Position.ToUpper().Contains(normalizedSearchTerm) ||
                    u.CompanyName.ToUpper().Contains(normalizedSearchTerm))                    
                .Select(u => new UserManagementViewModel
                {
                    Id = u.Id.ToString(),
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Position = u.Position!,
                    CompanyName = u.CompanyName!,
                    UserName = u.UserName,
                    Role = u.Role.ToString(),
                    UserImageUrl = u.UserImage!.ImageUrl
                })
                .OrderByDescending(u => u.Position)
				.ToListAsync();

            return users;
        }


        public async Task<int> GetTotalUsersCountAsync()
        {
            return await context.Users.CountAsync();
        }

        public async Task<AdminCreateUserViewModel> CreateUserAsync(AdminCreateUserViewModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("The model cannot be null");
			}

			var userRole = await roleManager.FindByNameAsync(UserRoleName);

			if (userRole == null)
			{
				throw new ArgumentNullException("Invalid role selected!");
			}

			var newUser = new User
			{
				Id = Guid.NewGuid(),
				UserName = model.UserName,
				Email = model.Email,
				PhoneNumber = model.PhoneNumber,
				Position = model.Position,
				FirstName = model.FirstName,
				LastName = model.LastName,
				CompanyName = model.CompanyName,
				UserImageId = DefaultImageId,
				Role = userRole,
				RoleId = userRole.Id
			};

			var result = await userManager.CreateAsync(newUser, model.Password);


			var roleResult = await userManager.AddToRoleAsync(newUser, UserRoleName);
			if (!roleResult.Succeeded)
			{
				var roleErrors = string.Join(Environment.NewLine, roleResult.Errors.Select(e => e.Description));
				throw new InvalidOperationException($"Role assignment failed: {roleErrors}");
			}

			if (result.Succeeded)
			{
				var userViewModelResult = new AdminCreateUserViewModel
				{
					Id = newUser.Id.ToString(),
					FirstName = newUser.FirstName,
					LastName = newUser.LastName,
					Email = newUser.Email,
					PhoneNumber = newUser.PhoneNumber,
					Position = newUser.Position,
					CompanyName = newUser.CompanyName,
					UserName = newUser.UserName,
					Role = model.Role,
					Password = model.Password
				};

				return userViewModelResult;
			}

			throw new InvalidOperationException(UserCreationFailed);
		}

        public async Task<AdminCreateUserViewModel> UpdateUserAsync(Guid userId, AdminCreateUserViewModel updatedUserViewModel)
		{
			var user = await context.Users.FindAsync(userId);

			if (user == null)
				throw new ArgumentException(UserNotFound);
			
			user.FirstName = updatedUserViewModel.FirstName;
			user.LastName = updatedUserViewModel.LastName;
			user.Email = updatedUserViewModel.Email;
			user.PhoneNumber = updatedUserViewModel.PhoneNumber;
			user.Position = updatedUserViewModel.Position;
			user.CompanyName = updatedUserViewModel.CompanyName;

			context.Users.Update(user);
			await context.SaveChangesAsync();

			return updatedUserViewModel;
		}

        public async Task<bool> DeleteUserAsync(Guid userId)
		{
			var user = await context.Users.FindAsync(userId);

			if (user == null)
			{
				throw new ArgumentException(UserNotFound);
			}

			user.FirstName = "";
			user.LastName = "";
			user.Email = "";
			user.CompanyName = "";

			context.Users.Update(user);
			await context.SaveChangesAsync();

			return true;
		}



        public async Task<List<string>> GetUserRolesAsync()
		{
			var roles = await context.Roles.Select(r => r.Name).ToListAsync();
			return roles;
		}

		public async Task<bool> AssignRoleToUserAsync(Guid userId, string roleName)
		{
			var user = await context.Users.FindAsync(userId);

			if (user == null)
				throw new ArgumentException(UserNotFound);

			var result = await userManager.AddToRoleAsync(user, roleName);

			return result.Succeeded;
		}

		public async Task<bool> ChangeUserRoleAsync(Guid userId, string roleName)
		{
			var user = await context.Users.FindAsync(userId);

			if (user == null)
				throw new ArgumentException(UserNotFound);

			var currentRoles = await userManager.GetRolesAsync(user);
			var result = await userManager.RemoveFromRolesAsync(user, currentRoles);

			if (result.Succeeded)
			{
				var addRoleResult = await userManager.AddToRoleAsync(user, roleName);
				return addRoleResult.Succeeded;
			}

			return false;
		}

		public async Task<bool> LockUserAccountAsync(Guid userId)
		{
			var user = await userManager.FindByIdAsync(userId.ToString());

			if (user == null)
				throw new ArgumentException("User not found.");

			var result = await userManager.SetLockoutEnabledAsync(user, true);

			if (result.Succeeded)
			{
				await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddYears(256));  // the RAM memory of my first computer :) 
				return true;
			}

			return false;
		}

		public async Task<bool> UnlockUserAccountAsync(Guid userId)
		{
			var user = await userManager.FindByIdAsync(userId.ToString());

			if (user == null)
				throw new ArgumentException(UserNotFound);

			var result = await userManager.SetLockoutEnabledAsync(user, false);

			return result.Succeeded;
		}


	}
}

