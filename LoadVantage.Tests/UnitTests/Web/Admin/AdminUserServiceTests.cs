using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CloudinaryDotNet.Actions;

using Moq;
using MockQueryable;
using NUnit.Framework;

using LoadVantage.Areas.Admin.Services;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Image;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;

using static LoadVantage.Common.GeneralConstants.UserImage;


namespace LoadVantage.Tests.UnitTests.Web.Admin
{
    public class AdminUserServiceTests
    {

        private Mock<UserManager<BaseUser>> _userManager;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private LoadVantageDbContext _dbContext;
        private Mock<IImageService> _mockImageService;

        private AdminUserService _adminUserService;

        [SetUp]
        public void Setup()
        {
            var mockUserStore = new Mock<IUserStore<BaseUser>>();

            var mockOptions = new Mock<IOptions<IdentityOptions>>();
            mockOptions.Setup(o => o.Value).Returns(new IdentityOptions());

            _userManager = new Mock<UserManager<BaseUser>>(
                mockUserStore.Object,
                mockOptions.Object,
                new PasswordHasher<BaseUser>(),
                new List<IUserValidator<BaseUser>>(),
                new List<IPasswordValidator<BaseUser>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<BaseUser>>>().Object
            );

            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());

            var options = new DbContextOptionsBuilder<LoadVantageDbContext>()
                .UseInMemoryDatabase(databaseName: "LoadVantageTestInMemoryDB")
                .Options;

            _dbContext = new LoadVantageDbContext(options);

            _mockImageService = new Mock<IImageService>();

            _adminUserService = new AdminUserService(
                _userManager.Object,
                _httpContextAccessor.Object,
                _dbContext,
                _mockImageService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }


