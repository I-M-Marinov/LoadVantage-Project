using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Services;
using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data.Models;

using Moq;
using NUnit.Framework;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using LoadVantage.Areas.Admin.Models.Profile;

namespace LoadVantage.Tests.UnitTests.Web.Admin
{
    public class AdminProfileServiceTests
    {
        private Mock<IUserService> _mockUserService;
        private Mock<IAdminUserService> _mockAdminUserService;
        private Mock<UserManager<BaseUser>> _mockUserManager;
        private Mock<SignInManager<BaseUser>> _mockSignInManager;
        private Mock<IProfileHelperService> _mockProfileHelperService;
        private Mock<IHtmlSanitizerService> _mockHtmlSanitizer;

        private IAdminProfileService _profileService;

        [SetUp]
        public void SetUp()
        {
            _mockUserService = new Mock<IUserService>();
            _mockAdminUserService = new Mock<IAdminUserService>();

            var _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var _mockClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<BaseUser>>();
            var mockUserStore = new Mock<IUserStore<BaseUser>>();

            var mockOptions = new Mock<IOptions<IdentityOptions>>();
            mockOptions.Setup(o => o.Value).Returns(new IdentityOptions());

            _mockUserManager = new Mock<UserManager<BaseUser>>(
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

            _mockSignInManager = new Mock<SignInManager<BaseUser>>(
                _mockUserManager.Object,
                _mockHttpContextAccessor.Object,
                _mockClaimsPrincipalFactory.Object,
                null, // OptionsAccessor
                null, // Logger
                null, // UrlEncoder
                null // SystemClock
            );

            _mockProfileHelperService = new Mock<IProfileHelperService>();
            _mockHtmlSanitizer = new Mock<IHtmlSanitizerService>();

            _profileService = new AdminProfileService(
                _mockUserService.Object,
                _mockAdminUserService.Object,
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockProfileHelperService.Object,
                _mockHtmlSanitizer.Object
            );

        }

        [Test]
        public void GetAdminInformation_UserNotFound_ThrowsException()
        {
            var adminId = Guid.NewGuid();

            _mockUserService.Setup(s => s.GetUserByIdAsync(adminId)).ReturnsAsync((BaseUser?)null);

            Assert.That(async () => await _profileService.GetAdminInformation(adminId),
                Throws.TypeOf<Exception>().With.Message.EqualTo(UserNotFound));
        }

        [Test]
        public async Task GetAdminInformation_UserFound_ReturnsPopulatedProfile()
        {
            var adminId = Guid.NewGuid();

            var mockUser = new User()
            {
                Id = adminId,
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Position = "Admin",
                CompanyName = "Test Company",
                PhoneNumber = "123-456-7890",
                Email = "john.doe@example.com"
            };

            _mockUserService.Setup(s => s.GetUserByIdAsync(adminId)).ReturnsAsync(mockUser);
            _mockUserService.Setup(s => s.GetUserImageUrlAsync(adminId)).ReturnsAsync("http://imageurl.com");

            var result = await _profileService.GetAdminInformation(adminId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(mockUser.Id.ToString()));
            Assert.That(result.FirstName, Is.EqualTo(mockUser.FirstName));
            Assert.That(result.LastName, Is.EqualTo(mockUser.LastName));
            Assert.That(result.Username, Is.EqualTo(mockUser.UserName));
            Assert.That(result.Position, Is.EqualTo(mockUser.Position));
            Assert.That(result.CompanyName, Is.EqualTo(mockUser.CompanyName));
            Assert.That(result.PhoneNumber, Is.EqualTo(mockUser.PhoneNumber));
            Assert.That(result.Email, Is.EqualTo(mockUser.Email));
            Assert.That(result.UserImageUrl, Is.EqualTo("http://imageurl.com"));
            Assert.That(result.AdminImageFileUploadModel, Is.Not.Null);
            Assert.That(result.AdminChangePasswordViewModel, Is.Not.Null);
        }

        [Test]
        public async Task GetAdminInformation_InitializesViewModels()
        {
            var adminId = Guid.NewGuid();
            var mockUser = new User()
            {
                Id = adminId,
                FirstName = "Jane",
                LastName = "Doe",
                UserName = "janedoe",
                Position = "Admin",
                CompanyName = "Another Company",
                PhoneNumber = "987-654-3210",
                Email = "jane.doe@example.com"
            };

            _mockUserService.Setup(s => s.GetUserByIdAsync(adminId)).ReturnsAsync(mockUser);
            _mockUserService.Setup(s => s.GetUserImageUrlAsync(adminId)).ReturnsAsync("http://anotherimageurl.com");

            var result = await _profileService.GetAdminInformation(adminId);

            Assert.That(result.AdminImageFileUploadModel, Is.Not.Null);
            Assert.That(result.AdminChangePasswordViewModel, Is.Not.Null);
        }

        [Test]
        public async Task UpdateProfileInformation_AdminNotFound_ThrowsException()
        {
            var adminId = Guid.NewGuid();

            var model = new AdminProfileViewModel
            {
                Id = adminId.ToString()
            };

            _mockAdminUserService.Setup(s => s.GetAdminByIdAsync(adminId)).ReturnsAsync((User?)null);

            Assert.That(async () => await _profileService.UpdateProfileInformation(model, adminId),
                Throws.TypeOf<Exception>().With.Message.EqualTo(UserNotFound));
        }

        [Test]
        public async Task UpdateProfileInformation_NoChangesMade_ThrowsException()
        {
            var adminId = Guid.NewGuid();
            var model = new AdminProfileViewModel
            {
                Id = adminId.ToString(),
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                CompanyName = "Test Company",
                Position = "Admin",
                PhoneNumber = "123-456-7890",
                Email = "john.doe@example.com",

            };

            var mockAdmin = new User
            {
                Id = adminId,
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Position = "Admin",
                CompanyName = "Test Company",
                PhoneNumber = "123-456-7890",
                Email = "john.doe@example.com",
            };

            _mockHtmlSanitizer.Setup(s => s.Sanitize(It.IsAny<string>()))
                .Returns<string>(input => input);

            _mockAdminUserService.Setup(s => s.GetAdminByIdAsync(adminId)).ReturnsAsync(mockAdmin);


            Assert.That(async () => await _profileService.UpdateProfileInformation(model, adminId),
                Throws.TypeOf<Exception>().With.Message.EqualTo(NoChangesMadeToProfile));
        }

        [Test]
        public async Task UpdateProfileInformation_EmailAlreadyTaken_ThrowsException()
        {
            var adminId = Guid.NewGuid();

            var model = new AdminProfileViewModel
            {
                Id = adminId.ToString(),
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Position = "Admin",
                CompanyName = "Test Company",
                PhoneNumber = "123-456-7890",
                Email = "taken@example.com"
            };

            var mockAdmin = new User
            {
                Id = adminId,
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Position = "Admin",
                CompanyName = "Test Company",
                PhoneNumber = "123-456-7890",
                Email = "different@example.com"
            };

            _mockHtmlSanitizer.Setup(s => s.Sanitize(It.IsAny<string>()))
                .Returns<string>(input => input);

            _mockAdminUserService.Setup(s => s.GetAdminByIdAsync(adminId)).ReturnsAsync(mockAdmin);

            _mockProfileHelperService.Setup(s => s.IsEmailTakenAsync("taken@example.com", adminId)).ReturnsAsync(true);

            var exception = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _profileService.UpdateProfileInformation(model, adminId)
            );

            Assert.That(exception.Message, Is.EqualTo(EmailIsAlreadyTaken));
        }

