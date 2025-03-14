﻿using System.Net;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Chat;
using LoadVantage.Core.Models.Image;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;

using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;

#nullable disable

namespace LoadVantage.Core.Services
{
    [Authorize]
    public class UserService : IUserService
    {
        private readonly UserManager<BaseUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
		private readonly LoadVantageDbContext context;
        private readonly IImageService imageService;

        public UserService(
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



		public async Task<BaseUser> GetUserByIdAsync(Guid userId)
        {
            return await userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<IdentityResult> UpdateUserAsync(BaseUser user)
        {
	        return await userManager.UpdateAsync(user);
        }

		public async Task<BaseUser> GetCurrentUserAsync()
        {
	        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

	        if (string.IsNullOrEmpty(userId))
	        {
		        return null;
			}

			return await userManager.FindByIdAsync(userId);
        }

		public async Task<BaseUser> FindUserByEmailAsync(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				throw new ArgumentException(EmailCannotBeNull, nameof(email));
			}

			return await userManager.FindByEmailAsync(email);
		}

		public async Task<BaseUser> FindUserByUsernameAsync(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentException(UserNameCannotBeNull, nameof(username));
			}

			return await userManager.FindByNameAsync(username);
		}

		public async Task<IdentityResult> CreateUserAsync(BaseUser user, string password)
		{
			if (user == null)
			{
				throw new ArgumentNullException(UserCannotBeNull);
			}

			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentException(PasswordCannotBeNull, nameof(password));
			}

			return await userManager.CreateAsync(user, password);
		}

		public async Task<IdentityResult> AssignUserRoleAsync(BaseUser user, string role)
		{
			if (user == null)
			{
				throw new ArgumentNullException(UserCannotBeNull);
			}

			if (string.IsNullOrWhiteSpace(role))
			{
				throw new ArgumentException(RoleCannotBeNull, nameof(role));
			}


			var isAlreadyInRole = await userManager.IsInRoleAsync(user, role);

			if (!isAlreadyInRole)
			{
				return await userManager.AddToRoleAsync(user, role);
			}

			throw new ArgumentException(RoleAlreadyAssignedToUser, nameof(role));

		}

		public async Task<ProfileViewModel> GetUserInformation(Guid userId)
        {
	        var user = await GetUserByIdAsync(userId);

	        if (user == null)
	        {
		        throw new Exception(UserNotFound);
	        }

	        var userImage = await GetUserImageUrlAsync(userId);

			var viewModel = new ProfileViewModel
	        {
		        Id = user.Id.ToString(),
		        Username = user.UserName!,
		        Email = user.Email!,
		        FirstName = user.FirstName,
		        LastName = user.LastName,
		        CompanyName = user.CompanyName!,
		        Position = user.Position!,
		        PhoneNumber = user.PhoneNumber!,
		        UserImageUrl = userImage,
		        ChangePasswordViewModel = new ChangePasswordViewModel(),
		        ImageFileUploadModel = new ImageFileUploadModel()
	        };

	        return viewModel;

        }

		public async Task<UserChatViewModel> GetChatUserInfoAsync(Guid userId)
        {
	        var user = await context.Users
				.Include(u => u.UserImage)
		        .Where(u => u.Id == userId)
		        .Select(u => new UserChatViewModel
		        {
			        Id = u.Id,
			        FullName = u.FullName,
			        ProfilePictureUrl = u.UserImage.ImageUrl,
			        PhoneNumber = u.PhoneNumber,
			        Company = u.CompanyName,
					Position = u.Position
		        })
		        .AsNoTracking()
		        .FirstOrDefaultAsync();

	        return user;
        }

		public async Task UpdateUserImageAsync(Guid userId, IFormFile file)
        {
	        var user = await GetUserByIdAsync(userId);

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
	        var user = await GetUserByIdAsync(userId);

			var userImage = await context.UsersImages
                .SingleOrDefaultAsync(ui => ui.Id == user.UserImageId);

			if (userImage.Id == DefaultImageId)
			{
				return; // no need to do anything if the user is already with the default user image 
			}

            var deleteResult = await imageService.DeleteImageAsync(userImage.PublicId);
            
			if (!deleteResult.IsSuccess)
            {
                throw new Exception($"{ErrorRemovingImage} {deleteResult.Message}");
            }

            context.UsersImages.Remove(userImage);
			user.UserImageId = DefaultImageId; // assign the default user image 
			await context.SaveChangesAsync();
            
        }

        public async Task<string> GetUserImageUrlAsync(Guid userId)
        {
	        var user = await GetUserByIdAsync(userId);

			var userImage = await context.UsersImages
		        .Where(ui => ui.Id == user.UserImageId)
		        .Select(ui => ui.ImageUrl)
		        .FirstOrDefaultAsync();

	        return userImage ?? DefaultImagePath;
        }

        public async Task<Guid> GetOrCreateDefaultImageAsync()
        {
	        var defaultImage = await context.UsersImages
		        .FirstOrDefaultAsync(img => img.Id == DefaultImageId);

	        if (defaultImage == null)
	        {
		        defaultImage = new UserImage
		        {
			        Id = DefaultImageId,
			        ImageUrl = DefaultImagePath,
			        PublicId = DefaultPublicId
		        };

		        context.UsersImages.Add(defaultImage);
		        await context.SaveChangesAsync();
	        }

	        return defaultImage.Id;
        }

		public async Task<IEnumerable<Claim>> GetUserClaimsAsync(BaseUser user)
        {
	        var claims = await userManager.GetClaimsAsync(user);
	        return claims;
        }

        public async Task<IdentityResult> AddUserClaimAsync(BaseUser user, Claim claim)
        {
	        if (user == null)
	        {
		        throw new ArgumentNullException(nameof(user));
			}

	        if (claim == null)
	        {
		        throw new ArgumentNullException(nameof(claim));
			}

	        return await userManager.AddClaimAsync(user, claim);
        }

        public async Task<IdentityResult> AddUserClaimsAsync(BaseUser user, IEnumerable<Claim> claims)
        {
	        if (user == null)
	        {
		        throw new ArgumentNullException(nameof(user));
			}

			if (claims == null || !claims.Any())
			{
				throw new ArgumentException(ClaimsCannotBeNull, nameof(claims));
			}

			return await userManager.AddClaimsAsync(user, claims);
        }

	}

}
