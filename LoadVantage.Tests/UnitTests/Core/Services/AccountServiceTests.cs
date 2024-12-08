using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Account;
using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data.Models;

using Moq;
using NUnit.Framework;

using static LoadVantage.Common.ValidationConstants;

namespace LoadVantage.Tests.UnitTests.Core.Services
{
    public class AccountServiceTests
    {
        private Mock<RoleManager<Role>> _roleManager;
        private Mock<IHtmlSanitizerService> _htmlSanitizerService;
        private Mock<IUserService> _userService;
        private Mock<IProfileHelperService> _profileHelperService;
        private Mock<SignInManager<BaseUser>> _signInManager;

        private AccountService _accountService;

        [SetUp]
        public void SetUp()
        {

            var _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var _mockClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<BaseUser>>();
            var mockUserStore = new Mock<IUserStore<BaseUser>>();

            var _mockUserManager = new Mock<UserManager<BaseUser>>(
                mockUserStore.Object,
                null, // OptionsAccessor
                null, // PasswordHasher
                null, // UserValidators
                null, // PasswordValidators
                null, // KeyNormalizer
                null, // ErrorsDescriber
                null, // Services
                null  // Logger
            );

            _signInManager = new Mock<SignInManager<BaseUser>>(
                _mockUserManager.Object,
                _mockHttpContextAccessor.Object,
                _mockClaimsPrincipalFactory.Object,
                null, // OptionsAccessor
                null, // Logger
                null, // UrlEncoder
                null  // SystemClock
            );

            _roleManager = new Mock<RoleManager<Role>>(
                new Mock<IRoleStore<Role>>().Object,
                null, null, null, null
            );

            _htmlSanitizerService = new Mock<IHtmlSanitizerService>();

            _userService = new Mock<IUserService>();

            _profileHelperService = new Mock<IProfileHelperService>();

            _accountService = new AccountService(
                _signInManager.Object,
                _roleManager.Object,
                _htmlSanitizerService.Object,
                _userService.Object,
                _profileHelperService.Object
            );
        }

        [Test]
        public async Task RegisterUserAsync_InvalidPosition_ReturnsIdentityResultWithError()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Position = "InvalidPosition",
                Email = "test@example.com",
                UserName = "testuser",
                Password = "Password123",
                FirstName = "Test",
                LastName = "User",
                Company = "TestCompany",
                Role = "User"
            };

            var mockAccountService = new Mock<IAccountService>();

            _htmlSanitizerService.Setup(s => s.Sanitize(It.IsAny<string>())).Returns<string>(s => s);
            mockAccountService
                .Setup(u => u.RegisterUserAsync(It.IsAny<RegisterViewModel>()))
                .ReturnsAsync((IdentityResult.Failed(new IdentityError { Description = "Invalid Position Selected" }), "Position"));

            var result = await mockAccountService.Object.RegisterUserAsync(model);