        [Test]
        public async Task GetCurrentAdminAsync_ShouldReturnAdmin_WhenValidAdminIdClaimExists()
        {
            var adminId = Guid.NewGuid().ToString();
            var adminUser = new Administrator
            {
                Id = Guid.Parse(adminId),
                UserName = "adminUser"
            };

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, adminId)
            }));

            _httpContextAccessor.Setup(h => h.HttpContext.User).Returns(claimsPrincipal);
            _userManager.Setup(um => um.FindByIdAsync(adminId)).ReturnsAsync(adminUser);

            var result = await _adminUserService.GetCurrentAdminAsync();

            Assert.That(result, Is.EqualTo(adminUser));
        }

        [Test]
        public async Task GetCurrentAdminAsync_ShouldReturnNull_WhenAdminIdClaimIsMissing()
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            _httpContextAccessor.Setup(h => h.HttpContext.User).Returns(claimsPrincipal);

            var result = await _adminUserService.GetCurrentAdminAsync();

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCurrentAdminAsync_ShouldReturnNull_WhenAdminIdClaimIsEmpty()
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, string.Empty)
            }));
            _httpContextAccessor.Setup(h => h.HttpContext.User).Returns(claimsPrincipal);

            var result = await _adminUserService.GetCurrentAdminAsync();

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAdminByIdAsync_ShouldReturnAdmin_WhenAdminExists()
        {
            var adminId = Guid.NewGuid();
            var adminUser = new Administrator
            {
                Id = adminId,
                UserName = "adminUser"
            };

            _userManager.Setup(um => um.FindByIdAsync(adminId.ToString())).ReturnsAsync(adminUser);

            var result = await _adminUserService.GetAdminByIdAsync(adminId);

            Assert.That(result, Is.EqualTo(adminUser));
        }

        [Test]
        public async Task GetAdminByIdAsync_ShouldReturnNull_WhenAdminDoesNotExist()
        {
            var adminId = Guid.NewGuid();

            _userManager.Setup(um => um.FindByIdAsync(adminId.ToString())).ReturnsAsync((Administrator)null);

            var result = await _adminUserService.GetAdminByIdAsync(adminId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCurrentAdministratorAsync_ShouldReturnAdministrator_WhenAdminIdExists()
        {
            var adminId = Guid.NewGuid();
            var administrator = new Administrator
            {
                Id = adminId,
                UserName = "adminUser"
            };

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, adminId.ToString())
            }, "mock");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var httpContext = new DefaultHttpContext();
            httpContext.User = claimsPrincipal;

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(httpContext);

            _userManager.Setup(um => um.FindByIdAsync(adminId.ToString())).ReturnsAsync(administrator);

            var adminUserService = new AdminUserService(
                _userManager.Object,
                httpContextAccessor.Object,
                _dbContext,
                _mockImageService.Object);

            var result = await adminUserService.GetCurrentAdministratorAsync();

            Assert.That(result, Is.EqualTo(administrator));
        }

        [Test]
        public async Task GetCurrentAdministratorAsync_ShouldReturnNull_WhenAdminIdDoesNotExist()
        {
            var claimsPrincipal = new Mock<ClaimsPrincipal>();
            var claimsIdentity = new Mock<ClaimsIdentity>();

            claimsPrincipal.Setup(cp => cp.Identity).Returns(claimsIdentity.Object);
            claimsIdentity.Setup(ci => ci.FindFirst(ClaimTypes.NameIdentifier)).Returns((Claim)null);

            _httpContextAccessor.Setup(h => h.HttpContext.User).Returns(claimsPrincipal.Object);

            var result = await _adminUserService.GetCurrentAdministratorAsync();

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCurrentAdministratorAsync_ShouldReturnNull_WhenAdministratorNotFound()
        {
            var adminId = Guid.NewGuid();

            var claimsPrincipal = new Mock<ClaimsPrincipal>();
            var claimsIdentity = new Mock<ClaimsIdentity>();

            claimsPrincipal.Setup(cp => cp.Identity).Returns(claimsIdentity.Object);
            claimsIdentity.Setup(ci => ci.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, adminId.ToString()));

            _httpContextAccessor.Setup(h => h.HttpContext.User).Returns(claimsPrincipal.Object);
            _userManager.Setup(um => um.FindByIdAsync(adminId.ToString())).ReturnsAsync((BaseUser)null);

            var result = await _adminUserService.GetCurrentAdministratorAsync();

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            var users = new List<BaseUser>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "user1"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "user2"
                }
            };

            var mockUserQueryable = users.AsQueryable().BuildMock();
            _userManager.Setup(um => um.Users).Returns(mockUserQueryable);

            var result = await _adminUserService.GetAllUsersAsync();

            Assert.That(result, Is.EqualTo(users));
        }

        [Test]
        public async Task GetDispatchersAsync_ShouldReturnOnlyDispatchers_IfThereIsAnAdministratorInTheList()
        {
            var users = new List<BaseUser>
            {
                new Dispatcher()
                {
                    Id = Guid.NewGuid(),
                    UserName = "dispatcher1"
                },
                new Dispatcher()
                {
                    Id = Guid.NewGuid(),
                    UserName = "dispatcher2"
                },
                new Administrator()
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin"
                }
            };

            var mockUserQueryable = users.AsQueryable().BuildMock();
            _userManager.Setup(um => um.Users).Returns(mockUserQueryable);

            var result = await _adminUserService.GetDispatchersAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(u => u is Dispatcher), Is.True);
            Assert.That(result.Select(u => u.UserName), Is.EquivalentTo(new[] { "dispatcher1", "dispatcher2" }));
        }

        [Test]
        public async Task GetDispatchersAsync_ShouldReturnOnlyDispatchers_IfThereIsABrokerInTheList()
        {
            var users = new List<BaseUser>
            {
                new Dispatcher()
                {
                    Id = Guid.NewGuid(),
                    UserName = "dispatcher1"
                },
                new Dispatcher()
                {
                    Id = Guid.NewGuid(),
                    UserName = "dispatcher2"
                },
                new Broker()
                {
                    Id = Guid.NewGuid(),
                    UserName = "broker"
                }
            };

            var mockUserQueryable = users.AsQueryable().BuildMock();
            _userManager.Setup(um => um.Users).Returns(mockUserQueryable);

            var result = await _adminUserService.GetDispatchersAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(u => u is Dispatcher), Is.True);
            Assert.That(result.Select(u => u.UserName), Is.EquivalentTo(new[] { "dispatcher1", "dispatcher2" }));
        }

        [Test]
        public async Task GetBrokersAsync_ShouldReturnOnlyBrokers_IfThereIsADispatcherInTheList()
        {
            var users = new List<BaseUser>
            {
                new Broker() { Id = Guid.NewGuid(), UserName = "broker1" },
                new Broker() { Id = Guid.NewGuid(), UserName = "broker2" },
                new Dispatcher() { Id = Guid.NewGuid(), UserName = "dispatcher1" }
            };

            var mockUserQueryable = users.AsQueryable().BuildMock();
            _userManager.Setup(um => um.Users).Returns(mockUserQueryable);

            var result = await _adminUserService.GetBrokersAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(u => u is Broker), Is.True);
            Assert.That(result.Select(u => u.UserName), Is.EquivalentTo(new[] { "broker1", "broker2" }));
        }

        [Test]
        public async Task GetBrokersAsync_ShouldReturnOnlyBrokers_IfThereIsAnAdministratorInTheList()
        {
            var users = new List<BaseUser>
            {
                new Broker() { Id = Guid.NewGuid(), UserName = "broker1" },
                new Broker() { Id = Guid.NewGuid(), UserName = "broker2" },
                new Administrator() { Id = Guid.NewGuid(), UserName = "admin" }
            };

            var mockUserQueryable = users.AsQueryable().BuildMock();
            _userManager.Setup(um => um.Users).Returns(mockUserQueryable);

            var result = await _adminUserService.GetBrokersAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(u => u is Broker), Is.True);
            Assert.That(result.Select(u => u.UserName), Is.EquivalentTo(new[] { "broker1", "broker2" }));
        }

        [Test]
        public async Task UpdateUserImageAsync_ShouldUpdateUserImage_WhenImageIsUploadedSuccessfully()
        {
            var userId = Guid.NewGuid();
            var oldImageId = Guid.NewGuid();
            var newImageId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();

            var adminUser = new Administrator
            {
                Id = userId,
                FirstName = "Chuck",
                LastName = "Norris",
                UserName = "god-mode",
                Email = "gmail@chucknorris.com",
                UserImageId = oldImageId
            };

            var oldUserImage = new UserImage
            {
                Id = oldImageId,
                ImageUrl = "old-image-url",
                PublicId = "old-public-id"
            };

            await _dbContext.Users.AddAsync(adminUser);
            await _dbContext.UsersImages.AddAsync(oldUserImage);
            await _dbContext.SaveChangesAsync();

            _userManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(adminUser);

            var uploadResult = new ImageUploadResult
            {
                StatusCode = HttpStatusCode.OK,
                SecureUrl = new Uri("https://new-image-url/"),
                PublicId = "new-public-id"
            };

            _mockImageService.Setup(s => s.UploadImageAsync(fileMock.Object)).ReturnsAsync(uploadResult);

            _mockImageService.Setup(s => s.DeleteImageAsync(oldUserImage.PublicId)).ReturnsAsync(new DeleteImageResult() { IsSuccess = true });

            await _adminUserService.UpdateUserImageAsync(userId, fileMock.Object);

            var updatedUser = await _dbContext.Users.SingleAsync(u => u.Id == userId);
            var newUserImage = await _dbContext.UsersImages.SingleAsync(ui => ui.Id == updatedUser.UserImageId);

            Assert.That(newUserImage.ImageUrl, Is.EqualTo("https://new-image-url/"));
            Assert.That(newUserImage.PublicId, Is.EqualTo("new-public-id"));
            Assert.That(updatedUser.UserImageId, Is.EqualTo(newUserImage.Id));
        }

        [Test]
        public async Task UpdateUserImageAsync_ShouldThrowException_WhenImageUploadFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var oldImageId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();

            var adminUser = new Administrator
            {
                Id = userId,
                FirstName = "Chuck",
                LastName = "Norris",
                UserName = "god-mode",
                Email = "gmail@chucknorris.com",
                UserImageId = oldImageId
            };

            _userManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(adminUser);

            var uploadResult = new ImageUploadResult
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            _mockImageService.Setup(s => s.UploadImageAsync(fileMock.Object)).ReturnsAsync(uploadResult);

            var ex = Assert.ThrowsAsync<Exception>(() => _adminUserService.UpdateUserImageAsync(userId, fileMock.Object));

            Assert.That(ex.Message, Is.EqualTo(ImageUploadFailed));
        }

        [Test]
        public async Task UpdateUserImageAsync_ShouldRetainDefaultImage_WhenNoPreviousImageExists()
        {
            var userId = Guid.NewGuid();
            var defaultImageId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();

            var defaultUserImage = new UserImage
            {
                Id = DefaultImageId,
                ImageUrl = DefaultImagePath,
                PublicId = DefaultPublicId
            };

            await _dbContext.UsersImages.AddAsync(defaultUserImage);

            var adminUser = new Administrator
            {
                Id = userId,
                FirstName = "Chuck",
                LastName = "Norris",
                UserName = "god-mode",
                Email = "gmail@chucknorris.com",
                UserImageId = defaultImageId
            };

            await _dbContext.Users.AddAsync(adminUser);
            await _dbContext.SaveChangesAsync();

            _userManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(adminUser);

            var uploadResult = new ImageUploadResult
            {
                StatusCode = HttpStatusCode.OK,
                SecureUrl = new Uri("https://new-image-url/"),
                PublicId = "new-public-id"
            };

            _mockImageService.Setup(s => s.UploadImageAsync(fileMock.Object)).ReturnsAsync(uploadResult);

            await _adminUserService.UpdateUserImageAsync(userId, fileMock.Object);

            var updatedUser = await _dbContext.Users.SingleAsync(u => u.Id == userId);
            var newUserImage = await _dbContext.UsersImages.SingleAsync(ui => ui.Id == updatedUser.UserImageId);

            Assert.That(newUserImage.ImageUrl, Is.EqualTo("https://new-image-url/"));
            Assert.That(newUserImage.PublicId, Is.EqualTo("new-public-id"));
            Assert.That(updatedUser.UserImageId, Is.EqualTo(newUserImage.Id));
        }

        [Test]
        public async Task UpdateUserImageAsync_ShouldDeleteOldImage_WhenOldImageExists()
        {
            var userId = Guid.NewGuid();
            var oldImageId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();

            var adminUser = new Administrator
            {
                Id = userId,
                FirstName = "Chuck",
                LastName = "Norris",
                UserName = "god-mode",
                Email = "gmail@chucknorris.com",
                UserImageId = oldImageId
            };

            var oldUserImage = new UserImage
            {
                Id = oldImageId,
                ImageUrl = "old-image-url",
                PublicId = "old-public-id"
            };

            await _dbContext.Users.AddAsync(adminUser);
            await _dbContext.UsersImages.AddAsync(oldUserImage);
            await _dbContext.SaveChangesAsync();

            _userManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(adminUser);

            var uploadResult = new ImageUploadResult
            {
                StatusCode = HttpStatusCode.OK,
                SecureUrl = new Uri("https://new-image-url"),
                PublicId = "new-public-id"
            };

            _mockImageService.Setup(s => s.UploadImageAsync(fileMock.Object)).ReturnsAsync(uploadResult);
            _mockImageService.Setup(s => s.DeleteImageAsync(oldUserImage.PublicId)).ReturnsAsync(new DeleteImageResult { IsSuccess = true });

            await _adminUserService.UpdateUserImageAsync(userId, fileMock.Object);

            _mockImageService.Verify(s => s.DeleteImageAsync(oldUserImage.PublicId), Times.Once);
        }

        [Test]
        public async Task DeleteUserImageAsync_ShouldDeleteImage_WhenUserAndImageExist()
        {
            var userId = Guid.NewGuid();
            var imageId = Guid.NewGuid();

            var defaultUserImage = new UserImage
            {
                Id = DefaultImageId,
                ImageUrl = DefaultImagePath,
                PublicId = DefaultPublicId
            };

            var userImage = new UserImage
            {
                Id = imageId,
                PublicId = "public-id-to-delete",
                ImageUrl = "https://image-url"
            };

            var adminUser = new Administrator()
            {
                Id = userId,
                FirstName = "Chuck",
                LastName = "Norris",
                UserName = "god-mode",
                Email = "gmail@chucknorris.com",
                UserImage = userImage,
                UserImageId = imageId
            };

            await _dbContext.UsersImages.AddAsync(userImage);
            await _dbContext.UsersImages.AddAsync(defaultUserImage);
            await _dbContext.Users.AddAsync(adminUser);

            await _dbContext.SaveChangesAsync();

            _mockImageService.Setup(s => s.DeleteImageAsync(userImage.PublicId))
                .ReturnsAsync(new DeleteImageResult { IsSuccess = true });

            _userManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(adminUser);

            await _adminUserService.DeleteUserImageAsync(userId, imageId);

            var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            Assert.That(await _dbContext.UsersImages.AnyAsync(ui => ui.Id == imageId), Is.False);
            Assert.That(updatedUser.UserImageId, Is.EqualTo(DefaultImageId));
        }

        [Test]
        public async Task DeleteUserImageAsync_ShouldThrowException_WhenImageDeletionFails()
        {
            var userId = Guid.NewGuid();
            var imageId = Guid.NewGuid();

            var userImage = new UserImage
            {
                Id = imageId,
                PublicId = "public-id-to-delete",
                ImageUrl = "https://image-url"
            };

            var adminUser = new Administrator
            {
                Id = userId,
                FirstName = "Chuck",
                LastName = "Norris",
                UserName = "god-mode",
                Email = "gmail@chucknorris.com",
                UserImageId = imageId
            };

            await _dbContext.UsersImages.AddAsync(userImage);
            await _dbContext.Users.AddAsync(adminUser);
            await _dbContext.SaveChangesAsync();

            _userManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(adminUser);

            _mockImageService.Setup(s => s.DeleteImageAsync(userImage.PublicId))
                .ReturnsAsync(new DeleteImageResult
                {
                    IsSuccess = false,
                    Message = "Error deleting image."
                });

            var ex = Assert.ThrowsAsync<Exception>(() => _adminUserService.DeleteUserImageAsync(userId, imageId));
            Assert.That(ex.Message, Is.EqualTo("Error removing the old profile image:  Error deleting image."));
        }

        [Test]
        public async Task GetUserCountAsync_ShouldReturnCorrectUserCount()
        {
            int userCount = 2;

            await _dbContext.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Chuck",
                LastName = "Norris",
                UserName = "god-mode",
                Email = "gmail@chucknorris.com",
                Position = "Dispatcher"
            });

            await _dbContext.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                Email = "batman@chucknorris.com",
                Position = "Broker"
            });

            await _dbContext.SaveChangesAsync();

            var result = await _adminUserService.GetUserCountAsync();

            Assert.That(userCount.Equals(result));
        }

        [Test]
        public async Task GetUserCountAsync_ShouldReturnCorrectDispatcherCount()
        {
            int userCount = 1;

            await _dbContext.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Chuck",
                LastName = "Norris",
                UserName = "god-mode",
                Email = "gmail@chucknorris.com",
                Position = "Dispatcher",
            });

            await _dbContext.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                Email = "batman@chucknorris.com",
                Position = "Broker"
            });

            await _dbContext.SaveChangesAsync();

            var result = await _adminUserService.GetDispatcherCountAsync();

            Assert.That(userCount.Equals(result));
        }

        [Test]
        public async Task GetUserCountAsync_ShouldReturnCorrectBrokersCount()
        {
            int userCount = 1;

            await _dbContext.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Chuck",
                LastName = "Norris",
                UserName = "god-mode",
                Email = "gmail@chucknorris.com",
                Position = "Dispatcher",
            });

            await _dbContext.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                Email = "batman@chucknorris.com",
                Position = "Broker"
            });

            await _dbContext.SaveChangesAsync();

            var result = await _adminUserService.GetBrokerCountAsync();

            Assert.That(userCount.Equals(result));
        }

        [Test]
        public async Task GetAllUsersFromACompany_ShouldReturnOnlyUsersWithCompanyName()
        {

            await _dbContext.Users.AddAsync(new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "Chuck",
                LastName = "Norris",
                UserName = "god-mode",
                Email = "gmail@chucknorris.com",
                CompanyName = "CompanyA"
            });

            await _dbContext.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                Email = "batman@chucknorris.com",
                CompanyName = "CompanyB"
            });

            await _dbContext.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Gosho",
                LastName = "Ot Pochivka",
                UserName = "chalga-forever",
                Email = "chalgar@ciganiq.bg",
                CompanyName = null
            });

            await _dbContext.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Slavi",
                LastName = "Trifonov",
                UserName = "dylgiq",
                Email = "100metra@7-8dni-shte-si-padnala.com",
                CompanyName = "CompanyA"
            });

            await _dbContext.SaveChangesAsync();

            var result = await _adminUserService.GetAllUsersFromACompany();

            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result.All(user => !string.IsNullOrEmpty(user.CompanyName)), Is.True);
        }

        [Test]
        public async Task DeleteUserPassword_ShouldReturnSuccess_WhenPasswordRemovedSuccessfully()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                Email = "batman@chucknorris.com",
            };

            _userManager.Setup(um => um.RemovePasswordAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _adminUserService.DeleteUserPassword(user);

            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task DeleteUserPassword_ShouldThrowArgumentNullException_WhenUserIsNull()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _adminUserService.DeleteUserPassword(null));

            Assert.That(ex.ParamName, Is.EqualTo("user"));
        }

        [Test]
        public async Task DeleteUserPassword_ShouldReturnFailure_WhenPasswordRemovalFails()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                Email = "batman@chucknorris.com",
            };

            var identityResultFailed = IdentityResult.Failed(new IdentityError { Description = "Password removal failed" });
            _userManager.Setup(um => um.RemovePasswordAsync(user))
                .ReturnsAsync(identityResultFailed);

            var result = await _adminUserService.DeleteUserPassword(user);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
        }

        [Test]
        public async Task AddUserDefaultPassword_ShouldReturnSuccess_WhenPasswordAddedSuccessfully()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                Email = "batman@chucknorris.com",
            };

            _userManager.Setup(um => um.AddPasswordAsync(user, It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _adminUserService.AddUserDefaultPassword(user);

            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public void AddUserDefaultPassword_ShouldThrowArgumentNullException_WhenUserIsNull()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _adminUserService.AddUserDefaultPassword(null));

            Assert.That(ex.ParamName, Is.EqualTo("user"));
        }

        [Test]
        public async Task AddUserDefaultPassword_ShouldReturnFailure_WhenPasswordAdditionFails()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                Email = "batman@chucknorris.com",
            };

            var identityResultFailed = IdentityResult.Failed(new IdentityError { Description = "Password addition failed" });
            _userManager.Setup(um => um.AddPasswordAsync(user, It.IsAny<string>()))
                .ReturnsAsync(identityResultFailed);

            var result = await _adminUserService.AddUserDefaultPassword(user);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
        }
    }
}
