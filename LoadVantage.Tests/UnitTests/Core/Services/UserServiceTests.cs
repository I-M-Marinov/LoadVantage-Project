using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NUnit.Framework;
using Moq;

using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data.Models;

using LoadVantage.Core.Models.Image;
using LoadVantage.Core.Models.Profile;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using static LoadVantage.Common.GeneralConstants.UserImage;

using UserImage = LoadVantage.Infrastructure.Data.Models.UserImage;

namespace LoadVantage.Tests.UnitTests.Core.Services
{
    public class UserServiceTests
    {
        private Mock<UserManager<BaseUser>> _mockUserManager;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private LoadVantageDbContext _dbContext;
        private Mock<IImageService> _mockImageService;
        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            _mockUserManager = new Mock<UserManager<BaseUser>>(
                new Mock<IUserStore<BaseUser>>().Object,
                null, null, null, null, null, null, null, null);

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());

            var options = new DbContextOptionsBuilder<LoadVantageDbContext>()
                .UseInMemoryDatabase(databaseName: "LoadVantageTestInMemoryDB")
                .Options;

            _dbContext = new LoadVantageDbContext(options);

            _mockImageService = new Mock<IImageService>();

            _userService = new UserService(
                _mockUserManager.Object,
                _mockHttpContextAccessor.Object,
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
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var expectedUser = new User()
            {
                Id = userId,
                UserName = "Batman"
            };

            _mockUserManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(expectedUser);

            var result = await _userService.GetUserByIdAsync(userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(userId));
            Assert.That(result.UserName, Is.EqualTo("Batman"));
            _mockUserManager.Verify(um => um.FindByIdAsync(userId.ToString()), Times.Once);

        }


        [Test]
        public async Task UpdateUserAsync_ShouldReturnSuccess_WhenUpdateIsSuccessful()
        {
            var testUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = "Batman"
            };
            var successResult = IdentityResult.Success;

            _mockUserManager.Setup(um => um.UpdateAsync(testUser))
                .ReturnsAsync(successResult);