            Assert.That(result.Result.Succeeded, Is.False);
            Assert.That(result.Result.Errors, Has.Some.Matches<IdentityError>(e => e.Description == "Invalid Position Selected"));
            Assert.That(result.Item2, Is.EqualTo("Position"));
        }

        [Test]
        public async Task RegisterUserAsync_EmailAlreadyExists_ReturnsError()
        {
            var model = new RegisterViewModel
            {
                Email = "existing@email.com"
            };

            var mockUserService = new Mock<IUserService>();
            var mockAccountService = new Mock<IAccountService>();

            mockUserService.Setup(u => u.FindUserByEmailAsync(model.Email)).ReturnsAsync(new User());

            mockAccountService
                .Setup(a => a.RegisterUserAsync(It.IsAny<RegisterViewModel>()))
                .ReturnsAsync((IdentityResult.Failed(new IdentityError { Description = EmailAlreadyExists }), "Email"));

            var result = await mockAccountService.Object.RegisterUserAsync(model);

            Assert.That(result.Result.Succeeded, Is.False);
            Assert.That(result.Result.Errors, Has.Some.Matches<IdentityError>(e => e.Description == EmailAlreadyExists));
            Assert.That(result.Error, Is.EqualTo("Email"));
        }

        [Test]
        public async Task RegisterUserAsync_UsernameAlreadyExists_ReturnsError()
        {
            var model = new RegisterViewModel
            {
                UserName = "existingUser"
            };

            var mockUserService = new Mock<IUserService>();
            var mockAccountService = new Mock<IAccountService>();

            mockUserService.Setup(u => u.FindUserByUsernameAsync(model.UserName)).ReturnsAsync(new User());

            mockAccountService
                .Setup(a => a.RegisterUserAsync(It.IsAny<RegisterViewModel>()))
                .ReturnsAsync((IdentityResult.Failed(new IdentityError { Description = UserNameAlreadyExists }), "Username"));

            var result = await mockAccountService.Object.RegisterUserAsync(model);

            Assert.That(result.Result.Succeeded, Is.False);
            Assert.That(result.Result.Errors, Has.Some.Matches<IdentityError>(e => e.Description == UserNameAlreadyExists));
            Assert.That(result.Error, Is.EqualTo("Username"));
        }

        [Test]
        public async Task RegisterUserAsync_InvalidRole_ReturnsError()
        {
            var model = new RegisterViewModel
            {
                Role = "InvalidRole"
            };

            var mockRoleManager = new Mock<RoleManager<Role>>();
            var mockAccountService = new Mock<IAccountService>();

            mockRoleManager.Setup(r => r.FindByNameAsync(model.Role)).ReturnsAsync((Role?)null);

            mockAccountService
                .Setup(a => a.RegisterUserAsync(It.IsAny<RegisterViewModel>()))
                .ReturnsAsync((IdentityResult.Failed(new IdentityError { Description = InvalidRoleSelected }), "Role"));

            var result = await mockAccountService.Object.RegisterUserAsync(model);

            Assert.That(result.Result.Succeeded, Is.False);
            Assert.That(result.Result.Errors, Has.Some.Matches<IdentityError>(e => e.Description == InvalidRoleSelected));
            Assert.That(result.Error, Is.EqualTo("Role"));
        }

        [Test]
        public async Task RegisterUserAsync_ValidDetails_ReturnsSuccess()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Position = "Dispatcher",
                Email = "dispatcher1@gmail.com",
                UserName = "dispatcher1",
                Password = "Password123",
                FirstName = "The Rock",
                LastName = "Johnson",
                Company = "Can you smell what the rock is cooking.",
                Role = "User"
            };

            var mockUserService = new Mock<IUserService>();
            var mockRoleManager = new Mock<RoleManager<Role>>();
            var mockAccountService = new Mock<IAccountService>();

            mockUserService.Setup(u => u.FindUserByEmailAsync(model.Email)).ReturnsAsync((User?)null);
            mockUserService.Setup(u => u.FindUserByUsernameAsync(model.UserName)).ReturnsAsync((User?)null);
            mockRoleManager.Setup(r => r.FindByNameAsync(model.Role)).ReturnsAsync(new Role { Name = "User" });
            mockUserService.Setup(u => u.CreateUserAsync(It.IsAny<User>(), model.Password)).ReturnsAsync(IdentityResult.Success);

            mockAccountService
                .Setup(a => a.RegisterUserAsync(It.IsAny<RegisterViewModel>()))
                .ReturnsAsync((IdentityResult.Success, null));

            var result = await mockAccountService.Object.RegisterUserAsync(model);

            Assert.That(result.Result.Succeeded, Is.True);
            Assert.That(result.Result.Errors, Is.Empty);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task LoginAsync_InvalidUsername_ReturnsFailedResult()
        {

            var username = "invalidUser";
            var password = "password123";

            _htmlSanitizerService.Setup(h => h.Sanitize(username)).Returns(username);
            _htmlSanitizerService.Setup(h => h.Sanitize(password)).Returns(password);
            _userService.Setup(u => u.FindUserByUsernameAsync(username)).ReturnsAsync((User?)null);


            var result = await _accountService.LoginAsync(username, password);

            Assert.That(result.result, Is.EqualTo(SignInResult.Failed));
            Assert.That(result.message, Is.EqualTo(InvalidUserNameOrPassword));
        }

        [Test]
        public async Task LoginAsync_UserLockedOut_ReturnsLockedOutResult()
        {
            var username = "lockedOutUser";
            var password = "password123";

            var user = new User
            {
                LockoutEnd = DateTime.UtcNow.AddMinutes(10),
                IsActive = true
            };

            _htmlSanitizerService.Setup(h => h.Sanitize(username)).Returns(username);
            _htmlSanitizerService.Setup(h => h.Sanitize(password)).Returns(password);
            _userService.Setup(u => u.FindUserByUsernameAsync(username)).ReturnsAsync(user);

            var result = await _accountService.LoginAsync(username, password);


            Assert.That(result.result, Is.EqualTo(SignInResult.LockedOut));
            Assert.That(result.message, Does.StartWith("Too many failed login attempts. You can try again after"));
        }

        [Test]
        public async Task LoginAsync_InactiveUser_ReturnsNotAllowedResult()
        {
            var username = "inactiveUser";
            var password = "password123";

            var user = new User
            {
                IsActive = false
            };

            _htmlSanitizerService.Setup(h => h.Sanitize(username)).Returns(username);
            _htmlSanitizerService.Setup(h => h.Sanitize(password)).Returns(password);
            _userService.Setup(u => u.FindUserByUsernameAsync(username)).ReturnsAsync(user);

            var result = await _accountService.LoginAsync(username, password);

            Assert.That(result.result, Is.EqualTo(SignInResult.NotAllowed));
            Assert.That(result.message, Is.EqualTo(ThisAccountIsInactive));
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccessAndHandlesClaims()
        {
            var username = "validUser";
            var password = "password123";

            var user = new User
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                FirstName = "Test",
                LastName = "User",
                UserName = username,
                Position = "Broker"
            };

            _htmlSanitizerService.Setup(h => h.Sanitize(username)).Returns(username);
            _htmlSanitizerService.Setup(h => h.Sanitize(password)).Returns(password);

            _userService.Setup(u => u.FindUserByUsernameAsync(username)).ReturnsAsync(user);

            _signInManager.Setup(s => s.PasswordSignInAsync(user, password, false, true))
                .ReturnsAsync(SignInResult.Success);

            _userService.Setup(u => u.AddUserClaimsAsync(user, It.IsAny<IEnumerable<Claim>>()))
                .ReturnsAsync(IdentityResult.Success);

            _profileHelperService.Setup(p => p.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());
            _profileHelperService.Setup(p => p.GetMissingClaims(It.IsAny<IEnumerable<Claim>>(), user.FirstName, user.LastName, user.UserName, user.Position))
                .Returns(new List<Claim>
                {
                    new Claim("FirstName", user.FirstName)
                });

            _accountService = new AccountService(
                _signInManager.Object,
                _roleManager.Object,
                _htmlSanitizerService.Object,
                _userService.Object,
                _profileHelperService.Object
            );

            var result = await _accountService.LoginAsync(username, password);

            Assert.That(result.result, Is.EqualTo(SignInResult.Success));
            Assert.That(result.message, Is.Null);

            _signInManager.Verify(s => s.PasswordSignInAsync(user, password, false, true), Times.Once);
            _profileHelperService.Verify(p => p.GetClaimsAsync(user), Times.Once);
            _profileHelperService.Verify(p => p.GetMissingClaims(It.IsAny<IEnumerable<Claim>>(), user.FirstName, user.LastName, user.UserName, user.Position), Times.Once);
            _userService.Verify(u => u.AddUserClaimsAsync(user, It.IsAny<IEnumerable<Claim>>()), Times.Once);
        }

        [Test]
        public async Task LoginAsync_InvalidPassword_ReturnsFailedResult()
        {
            var username = "validUser";
            var password = "invalidPassword";

            var user = new User
            {
                IsActive = true
            };

            _htmlSanitizerService.Setup(h => h.Sanitize(username)).Returns(username);
            _htmlSanitizerService.Setup(h => h.Sanitize(password)).Returns(password);
            _userService.Setup(u => u.FindUserByUsernameAsync(username)).ReturnsAsync(user);

            _signInManager.Setup(s => s.PasswordSignInAsync(user, password, false, true)).ReturnsAsync(SignInResult.Failed);

            _accountService = new AccountService(
                _signInManager.Object,
                _roleManager.Object,
                _htmlSanitizerService.Object,
                _userService.Object,
                _profileHelperService.Object
            );

            var result = await _accountService.LoginAsync(username, password);

            Assert.That(result.result, Is.EqualTo(SignInResult.Failed));
            Assert.That(result.message, Is.EqualTo(InvalidUserNameOrPassword));
        }

        [Test]
        public async Task LogOutAsync_CallsSignOutAsync()
        {
            await _accountService.LogOutAsync();
            _signInManager.Verify(s => s.SignOutAsync(), Times.Once);
        }

    }
}