using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;
using NUnit.Framework;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Account;
using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data.Models;

using static LoadVantage.Common.ValidationConstants;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;



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

        private (Mock<SignInManager<BaseUser>> SignInManager, Mock<RoleManager<Role>> RoleManager,
	        Mock<IHtmlSanitizerService> HtmlSanitizer, Mock<IUserService> UserService,
	        Mock<IProfileHelperService> ProfileHelperService) CreateMockSetup()
        {
	        var mockUserStore = new Mock<IUserStore<BaseUser>>();
	        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
	        var mockUserClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<BaseUser>>();
	        var mockIdentityOptions = Options.Create(new IdentityOptions());
	        var mockLogger = new Mock<ILogger<SignInManager<BaseUser>>>();
	        var mockAuthenticationSchemeProvider = new Mock<IAuthenticationSchemeProvider>();
	        var mockUserConfirmation = new Mock<IUserConfirmation<BaseUser>>();

	        var mockUserManager = new Mock<UserManager<BaseUser>>(
		        mockUserStore.Object,
		        null,
		        null,
		        null,
		        null,
		        null,
		        null,
		        null,
		        null
	        );

	        var mockSignInManager = new Mock<SignInManager<BaseUser>>(
		        mockUserManager.Object,
		        mockHttpContextAccessor.Object,
		        mockUserClaimsPrincipalFactory.Object,
		        mockIdentityOptions,
		        mockLogger.Object,
		        mockAuthenticationSchemeProvider.Object,
		        mockUserConfirmation.Object
	        );

	        var mockRoleManager = new Mock<RoleManager<Role>>(
		        new Mock<IRoleStore<Role>>().Object,
		        null,
		        null,
		        null,
		        null
	        );

	        var mockHtmlSanitizer = new Mock<IHtmlSanitizerService>();
	        var mockUserService = new Mock<IUserService>();
	        var mockProfileHelperService = new Mock<IProfileHelperService>();

	        return (mockSignInManager, mockRoleManager, mockHtmlSanitizer, mockUserService, mockProfileHelperService);
        }

		[Test]
		public async Task RegisterUserAsync_WithValidDetails_ReturnsSuccess()
		{
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

			var mockUserStore = new Mock<IUserStore<BaseUser>>();
			var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
			var mockUserClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<BaseUser>>();
			var mockIdentityOptions = Options.Create(new IdentityOptions());
			var mockLogger = new Mock<ILogger<SignInManager<BaseUser>>>();
			var mockAuthenticationSchemeProvider = new Mock<IAuthenticationSchemeProvider>();
			var mockUserConfirmation = new Mock<IUserConfirmation<BaseUser>>();

			var mockUserManager = new Mock<UserManager<BaseUser>>(
				mockUserStore.Object,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);

			var mockSignInManager = new Mock<SignInManager<BaseUser>>(
				mockUserManager.Object,
				mockHttpContextAccessor.Object,
				mockUserClaimsPrincipalFactory.Object,
				mockIdentityOptions,
				mockLogger.Object,
				mockAuthenticationSchemeProvider.Object,
				mockUserConfirmation.Object
			);

			var mockRoleManager = new Mock<RoleManager<Role>>(
				new Mock<IRoleStore<Role>>().Object,
				null,
				null,
				null,
				null
			);

			var mockHtmlSanitizer = new Mock<IHtmlSanitizerService>();
			var mockUserService = new Mock<IUserService>();
			var mockProfileHelperService = new Mock<IProfileHelperService>();

			mockUserService.Setup(u => u.FindUserByEmailAsync(model.Email))
				.ReturnsAsync((User?)null); 

			mockUserService.Setup(u => u.FindUserByUsernameAsync(model.UserName))
				.ReturnsAsync((User?)null); 

			mockUserService.Setup(u => u.GetOrCreateDefaultImageAsync())
				.ReturnsAsync(Guid.NewGuid()); 

			mockUserService.Setup(u => u.CreateUserAsync(It.IsAny<User>(), model.Password))
				.ReturnsAsync(IdentityResult.Success); 

			mockUserManager.Setup(m => m.AddClaimAsync(It.IsAny<BaseUser>(), It.IsAny<Claim>()))
				.ReturnsAsync(IdentityResult.Success);
			mockUserService.Setup(u => u.AddUserClaimAsync(It.IsAny<BaseUser>(), It.IsAny<Claim>()))
				.Returns((BaseUser user, Claim claim) => mockUserManager.Object.AddClaimAsync(user, claim)); 

			mockRoleManager.Setup(r => r.FindByNameAsync(model.Role))
				.ReturnsAsync(new Role { Name = model.Role }); 

			mockHtmlSanitizer.Setup(h => h.Sanitize(It.IsAny<string>()))
				.Returns((string input) => input); 

			var accountService = new AccountService(
				mockSignInManager.Object,
				mockRoleManager.Object,
				mockHtmlSanitizer.Object,
				mockUserService.Object,
				mockProfileHelperService.Object
			);

			var result = await accountService.RegisterUserAsync(model);

			Assert.That(result.Result.Succeeded, Is.True);
			Assert.That(result.Result.Errors, Is.Empty);
			Assert.That(result.Error, Is.Null);

			mockUserService.Verify(u => u.FindUserByEmailAsync(model.Email), Times.Once);
			mockUserService.Verify(u => u.FindUserByUsernameAsync(model.UserName), Times.Once);
			mockUserService.Verify(u => u.GetOrCreateDefaultImageAsync(), Times.Once);
			mockUserService.Verify(u => u.CreateUserAsync(It.Is<User>(user =>
				user.Email == model.Email &&
				user.UserName == model.UserName
			), model.Password), Times.Once);
			mockUserService.Verify(u => u.AddUserClaimAsync(It.IsAny<BaseUser>(), It.IsAny<Claim>()), Times.Exactly(4));
			mockRoleManager.Verify(r => r.FindByNameAsync(model.Role), Times.Once);
		}

		[Test]
		public async Task RegisterUserAsync_WithInvalidRole_ReturnsIdentityError()
		{
			var model = new RegisterViewModel
			{
				Position = "Dispatcher",
				Email = "dispatcher1@gmail.com",
				UserName = "dispatcher1",
				Password = "Password123",
				FirstName = "The Rock",
				LastName = "Johnson",
				Company = "Can you smell what the rock is cooking.",
				Role = "Administrator" 
			};

			var mockUserStore = new Mock<IUserStore<BaseUser>>();
			var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
			var mockUserClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<BaseUser>>();
			var mockIdentityOptions = Options.Create(new IdentityOptions());
			var mockLogger = new Mock<ILogger<SignInManager<BaseUser>>>();
			var mockAuthenticationSchemeProvider = new Mock<IAuthenticationSchemeProvider>();
			var mockUserConfirmation = new Mock<IUserConfirmation<BaseUser>>();

			var mockUserManager = new Mock<UserManager<BaseUser>>(
				mockUserStore.Object,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);

			var mockSignInManager = new Mock<SignInManager<BaseUser>>(
				mockUserManager.Object,
				mockHttpContextAccessor.Object,
				mockUserClaimsPrincipalFactory.Object,
				mockIdentityOptions,
				mockLogger.Object,
				mockAuthenticationSchemeProvider.Object,
				mockUserConfirmation.Object
			);

			var mockRoleManager = new Mock<RoleManager<Role>>(
				new Mock<IRoleStore<Role>>().Object,
				null,
				null,
				null,
				null
			);

			var mockHtmlSanitizer = new Mock<IHtmlSanitizerService>();
			var mockUserService = new Mock<IUserService>();
			var mockProfileHelperService = new Mock<IProfileHelperService>();

			mockUserService.Setup(u => u.FindUserByEmailAsync(model.Email))
				.ReturnsAsync((User?)null);

			mockUserService.Setup(u => u.FindUserByUsernameAsync(model.UserName))
				.ReturnsAsync((User?)null);

			mockUserService.Setup(u => u.GetOrCreateDefaultImageAsync())
				.ReturnsAsync(Guid.NewGuid());

			mockRoleManager.Setup(r => r.FindByNameAsync(model.Role))
				.ReturnsAsync((Role?)null); 

			mockHtmlSanitizer.Setup(h => h.Sanitize(It.IsAny<string>()))
				.Returns((string input) => input);

			var accountService = new AccountService(
				mockSignInManager.Object,
				mockRoleManager.Object,
				mockHtmlSanitizer.Object,
				mockUserService.Object,
				mockProfileHelperService.Object
			);

			var result = await accountService.RegisterUserAsync(model);

			Assert.That(result.Result.Succeeded, Is.False);
			Assert.That(result.Result.Errors, Is.Not.Empty);
			Assert.That(result.Error, Is.EqualTo("Role"));
			Assert.That(result.Result.Errors.First().Description, Is.EqualTo(InvalidRoleSelected));

			mockUserService.Verify(u => u.FindUserByEmailAsync(model.Email), Times.Once);
			mockUserService.Verify(u => u.FindUserByUsernameAsync(model.UserName), Times.Once);
			mockRoleManager.Verify(r => r.FindByNameAsync(model.Role), Times.Once);
		}

		[Test]
		public async Task RegisterUserAsync_WithInvalidPosition_ReturnsIdentityError()
		{
			var model = new RegisterViewModel
			{
				Position = "InvalidPosition", 
				Email = "dispatcher1@gmail.com",
				UserName = "dispatcher1",
				Password = "Password123",
				FirstName = "The Rock",
				LastName = "Johnson",
				Company = "Can you smell what the rock is cooking.",
				Role = "Dispatcher"
			};

			var mockSetup = CreateMockSetup(); 
			var accountService = new AccountService(
				mockSetup.SignInManager.Object,
				mockSetup.RoleManager.Object,
				mockSetup.HtmlSanitizer.Object,
				mockSetup.UserService.Object,
				mockSetup.ProfileHelperService.Object
			);

			var result = await accountService.RegisterUserAsync(model);

			Assert.That(result.Result.Succeeded, Is.False);
			Assert.That(result.Result.Errors, Is.Not.Empty);
			Assert.That(result.Error, Is.EqualTo("Position"));
			Assert.That(result.Result.Errors.First().Description, Is.EqualTo(InvalidPositionSelected));
		}

		[Test]
		public async Task RegisterUserAsync_WithExistingEmail_ReturnsIdentityError()
		{
			var model = new RegisterViewModel
			{
				Position = "Dispatcher",
				Email = "dispatcher1@gmail.com", 
				UserName = "dispatcher1",
				Password = "Password123",
				FirstName = "The Rock",
				LastName = "Johnson",
				Company = "Can you smell what the rock is cooking.",
				Role = "Dispatcher"
			};

			var mockSetup = CreateMockSetup();
			mockSetup.UserService.Setup(u => u.FindUserByEmailAsync(model.Email))
				.ReturnsAsync(new User { Email = model.Email }); 

			var accountService = new AccountService(
				mockSetup.SignInManager.Object,
				mockSetup.RoleManager.Object,
				mockSetup.HtmlSanitizer.Object,
				mockSetup.UserService.Object,
				mockSetup.ProfileHelperService.Object
			);

			var result = await accountService.RegisterUserAsync(model);

			Assert.That(result.Result.Succeeded, Is.False);
			Assert.That(result.Result.Errors, Is.Not.Empty);
			Assert.That(result.Error, Is.EqualTo("Email"));
			Assert.That(result.Result.Errors.First().Description, Is.EqualTo(EmailAlreadyExists));
			mockSetup.UserService.Verify(u => u.FindUserByEmailAsync(model.Email), Times.Once);
		}

		[Test]
		public async Task RegisterUserAsync_WithExistingUsername_ReturnsIdentityError()
		{
			var model = new RegisterViewModel
			{
				Position = "Dispatcher",
				Email = "dispatcher1@gmail.com",
				UserName = "dispatcher1",
				Password = "Password123",
				FirstName = "The Rock",
				LastName = "Johnson",
				Company = "Can you smell what the rock is cooking.",
				Role = "Dispatcher"
			};

			var mockSetup = CreateMockSetup();

			mockSetup.UserService.Setup(u => u.FindUserByUsernameAsync(model.UserName))
				.ReturnsAsync(new User { UserName = model.UserName }); 

			var accountService = new AccountService(
				mockSetup.SignInManager.Object,
				mockSetup.RoleManager.Object,
				mockSetup.HtmlSanitizer.Object,
				mockSetup.UserService.Object,
				mockSetup.ProfileHelperService.Object
			);

			var result = await accountService.RegisterUserAsync(model);

			Assert.That(result.Result.Succeeded, Is.False);
			Assert.That(result.Result.Errors, Is.Not.Empty);
			Assert.That(result.Error, Is.EqualTo("Username"));
			Assert.That(result.Result.Errors.First().Description, Is.EqualTo(UserNameAlreadyExists));
			mockSetup.UserService.Verify(u => u.FindUserByUsernameAsync(model.UserName), Times.Once);
		}
		[Test]
		public async Task RegisterUserAsync_CreateUserFails_ReturnsIdentityResult()
		{
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

			var mockUserStore = new Mock<IUserStore<BaseUser>>();
			var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
			var mockUserClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<BaseUser>>();
			var mockIdentityOptions = Options.Create(new IdentityOptions());
			var mockLogger = new Mock<ILogger<SignInManager<BaseUser>>>();
			var mockAuthenticationSchemeProvider = new Mock<IAuthenticationSchemeProvider>();
			var mockUserConfirmation = new Mock<IUserConfirmation<BaseUser>>();

			var mockUserManager = new Mock<UserManager<BaseUser>>(
				mockUserStore.Object,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);

			var mockSignInManager = new Mock<SignInManager<BaseUser>>(
				mockUserManager.Object,
				mockHttpContextAccessor.Object,
				mockUserClaimsPrincipalFactory.Object,
				mockIdentityOptions,
				mockLogger.Object,
				mockAuthenticationSchemeProvider.Object,
				mockUserConfirmation.Object
			);

			var mockRoleManager = new Mock<RoleManager<Role>>(
				new Mock<IRoleStore<Role>>().Object,
				null,
				null,
				null,
				null
			);

			var mockHtmlSanitizer = new Mock<IHtmlSanitizerService>();
			var mockUserService = new Mock<IUserService>();
			var mockProfileHelperService = new Mock<IProfileHelperService>();

			mockUserService.Setup(u => u.FindUserByEmailAsync(model.Email))
				.ReturnsAsync((User?)null);

			mockUserService.Setup(u => u.FindUserByUsernameAsync(model.UserName))
				.ReturnsAsync((User?)null);

			mockUserService.Setup(u => u.GetOrCreateDefaultImageAsync())
				.ReturnsAsync(Guid.NewGuid());

			mockUserService.Setup(u => u.CreateUserAsync(It.IsAny<User>(), model.Password))
				.ReturnsAsync(IdentityResult.Success);

			mockUserManager.Setup(m => m.AddClaimAsync(It.IsAny<BaseUser>(), It.IsAny<Claim>()))
				.ReturnsAsync(IdentityResult.Success);
			mockUserService.Setup(u => u.AddUserClaimAsync(It.IsAny<BaseUser>(), It.IsAny<Claim>()))
				.Returns((BaseUser user, Claim claim) => mockUserManager.Object.AddClaimAsync(user, claim));

			mockRoleManager.Setup(r => r.FindByNameAsync(model.Role))
				.ReturnsAsync(new Role { Name = model.Role });

			mockHtmlSanitizer.Setup(h => h.Sanitize(It.IsAny<string>()))
				.Returns((string input) => input);

			mockUserService.Setup(u => u.CreateUserAsync(It.IsAny<User>(), model.Password))
				.ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = UserCreationFailed }));

			var accountService = new AccountService(
				mockSignInManager.Object,
				mockRoleManager.Object,
				mockHtmlSanitizer.Object,
				mockUserService.Object,
				mockProfileHelperService.Object
			);


			var result = await accountService.RegisterUserAsync(model);

			Assert.That(result.Result.Succeeded, Is.False);
			Assert.That(result.Result.Errors, Is.Not.Empty);
			Assert.That(result.Result.Errors.First().Description, Is.EqualTo(UserCreationFailed));

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