            var result = await _userService.UpdateUserAsync(testUser);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(successResult));
            Assert.That(result.Succeeded, Is.True);

            _mockUserManager.Verify(um => um.UpdateAsync(testUser), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_ShouldReturnFailure_WhenUpdateFails()
        {
            var testUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = "Batman"
            };

            var failureResult = IdentityResult.Failed(new IdentityError { Description = "Update failed" });

            _mockUserManager.Setup(um => um.UpdateAsync(testUser))
                .ReturnsAsync(failureResult);

            var result = await _userService.UpdateUserAsync(testUser);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(failureResult));
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors.First().Description, Is.EqualTo("Update failed"));

            _mockUserManager.Verify(um => um.UpdateAsync(testUser), Times.Once);
        }


        [Test]
        public async Task GetCurrentUserAsync_ShouldReturnNull_WhenUserIdClaimIsMissing()
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            _mockHttpContextAccessor.Setup(h => h.HttpContext.User).Returns(claimsPrincipal);

            var result = await _userService.GetCurrentUserAsync();

            Assert.That(result, Is.Null);

            _mockHttpContextAccessor.Verify(h => h.HttpContext.User, Times.Once);
            _mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetCurrentUserAsync_ShouldReturnUser_WhenUserIdClaimIsPresent()
        {
            var testUserId = Guid.NewGuid().ToString();
            var testUser = new User
            {
                Id = Guid.Parse(testUserId),
                UserName = "Batman"
            };

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, testUserId) }));

            _mockHttpContextAccessor.Setup(h => h.HttpContext.User).Returns(claimsPrincipal);
            _mockUserManager.Setup(um => um.FindByIdAsync(testUserId)).ReturnsAsync(testUser);

            var result = await _userService.GetCurrentUserAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(testUser));
            Assert.That(result.UserName, Is.EqualTo("Batman"));

            _mockHttpContextAccessor.Verify(h => h.HttpContext.User, Times.Once);
            _mockUserManager.Verify(um => um.FindByIdAsync(testUserId), Times.Once);
        }

        [Test]
        public async Task GetCurrentUserAsync_ShouldReturnUser_WhenUserIdClaimExists()
        {
            var userId = Guid.NewGuid().ToString();
            var claim = new Claim(ClaimTypes.NameIdentifier, userId);
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { claim }));
            _mockHttpContextAccessor.Setup(h => h.HttpContext.User).Returns(principal);

            var mockUser = new User { Id = Guid.Parse(userId) };
            _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(mockUser);

            var result = await _userService.GetCurrentUserAsync();

            Assert.That(result, Is.EqualTo(mockUser));
        }

        [Test]
        public void FindUserByEmailAsync_ShouldThrowArgumentException_WhenEmailIsNull()
        {
            string invalidEmail = null;

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.FindUserByEmailAsync(invalidEmail));

            Assert.That(ex.Message, Does.Contain("email"));
        }

        [Test]
        public async Task FindUserByEmailAsync_ShouldReturnUser_WhenEmailIsValid()
        {
            var email = "batman@gmail.com";
            var mockUser = new User { Email = email };
            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(mockUser);

            var result = await _userService.FindUserByEmailAsync(email);

            Assert.That(result, Is.EqualTo(mockUser));
        }

        [Test]
        public async Task FindUserByEmailAsync_ShouldReturnNull_WhenUserNotFound()
        {
            var email = "nobody@whatever.com";
            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync((User)null);

            var result = await _userService.FindUserByEmailAsync(email);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void FindUserByUsernameAsync_ShouldThrowArgumentException_WhenUsernameIsNullOrWhitespace()
        {
            string username = null;

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.FindUserByUsernameAsync(username));

            Assert.That(ex.Message, Does.Contain("username"));
        }

        [Test]
        public async Task FindUserByUsernameAsync_ShouldReturnUser_WhenUsernameIsValid()
        {
            var username = "batman";
            var mockUser = new User { UserName = username };
            _mockUserManager.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(mockUser);

            var result = await _userService.FindUserByUsernameAsync(username);

            Assert.That(result, Is.EqualTo(mockUser));
        }

        [Test]
        public async Task FindUserByUsernameAsync_ShouldReturnNull_WhenUserNotFound()
        {
            var username = "nobody";
            _mockUserManager.Setup(um => um.FindByNameAsync(username)).ReturnsAsync((User)null);

            var result = await _userService.FindUserByUsernameAsync(username);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateUserAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
        {
            User user = null;

            string password = "password123";

            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _userService.CreateUserAsync(user, password));

            Assert.That(ex.Message, Does.Contain(UserCannotBeNull));
        }

        [Test]
        public void CreateUserAsync_ShouldThrowArgumentException_WhenPasswordIsNullOrWhiteSpace()
        {
            var user = new User();
            string password = string.Empty;

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.CreateUserAsync(user, password));

            Assert.That(ex.Message, Does.Contain(PasswordCannotBeNull));
        }

        [Test]
        public async Task CreateUserAsync_ShouldCallCreateAsync_WhenUserAndPasswordAreValid()
        {

            var user = new User
            {
                UserName = "batman",
                Email = "batman@gmail.com"
            };

            string password = "password456";

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var result = await _userService.CreateUserAsync(user, password);

            _mockUserManager.Verify(um => um.CreateAsync(user, password), Times.Once);

            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public void AssignUserRoleAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
        {
            User user = null;
            string role = "Admin";

            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _userService.AssignUserRoleAsync(user, role));

            Assert.That(ex.Message, Does.Contain(UserCannotBeNull));
        }

        [Test]
        public void AssignUserRoleAsync_ShouldThrowArgumentException_WhenRoleIsNullOrWhiteSpace()
        {
            var user = new User
            {
                UserName = "batman",
                Email = "batman@gmail.com"
            };
            string role = string.Empty;

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.AssignUserRoleAsync(user, role));

            Assert.That(ex.Message, Does.Contain(RoleCannotBeNull));
        }

        [Test]
        public async Task AssignUserRoleAsync_ShouldCallAddToRoleAsync_WhenUserIsNotInRole()
        {
            var user = new User
            {
                UserName = "batman",
                Email = "batman@gmail.com"
            };
            string role = "Admin";

            _mockUserManager.Setup(um => um.IsInRoleAsync(user, role)).ReturnsAsync(false);
            _mockUserManager.Setup(um => um.AddToRoleAsync(user, role)).ReturnsAsync(IdentityResult.Success);

            var result = await _userService.AssignUserRoleAsync(user, role);

            _mockUserManager.Verify(um => um.AddToRoleAsync(user, role), Times.Once);
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public void AssignUserRoleAsync_ShouldThrowArgumentException_WhenRoleAlreadyAssigned()
        {
            var user = new User
            {
                UserName = "batman",
                Email = "batman@gmail.com"
            };
            string role = "Admin"; // Valid role

            _mockUserManager.Setup(um => um.IsInRoleAsync(user, role)).ReturnsAsync(true);

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.AssignUserRoleAsync(user, role));
            Assert.That(ex.Message, Does.Contain(RoleAlreadyAssignedToUser));
        }

        [Test]
        public void GetUserInformation_ShouldThrowException_WhenUserNotFound()
        {
            Guid userId = Guid.NewGuid();

            _mockUserManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync((User)null);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _userService.GetUserInformation(userId));

            Assert.That(ex.Message, Does.Contain(UserNotFound));
        }

        [Test]
        public async Task GetUserInformation_ShouldReturnProfileViewModel_WhenUserExists()
        {
            Guid userId = Guid.NewGuid();
            var user = new User()
            {
                Id = userId,
                UserName = "batman",
                Email = "batman@gmail.com",
                FirstName = "Bruce",
                LastName = "Wayne",
                CompanyName = "Wayne Enterprises",
                Position = "Broker",
                PhoneNumber = "800-666-9999",
                UserImageId = Guid.Empty
            };

            _mockUserManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);

            var result = await _userService.GetUserInformation(userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(userId.ToString()));
            Assert.That(result.Username, Is.EqualTo(user.UserName));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(result.LastName, Is.EqualTo(user.LastName));
            Assert.That(result.CompanyName, Is.EqualTo(user.CompanyName));
            Assert.That(result.Position, Is.EqualTo(user.Position));
            Assert.That(result.PhoneNumber, Is.EqualTo(user.PhoneNumber));
            Assert.That(result.UserImageUrl, Is.EqualTo(DefaultImagePath));  // default user image if UserImageId is empty Guid
            Assert.That(result.ChangePasswordViewModel, Is.InstanceOf<ChangePasswordViewModel>());
            Assert.That(result.ImageFileUploadModel, Is.InstanceOf<ImageFileUploadModel>());
        }

        [Test]
        public async Task GetUserInformation_ShouldReturnCorrectUserInformation()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                UserName = "batman",
                Email = "bruce@gmail.com",
                FirstName = "Bruce",
                LastName = "Wayne",
                PhoneNumber = "800-699-9966",
                CompanyName = "Wayne Enterprises",
                Position = "Broker",
                UserImageId = Guid.Empty
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            var result = await _userService.GetUserInformation(userId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(userId.ToString()));
            Assert.That(result.Username, Is.EqualTo(user.UserName));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(result.LastName, Is.EqualTo(user.LastName));
            Assert.That(result.PhoneNumber, Is.EqualTo(user.PhoneNumber));
            Assert.That(result.CompanyName, Is.EqualTo(user.CompanyName));
            Assert.That(result.Position, Is.EqualTo(user.Position));
            Assert.That(result.UserImageUrl, Is.EqualTo(DefaultImagePath));
        }

        [Test]
        public async Task GetUserImageUrlAsync_ShouldReturnUserImageUrl_WhenImageExists()
        {
            var userId = Guid.NewGuid();
            var userImageId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                UserName = "batman",
                Email = "batman@gmail.com",
                FirstName = "Bruce",
                LastName = "Wayne",
                UserImageId = userImageId
            };

            var userImage = new UserImage
            {
                Id = userImageId,
                ImageUrl = "http://example.com/user-image.jpg",
                PublicId = string.Empty
            };


            await _dbContext.Users.AddAsync(user);
            await _dbContext.UsersImages.AddAsync(userImage);
            await _dbContext.SaveChangesAsync();

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            var mockImageService = new Mock<IImageService>();

            var mockUserService = new UserService(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                _dbContext,
                mockImageService.Object);

            var result = await mockUserService.GetUserImageUrlAsync(userId);

            Assert.That(result, Is.EqualTo(userImage.ImageUrl)); // Should return the image URL from the database
        }

        [Test]
        public async Task GetUserClaimsAsync_ShouldReturnClaims_WhenUserHasClaims()
        {

            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                UserName = "batman",
                Email = "batman@gmail.com",
                FirstName = "Bruce",
                LastName = "Wayne",
                UserImageId = Guid.Empty
            };

            var expectedClaims = new List<Claim>
            {
                new Claim("Role", "Admin"),
                new Claim("Email", "testeronium@abv.bg")
            };


            _mockUserManager.Setup(um => um.GetClaimsAsync(user))
                .ReturnsAsync(expectedClaims);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());

            var mockImageService = new Mock<IImageService>();

            var userService = new UserService(
                _mockUserManager.Object,
                mockHttpContextAccessor.Object,
                _dbContext,
                mockImageService.Object);

            var result = await userService.GetUserClaimsAsync(user);

            Assert.That(result, Is.EqualTo(expectedClaims));
        }

        [Test]
        public async Task AddUserClaimAsync_ShouldAddClaim_WhenValidUserAndClaim()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                UserName = "batman",
                Email = "batman@gmail.com",
                FirstName = "Bruce",
                LastName = "Wayne",
                UserImageId = Guid.Empty
            };

            var claim = new Claim("Role", "Admin");

            _mockUserManager.Setup(um => um.AddClaimAsync(user, claim))
                .ReturnsAsync(IdentityResult.Success);

            _mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());


            var userService = new UserService(
                _mockUserManager.Object,
                _mockHttpContextAccessor.Object,
                _dbContext,
                _mockImageService.Object);

            var result = await userService.AddUserClaimAsync(user, claim);

            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public void AddUserClaimAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
        {
            User user = null;

            var claim = new Claim("Role", "Admin");

            var userService = new UserService(
                _mockUserManager.Object,
                _mockHttpContextAccessor.Object,
                _dbContext,
                _mockImageService.Object);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await userService.AddUserClaimAsync(user, claim));
        }

        [Test]
        public void AddUserClaimAsync_ShouldThrowArgumentNullException_WhenClaimIsNull()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                UserName = "batman",
                Email = "batman@gmail.com",
                FirstName = "Bruce",
                LastName = "Wayne",
                UserImageId = Guid.Empty
            };

            Claim claim = null;

            var userService = new UserService(
                _mockUserManager.Object,
                _mockHttpContextAccessor.Object,
                _dbContext,
                _mockImageService.Object);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await userService.AddUserClaimAsync(user, claim));
        }

        [Test]
        public async Task AddUserClaimsAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
        {
            User user = null;

            var claims = new List<Claim>
            {
                new Claim("Role", "Admin")
            };

            var userService = new UserService(
                _mockUserManager.Object,
                _mockHttpContextAccessor.Object,
                _dbContext,
                _mockImageService.Object);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await userService.AddUserClaimsAsync(user, claims));
        }

        [Test]
        public async Task AddUserClaimsAsync_ShouldThrowArgumentException_WhenClaimsIsNullOrEmpty()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                UserName = "batman",
                Email = "batman@gmail.com",
                FirstName = "Bruce",
                LastName = "Wayne",
                UserImageId = Guid.Empty
            };

            var userService = new UserService(
                _mockUserManager.Object,
                _mockHttpContextAccessor.Object,
                _dbContext,
                _mockImageService.Object);

            Assert.ThrowsAsync<ArgumentException>(async () => await userService.AddUserClaimsAsync(user, null));
        }

        [Test]
        public async Task AddUserClaimsAsync_ShouldCallAddClaimsAsync_WhenClaimsAreValid()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                UserName = "batman",
                Email = "batman@gmail.com",
                FirstName = "Bruce",
                LastName = "Wayne",
                UserImageId = Guid.Empty
            };

            var userService = new UserService(
                _mockUserManager.Object,
                _mockHttpContextAccessor.Object,
                _dbContext,
                _mockImageService.Object);


            IEnumerable<Claim> claims = new List<Claim>
            {
                new Claim("Role", "Admin"),
                new Claim("Permission", "Read")
            };

            _mockUserManager.Setup(um => um.AddClaimsAsync(user, claims))
                .ReturnsAsync(IdentityResult.Success);


            var result = await userService.AddUserClaimsAsync(user, claims);

            _mockUserManager.Verify(um => um.AddClaimsAsync(user, claims), Times.Once);
            Assert.That(result.Succeeded, Is.True);
        }

    }
}
