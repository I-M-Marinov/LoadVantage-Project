﻿using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using static LoadVantage.Common.GeneralConstants.SecretString;


namespace LoadVantage.Areas.Admin.Services
{
	public class AdminUserService : IAdminUserService
	{

		private readonly UserManager<BaseUser> userManager;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly LoadVantageDbContext context;
		private readonly IImageService imageService;

		public AdminUserService(
			UserManager<BaseUser> _userManager, 
			IHttpContextAccessor _httpContextAccessor, 
			LoadVantageDbContext _context, 
			IImageService _imageService)
		{
			userManager = _userManager;
			httpContextAccessor = _httpContextAccessor;
			context = _context;
			imageService = _imageService;
		}

		public async Task<BaseUser> GetCurrentAdminAsync()
		{
			
			var adminId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(adminId))
			{
				return null;
			}

			return await userManager.FindByIdAsync(adminId);
			
		}
		public async Task<BaseUser> GetAdminByIdAsync(Guid adminId)
		{
			return await userManager.FindByIdAsync(adminId.ToString());
		}
		public async Task<Administrator> GetCurrentAdministratorAsync()
		{
			var adminId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(adminId))
			{
				return null;
			}

			return await userManager.FindByIdAsync(adminId) as Administrator;
		}
		public async Task<IEnumerable<BaseUser>> GetAllUsersAsync()
		{
			return await userManager.Users
				.ToListAsync();
		}
		public async Task<IEnumerable<BaseUser>> GetDispatchersAsync()
		{
			return await userManager.Users
				.Where(u => u is Dispatcher)
				.ToListAsync();
		}
		public async Task<IEnumerable<BaseUser>> GetBrokersAsync()
		{
			return await userManager.Users
				.Where(u => u is Broker)
				.ToListAsync();
		}
		public async Task<IEnumerable<Claim>> GetAdminClaimsAsync(BaseUser administrator)
		{
			var claims = await userManager.GetClaimsAsync(administrator);
			return claims;
		}
		public async Task UpdateUserImageAsync(Guid userId, IFormFile file)
		{
			var user = await GetAdminByIdAsync(userId);

			var userImage = await context.UsersImages
				.SingleOrDefaultAsync(ui => ui.Id == user.UserImageId);

			// Delete the old image

			if (userImage != null && userImage.Id != DefaultImageId)
			{
				await DeleteUserImageAsync(userId, userImage.Id);
			}

			// Upload the new image

			var uploadResult = await imageService.UploadImageAsync(file);

			if (uploadResult.StatusCode == HttpStatusCode.OK)
			{
				var resultImageUrl = uploadResult.SecureUrl.ToString();
				var publicId = uploadResult.PublicId;

				userImage = new UserImage
				{
					Id = Guid.NewGuid(),
					ImageUrl = resultImageUrl,
					PublicId = publicId
				};

				await context.UsersImages.AddAsync(userImage);
				await context.SaveChangesAsync();


				// Add reference to the user in the User's table
				user!.UserImageId = userImage.Id;
				await userManager.UpdateAsync(user);

				// Save changes to the database
				await context.SaveChangesAsync();
			}
			else
			{
				throw new Exception(ImageUploadFailed);
			}
		}
		public async Task DeleteUserImageAsync(Guid userId, Guid imageId)
		{
			var user = await GetAdminByIdAsync(userId);

			var userImage = await context.UsersImages
				.SingleOrDefaultAsync(ui => ui.Id == user.UserImageId);

			if (userImage != null)
			{
				var deleteResult = await imageService.DeleteImageAsync(userImage.PublicId);

				if (!deleteResult.IsSuccess)
				{
					throw new Exception($"{ErrorRemovingImage} {deleteResult.Message}");
				}

				context.UsersImages.Remove(userImage);
				user.UserImageId = DefaultImageId;
				await context.SaveChangesAsync();
			}

		}
        public async Task<int> GetUserCountAsync()
        {
            return await context.Users.CountAsync();
        }
        public async Task<int> GetDispatcherCountAsync()
        {
            return await context.Users.CountAsync(user => user.Position == nameof(Dispatcher));
        }
        public async Task<int> GetBrokerCountAsync()
        {
            return await context.Users.CountAsync(user => user.Position == nameof(Broker));
        }
        public async Task<IEnumerable<BaseUser>> GetAllUsersFromACompany()
        {
            var usersWithCompanyName = await context.Users
                .Where(user => !string.IsNullOrEmpty(user.CompanyName))
                .ToListAsync();

            return usersWithCompanyName;
        }
        public async Task<IdentityResult> DeleteUserPassword(BaseUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), UserCannotBeNull);
            }

            return await userManager.RemovePasswordAsync(user);
        }
        public async Task<IdentityResult> AddUserDefaultPassword(BaseUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), UserCannotBeNull);
            }

            return await userManager.AddPasswordAsync(user, PasswordDefaultAfterReset);
        }

	}
}
