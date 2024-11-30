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
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Formatting;

namespace LoadVantage.Areas.Admin.Services
{
	public class UserManagementService : IUserManagementService
	{
		private readonly LoadVantageDbContext context;
		private readonly UserManager<BaseUser> userManager;
		private readonly RoleManager<Role> roleManager;
		private readonly IUserService userService;

		public UserManagementService(LoadVantageDbContext _context, UserManager<BaseUser> _userManager, RoleManager<Role> _roleManager, IUserService _userService)
		{
			context = _context;
			userManager = _userManager;
			roleManager = _roleManager;
			userService = _userService;
		}

        public async Task<List<UserManagementViewModel>> GetUsersAsync(int pageNumber , int pageSize)
        {
            var query = context.Users
                .Include(u => u.UserImage)
                .Include(u => u.Role)
				.OrderByDescending(u => u.Position)
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
                PhoneNumber = u.PhoneNumber ?? "N/A",
                Position = u.Position!,
                CompanyName = u.CompanyName!,
                UserName = u.UserName,
                Role = u.Role.ToString(),
				UserImageUrl = u.UserImage?.ImageUrl ?? "N/A",
				Password = null,
				IsActive = u.IsActive
            }).ToList();
        }

        public async Task<List<UserManagementViewModel>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
				var allUsers = await context.Users
                    .Include(u => u.UserImage)
                    .Include(u => u.Role)
                    .OrderBy(u => u.Position)
                    .Select(u => new UserManagementViewModel
                    {
                        Id = u.Id.ToString(),
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber ?? "N/A",
                        Position = u.Position!,
                        CompanyName = u.CompanyName!,
                        UserName = u.UserName,
                        Role = u.Role.ToString(),
                        UserImageUrl = u.UserImage!.ImageUrl,
                        IsActive = u.IsActive
					}).ToListAsync();

                return allUsers;
            }

            var normalizedSearchTerm = searchTerm.ToUpper(); 

            var users = await context.Users
                .Include(u => u.UserImage)
                .Include(u => u.Role)
				.Where(u =>
                    (u.FirstName + " " + u.LastName).ToUpper().Contains(normalizedSearchTerm) || 
                    u.Email.ToUpper().Contains(normalizedSearchTerm) ||                        
                    u.PhoneNumber.Contains(normalizedSearchTerm) ||                          
                    u.UserName.ToUpper().Contains(normalizedSearchTerm) ||
                    u.Position.ToUpper().Contains(normalizedSearchTerm) ||
                    u.CompanyName.ToUpper().Contains(normalizedSearchTerm))
	            .OrderBy(u => u.Position)
				.Select(u => new UserManagementViewModel
                {
                    Id = u.Id.ToString(),
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber ?? "N/A",
					Position = u.Position!,
                    CompanyName = u.CompanyName!,
                    UserName = u.UserName,
                    Role = u.Role.ToString(),
                    UserImageUrl = u.UserImage!.ImageUrl,
                    IsActive = u.IsActive
				}).ToListAsync();

            return users;
        }


        public async Task<int> GetTotalUsersCountAsync()
        {
            return await context.Users.CountAsync();
        }

        public async Task<AdminCreateUserViewModel> CreateAdministratorAsync(AdminCreateUserViewModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("The model cannot be null");
			}

			var userRole = await roleManager.FindByNameAsync(AdminRoleName);

			if (userRole == null)
			{
				throw new ArgumentNullException("Invalid role selected!");
			}

			var newUser = new Administrator()
			{
				Id = Guid.NewGuid(),
				UserName = model.UserName,
				Email = model.Email,
				PhoneNumber = model.PhoneNumber,
				Position = AdminPositionName,
				FirstName = model.FirstName,
				LastName = model.LastName,
				CompanyName = model.CompanyName,
				UserImageId = DefaultImageId,
				Role = userRole,
				RoleId = userRole.Id
			};

			var result = await userManager.CreateAsync(newUser, model.Password);


			var roleResult = await userManager.AddToRoleAsync(newUser, AdminRoleName);
			if (!roleResult.Succeeded)
			{
				var roleErrors = string.Join(Environment.NewLine, roleResult.Errors.Select(e => e.Description));
				throw new InvalidOperationException($"Role assignment failed: {roleErrors}");
			}

			var claims = new List<Claim>
			{
				new Claim("FirstName", newUser.FirstName),
				new Claim("LastName", newUser.LastName),
				new Claim("UserName", newUser.UserName),
				new Claim("Position", newUser.Position),
			};

				await userService.AddUserClaimsAsync(newUser, claims);

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

			var claims = new List<Claim>
			{
				new Claim("FirstName", newUser.FirstName),
				new Claim("LastName", newUser.LastName),
				new Claim("UserName", newUser.UserName),
				new Claim("Position", newUser.Position),
			};

			await userService.AddUserClaimsAsync(newUser, claims);

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

		public async Task<bool> DeactivateUserAsync(Guid userId)
        {
	        var users = await context.Users
		        .Include(u => u.UserImage).ToListAsync();

	        BaseUser user = users.First(u => u.Id == userId);

			if (user == null)
			{
				throw new ArgumentException(UserNotFound);
			}

			user.FirstName = "N/A";
			user.LastName = "N/A";
			user.Email = "N/A";
			user.CompanyName = "N/A";
			user.NormalizedEmail = "N/A";
			user.PhoneNumber = "N/A";
			user.IsActive = false; // deactivate the account 

			var userImageId = user.UserImage.Id;

			if (userImageId != DefaultImageId)
			{
				await userService.DeleteUserImageAsync(user.Id, userImageId); // removes the picture for that user and defaults account to the default picture 
			}
			
			context.Users.Update(user);
			await context.SaveChangesAsync();

			return true;
		}

        public async Task<bool> ReactivateUserAsync(Guid userId)
        {
	        var users = await context.Users
		        .Include(u => u.UserImage).ToListAsync();

	        BaseUser user = users.First(u => u.Id == userId);

	        if (user == null)
	        {
		        throw new ArgumentException(UserNotFound);
	        }

	        user.IsActive = true; // activate the account 
	        user.UserImageId = DefaultImageId; // set the user's image to the default one 
			context.Users.Update(user);
	        await context.SaveChangesAsync();

	        return true;
        }

        public async Task<bool> UpdateUserAsync(AdminEditUserViewModel model)
        {
	        var user = await userService.GetUserByIdAsync(Guid.Parse(model.Id));

	        if (user == null)
	        {
		        throw new InvalidOperationException(UserNotFound);
	        }

	        user.UserName = model.UserName;
	        user.Email = model.Email;
	        user.PhoneNumber = model.PhoneNumber;
	        user.FirstName = model.FirstName;
	        user.LastName = model.LastName;
	        user.CompanyName = model.CompanyName;


	        var updateResult = await userService.UpdateUserAsync(user);

	        if (!updateResult.Succeeded)
	        {
				throw new InvalidOperationException(FailedToUpdateTheUser);
			}

			return true;
		}
        
		public async Task<List<string>> GetUserRolesAsync()
		{
			var roles = await context.Roles.Select(r => r.Name).ToListAsync();
			return roles;
		}



	}
}