        [Test]
        public async Task UpdateProfileInformation_UsernameAlreadyTaken_ThrowsException()
        {
            // Arrange
            var adminId = Guid.NewGuid();

            var model = new AdminProfileViewModel
            {
                Id = adminId.ToString(),
                FirstName = "Bruce",
                LastName = "Wayne",
                Username = "takenUsername",
                CompanyName = "Wayne Enterprises",
                Position = "Admin",
                PhoneNumber = "123-456-7890",
                Email = "bruce.wayne@example.com"
            };

            var mockAdmin = new User
            {
                Id = adminId,
                FirstName = "Chuck",
                LastName = "Norris",
                UserName = "adminUsername",
                CompanyName = "Roundhouse Kick Inc.",
                Position = "Admin",
                PhoneNumber = "987-654-3210",
                Email = "chuck.norris@example.com"
            };

            _mockHtmlSanitizer.Setup(s => s.Sanitize(It.IsAny<string>())).Returns<string>(input => input);

            _mockAdminUserService.Setup(s => s.GetAdminByIdAsync(adminId)).ReturnsAsync(mockAdmin);

            _mockProfileHelperService.Setup(s => s.IsUsernameTakenAsync("takenUsername", adminId)).ReturnsAsync(true);

            var exception = Assert.ThrowsAsync<InvalidDataException>(
                async () => await _profileService.UpdateProfileInformation(model, adminId)
            );

            Assert.That(exception.Message, Is.EqualTo(UserNameIsAlreadyTaken));
        }

