using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using LoadVantage.Core.Models.Image;
using LoadVantage.Core.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using static LoadVantage.Common.GeneralConstants.UserImage;


#nullable disable

namespace LoadVantage.Core.Services
{
    [Authorize]
    public class UserService(UserManager<User> userManager, LoadVantageDbContext context, IImageService imageService) : IUserService
    {
        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<ProfileViewModel> GetUserInformation(Guid userId)
        {
	        var user =  await GetUserByIdAsync(userId);

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
		        UserImageUrl = user.UserImage!.ImageUrl,
		        ChangePasswordViewModel = new ChangePasswordViewModel(),
		        ImageFileUploadModel = new ImageFileUploadModel()
	        };

	        return viewModel;

        }

		public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return  await userManager.Users
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetDispatchersAsync()
        {
            return await userManager.Users
                .Where(u => u is Dispatcher)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetBrokersAsync()
        {
            return await userManager.Users
                .Where(u => u is Broker)
                .ToListAsync();
        }

        public async Task UpdateUserPositionAsync(Guid userId, string position)
        {
            var user = await GetUserByIdAsync(userId);

            if (user != null)
            {
                user.Position = position;
                await userManager.UpdateAsync(user);
            }
        }

        public async Task AssignUserRoleAsync(Guid userId, string role)
        {
            var user = await GetUserByIdAsync(userId);
            var isAlreadyInRole = await userManager.IsInRoleAsync(user, role);

            if (user != null && !isAlreadyInRole)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }

        public async Task UpdateUserImageAsync(Guid userId, IFormFile file)
        {
            var userImage = await context.UsersImages
                .SingleOrDefaultAsync(ui => ui.UserId == userId);

            // Delete the old image

            if (userImage != null)
            {
                await DeleteUserImageAsync(userId, userImage.Id); 
            }

            // Upload the new image

            var uploadResult = await imageService.UploadImageAsync(file);

            if (uploadResult.StatusCode == HttpStatusCode.OK)
            {
                var resultImageUrl = uploadResult.SecureUrl.ToString();
                var publicId = uploadResult.PublicId;

                if (userImage == null)
                {
                    userImage = new UserImage
                    {
                        UserId = userId,
                        ImageUrl = resultImageUrl,
                        PublicId = publicId
                    };
                    await context.UsersImages.AddAsync(userImage);
				}
				else
                {
                    userImage.ImageUrl = resultImageUrl;
                    userImage.PublicId = publicId;
                }

                // Add reference to the user in the User's table

				var user = await userManager.FindByIdAsync(userId.ToString());
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
            var userImage = await context.UsersImages
                .SingleOrDefaultAsync(ui => ui.UserId == userId);


            if (userImage != null)
            {
                if (!string.IsNullOrEmpty(userImage.PublicId) && userImage.PublicId != DefaultImagePath)
                {
                    try
                    {
                        var deleteResult = await imageService.DeleteImageAsync(userImage.PublicId);

                        if (!deleteResult.IsSuccess)
                        {
                            throw new Exception($"{ErrorRemovingImage} {deleteResult.Message}");
                        }

                        userImage.ImageUrl = DefaultImagePath; // Default image URL
                        userImage.PublicId = DefaultImagePath; // Set PublicId to the default image string
                    }
                    catch (Exception ex)
                    {
                        userImage.ImageUrl = DefaultImagePath; // Default to local image
                        userImage.PublicId = DefaultImagePath; // Set PublicId to the default image string
                    }
                }
                else
                {
                    userImage.ImageUrl = DefaultImagePath; // Local default image URL
                    userImage.PublicId = DefaultImagePath; // Set PublicId to default image string
                }

                await context.SaveChangesAsync();
            }

        }
	}

}
