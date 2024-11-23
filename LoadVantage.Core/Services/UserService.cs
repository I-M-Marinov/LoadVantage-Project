using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using LoadVantage.Core.Models.Image;
using LoadVantage.Core.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using LoadVantage.Core.Models.Chat;



#nullable disable

namespace LoadVantage.Core.Services
{
    [Authorize]
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
		private readonly LoadVantageDbContext context;
        private readonly IImageService imageService;

        public UserService(UserManager<User> _userManager, IHttpContextAccessor _httpContextAccessor, LoadVantageDbContext _context, IImageService _imageService)
		{
            userManager = _userManager;
            httpContextAccessor = _httpContextAccessor;
			context = _context;
            imageService = _imageService;
		}

		

		public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<User> GetCurrentUserAsync()
        {
	        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

	        if (string.IsNullOrEmpty(userId))
	        {
		        return null;
			}

			return await userManager.FindByIdAsync(userId);
        }

		public async Task<User> FindUserByEmailAsync(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentException("Email cannot be null or empty.", nameof(email));

			return await userManager.FindByEmailAsync(email);
		}

		public async Task<User> FindUserByUsernameAsync(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
				throw new ArgumentException("Username cannot be null or empty.", nameof(username));

			return await userManager.FindByNameAsync(username);
		}

		public async Task<IdentityResult> CreateUserAsync(User user, string password)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrWhiteSpace(password))
				throw new ArgumentException("Password cannot be null or empty.", nameof(password));

			return await userManager.CreateAsync(user, password);
		}

		public async Task<IdentityResult> AssignUserRoleAsync(User user, string role)
		{

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrWhiteSpace(role))
				throw new ArgumentException("Role cannot be null or empty.", nameof(role));

			var isAlreadyInRole = await userManager.IsInRoleAsync(user, role);

			if (!isAlreadyInRole)
			{
				return await userManager.AddToRoleAsync(user, role);
			}

			throw new ArgumentException("That role is already assigned to the user.", nameof(role));

		}
		public async Task<IEnumerable<Claim>> GetUserClaimsAsync(User user)
		{
			var claims = await userManager.GetClaimsAsync(user);
			return claims;
		}

		public async Task<IdentityResult> AddUserClaimAsync(User user, Claim claim)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			return await userManager.AddClaimAsync(user, claim);
		}

		public async Task<IdentityResult> AddUserClaimsAsync(User user, IEnumerable<Claim> claims)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (claims == null || !claims.Any())
				throw new ArgumentException("Claims cannot be null or empty.", nameof(claims));

			return await userManager.AddClaimsAsync(user, claims);
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

        public async Task<ProfileViewModel> GetUserInformation(Guid userId)
        {
	        var user = await GetUserByIdAsync(userId);

	        if (user == null)
	        {
		        throw new Exception(UserNotFound);
	        }

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
			        Company = u.CompanyName
		        })
		        .AsNoTracking()
		        .FirstOrDefaultAsync();

	        return user;
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