        [Test]
        public async Task UpdateProfileInformation_SuccessfulUpdate_ReturnsUpdatedProfile()
        {
            var adminId = Guid.NewGuid();
            var model = new AdminProfileViewModel
            {
                Id = adminId.ToString(),
                FirstName = "Jane",
                LastName = "Doe",
                Username = "janedoe",
                Email = "jane.doe@example.com",
                CompanyName = "New Company",
                Position = "Admin",
                PhoneNumber = "9876543210"
            };

            var mockAdmin = new User
            {
                Id = adminId,
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john.doe@example.com",
                CompanyName = "Company",
                Position = "Admin",
                PhoneNumber = "1234567890"
            };

            _mockHtmlSanitizer.Setup(s => s.Sanitize(It.IsAny<string>())).Returns<string>(input => input);
            _mockAdminUserService.Setup(s => s.GetAdminByIdAsync(adminId)).ReturnsAsync(mockAdmin);
            _mockProfileHelperService.Setup(s => s.IsEmailTakenAsync(It.IsAny<string>(), adminId)).ReturnsAsync(false);
            _mockProfileHelperService.Setup(s => s.IsUsernameTakenAsync(It.IsAny<string>(), adminId))
                .ReturnsAsync(false);
            _mockUserService.Setup(s => s.GetUserImageUrlAsync(adminId)).ReturnsAsync("http://newimageurl.com");

            _mockAdminUserService
                .Setup(s => s.GetAdminClaimsAsync(It.IsAny<BaseUser>()))
                .ReturnsAsync(new List<Claim> { new Claim("Position", "Admin") });

            _mockProfileHelperService
                .Setup(s => s.GetMissingClaims(It.IsAny<IEnumerable<Claim>>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Claim> { new Claim("FirstName", "Jane"), new Claim("LastName", "Doe") });

            _mockUserManager
                .Setup(m => m.RemoveClaimsAsync(It.IsAny<BaseUser>(), It.IsAny<IEnumerable<Claim>>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(m => m.AddClaimsAsync(It.IsAny<BaseUser>(), It.IsAny<IEnumerable<Claim>>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(m => m.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success); // Simulate a successful update

            var result = await _profileService.UpdateProfileInformation(model, adminId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FirstName, Is.EqualTo("Jane"));
            Assert.That(result.LastName, Is.EqualTo("Doe"));
            Assert.That(result.Username, Is.EqualTo("janedoe"));
            Assert.That(result.Email, Is.EqualTo("jane.doe@example.com"));
            Assert.That(result.CompanyName, Is.EqualTo("New Company"));
            Assert.That(result.PhoneNumber, Is.EqualTo("9876543210"));
            Assert.That(result.UserImageUrl, Is.EqualTo("http://newimageurl.com"));

            _mockUserManager.Verify(m => m.RemoveClaimsAsync(It.IsAny<BaseUser>(), It.IsAny<IEnumerable<Claim>>()),
                Times.Once);
            _mockUserManager.Verify(m => m.AddClaimsAsync(It.IsAny<BaseUser>(), It.IsAny<IEnumerable<Claim>>()),
                Times.Once);
        }

        [Test]
        public async Task UpdateProfileInformation_IdOrPositionTampered_ReturnsSameModel()
        {
            var adminId = Guid.NewGuid();
            var tamperedId = Guid.NewGuid();

            var model = new AdminProfileViewModel
            {
                Id = tamperedId.ToString(),
                Position = "Admin"
            };

            var mockAdmin = new User
            {
                Id = adminId,
                Position = "Dispatcher"
            };

            _mockAdminUserService.Setup(s => s.GetAdminByIdAsync(adminId)).ReturnsAsync(mockAdmin);

            var result = await _profileService.UpdateProfileInformation(model, adminId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(model.Id));
            Assert.That(result.Position, Is.EqualTo(model.Position));
            Assert.That(result, Is.EqualTo(model));
        }

        [Test]
        public void ChangePasswordAsync_AdminIsNull_ThrowsArgumentNullException()
        {
            string currentPassword = "currentPassword";
            string newPassword = "newPassword";

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _profileService.ChangePasswordAsync(null, currentPassword, newPassword));
        }

        [Test]
        public async Task ChangePasswordAsync_CurrentAndNewPasswordsMatch_ReturnsFailedResult()
        {
            var admin = new Administrator { UserName = "admin" };

            string currentPassword = "password123";
            string newPassword = "password123";

            _mockHtmlSanitizer
                .Setup(s => s.Sanitize(It.IsAny<string>()))
                .Returns<string>(input => input);

            var result = await _profileService.ChangePasswordAsync(admin, currentPassword, newPassword);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors.First().Description, Is.EqualTo(CurrentAndNewPasswordCannotMatch));
        }

        [Test]
        public async Task ChangePasswordAsync_SuccessfulChange_LogsUserOutAndBackIn()
        {

            var admin = new Administrator { UserName = "admin" };

            string currentPassword = "currentPassword";
            string newPassword = "newPassword";

            _mockHtmlSanitizer
                .Setup(s => s.Sanitize(It.IsAny<string>()))
                .Returns<string>(input => input);

            _mockUserManager
                .Setup(m => m.ChangePasswordAsync(admin, currentPassword, newPassword))
                .ReturnsAsync(IdentityResult.Success);

            _mockSignInManager
                .Setup(s => s.SignOutAsync())
                .Returns(Task.CompletedTask);

            _mockSignInManager
                .Setup(s => s.PasswordSignInAsync(admin, newPassword, false, false))
                .ReturnsAsync(SignInResult.Success);

            var result = await _profileService.ChangePasswordAsync(admin, currentPassword, newPassword);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Succeeded, Is.True);

            _mockSignInManager.Verify(s => s.SignOutAsync(), Times.Once);
            _mockSignInManager.Verify(s => s.PasswordSignInAsync(admin, newPassword, false, false), Times.Once);
        }

        [Test]
        public async Task ChangePasswordAsync_PasswordChangeFails_ReturnsFailedResult()
        {
            var admin = new Administrator { UserName = "admin" };
            string currentPassword = "currentPassword";
            string newPassword = "newPassword";

            _mockHtmlSanitizer
                .Setup(s => s.Sanitize(It.IsAny<string>()))
                .Returns<string>(input => input);

            _mockUserManager
                .Setup(m => m.ChangePasswordAsync(admin, currentPassword, newPassword))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password change failed" }));

            var result = await _profileService.ChangePasswordAsync(admin, currentPassword, newPassword);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors.First().Description, Is.EqualTo("Password change failed"));

            _mockSignInManager.Verify(s => s.SignOutAsync(), Times.Never);
            _mockSignInManager.Verify(s => s.PasswordSignInAsync(It.IsAny<Administrator>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public async Task ChangePasswordAsync_EnsuresPasswordIsSanitized()
        {
            var admin = new Administrator { UserName = "admin" };
            string currentPassword = "<b>currentPassword</b>";
            string newPassword = "<script>alert('newPassword')</script>";
            string sanitizedCurrentPassword = "currentPassword";
            string sanitizedNewPassword = "newPassword";

            _mockHtmlSanitizer
                .Setup(s => s.Sanitize(currentPassword))
                .Returns(sanitizedCurrentPassword);
            _mockHtmlSanitizer
                .Setup(s => s.Sanitize(newPassword))
                .Returns(sanitizedNewPassword);

            _mockUserManager
                .Setup(m => m.ChangePasswordAsync(admin, sanitizedCurrentPassword, sanitizedNewPassword))
                .ReturnsAsync(IdentityResult.Success);

            _mockSignInManager
                .Setup(s => s.SignOutAsync())
                .Returns(Task.CompletedTask);

            _mockSignInManager
                .Setup(s => s.PasswordSignInAsync(admin, sanitizedNewPassword, false, false))
                .ReturnsAsync(SignInResult.Success);

            var result = await _profileService.ChangePasswordAsync(admin, currentPassword, newPassword);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Succeeded, Is.True);

            _mockUserManager.Verify(m => m.ChangePasswordAsync(admin, sanitizedCurrentPassword, sanitizedNewPassword), Times.Once);
        }



    }
}
