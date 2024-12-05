using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

using Moq;
using NUnit.Framework;

using LoadVantage.Core.Services;
using LoadVantage.Core.Contracts;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data.Contracts;

using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using Microsoft.AspNetCore.Http;
using LoadVantage.Core.Models.Profile;


namespace LoadVantage.Tests.Core.Services
{
	public class ProfileServiceTests
	{
		private Mock<IUserService> _mockUserService;
		private Mock<IProfileHelperService> _mockProfileHelperService;
		private Mock<IHtmlSanitizerService> _mockHtmlSanitizerService;
		private Mock<UserManager<BaseUser>> _mockUserManager; 
		private Mock<SignInManager<BaseUser>> _mockSignInManager;
		private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
		private Mock<IUserClaimsPrincipalFactory<BaseUser>> _mockClaimsPrincipalFactory;



		private ProfileService _profileService;

		[SetUp]
		public void SetUp()
		{

			_mockUserService = new Mock<IUserService>();
			_mockProfileHelperService = new Mock<IProfileHelperService>();
			_mockHtmlSanitizerService = new Mock<IHtmlSanitizerService>();
			_mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
			_mockClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<BaseUser>>();

			var mockUserStore = new Mock<IUserStore<BaseUser>>();


			var testUser = new User
			{
				Id = Guid.NewGuid(),
				UserName = "batman",
				Email = "batman@"
			};


			_mockUserManager = new Mock<UserManager<BaseUser>>(
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

			_mockSignInManager = new Mock<SignInManager<BaseUser>>(
				_mockUserManager.Object,
				_mockHttpContextAccessor.Object,
				_mockClaimsPrincipalFactory.Object,
				null, // OptionsAccessor
				null, // Logger
				null, // UrlEncoder
				null  // SystemClock
			);

			_mockUserManager
				.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync(testUser);

			_mockUserManager
				.Setup(m => m.UpdateAsync(It.IsAny<BaseUser>()))
				.ReturnsAsync(IdentityResult.Success);

			_mockSignInManager
				.Setup(s => s.RefreshSignInAsync(It.IsAny<BaseUser>()))
				.Returns(Task.CompletedTask);

			_profileService = new ProfileService(
				_mockUserService.Object,
				_mockUserManager.Object,
				_mockSignInManager.Object,
				_mockProfileHelperService.Object,
				_mockHtmlSanitizerService.Object
			);

		}

		[Test]
		public async Task GetUserInformation_ShouldReturnProfileViewModel_WhenUserExists()
		{
			var userId = Guid.NewGuid();

			var mockUser = new User
			{
				Id = userId,
				FirstName = "Bruce",
				LastName = "Wayne",
				UserName = "batman",
				Position = "Dispatcher",
				CompanyName = "Wayne Enterprises",
				PhoneNumber = "+1-800-699-9656",
				Email = "batman@google.com"
			};

			_mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync(mockUser);

			_mockUserService.Setup(service => service.GetUserImageUrlAsync(userId)).ReturnsAsync("http://batman.com/image.jpg");

			var result = await _profileService.GetUserInformation(userId);

			Assert.That(result, Is.Not.Null);
			Assert.That(result?.Id, Is.EqualTo(userId.ToString()));
			Assert.That(result?.FirstName, Is.EqualTo("Bruce"));
			Assert.That(result?.LastName, Is.EqualTo("Wayne"));
			Assert.That(result?.Username, Is.EqualTo("batman"));
			Assert.That(result?.Position, Is.EqualTo("Dispatcher"));
			Assert.That(result?.CompanyName, Is.EqualTo("Wayne Enterprises"));
			Assert.That(result?.PhoneNumber, Is.EqualTo("+1-800-699-9656"));
			Assert.That(result?.Email, Is.EqualTo("batman@google.com"));
			Assert.That(result?.UserImageUrl, Is.EqualTo("http://batman.com/image.jpg"));
		}

		[Test]
		public void GetUserInformation_ShouldThrowException_WhenUserNotFound()
		{
			var userId = Guid.NewGuid(); 

			_mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

			Assert.ThrowsAsync<Exception>(async () => await _profileService.GetUserInformation(userId), UserNotFound);
		}

		[Test]
		public async Task UpdateProfileInformation_ShouldReturnUpdatedProfile_WhenValidDataIsProvided()
		{
			var userId = Guid.NewGuid();

			var testUser = new User
			{
				Id = Guid.NewGuid(),
				UserName = "Gosho",
				Email = "goshko@softuni.bg",
				FirstName = "Gosho",
				LastName = "Geshev",
				Position = "Dispatcher",
				CompanyName = "SoftUni",
				PhoneNumber = "1234567890"
			};

			var model = new ProfileViewModel
			{
				Id = userId.ToString(),
				Username = "UpdatedUser",
				Email = "updateduser@example.com",
				FirstName = "UpdatedFirstName",
				LastName = "UpdatedLastName",
				CompanyName = "UpdatedCompany",
				Position = "Dispatcher",
				PhoneNumber = "0987654321"
			};

			_mockUserService.Setup(u => u.GetUserByIdAsync(It.Is<Guid>(id => id == userId)))
				.ReturnsAsync(testUser);

			_mockHtmlSanitizerService.Setup(s => s.Sanitize(It.IsAny<string>())).Returns<string>(x => x);

			_mockProfileHelperService.Setup(s => s.IsEmailTakenAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
			_mockProfileHelperService.Setup(s => s.IsUsernameTakenAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);

			_mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

			_mockUserService.Setup(s => s.GetUserImageUrlAsync(It.IsAny<Guid>()))
				.ReturnsAsync("https://gosho.com/gosho.jpg");

		
			var result = await _profileService.UpdateProfileInformation(model, userId);

		
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Username, Is.EqualTo(model.Username));
			Assert.That(result.Email, Is.EqualTo(model.Email));
			Assert.That(result.FirstName, Is.EqualTo(model.FirstName));
			Assert.That(result.LastName, Is.EqualTo(model.LastName));
			Assert.That(result.CompanyName, Is.EqualTo(model.CompanyName));
			Assert.That(result.PhoneNumber, Is.EqualTo(model.PhoneNumber));
		}

		[Test]
		public async Task UpdateProfileInformation_ShouldThrowException_WhenEmailIsTaken()
		{
			var testUser = new User
			{
				Id = Guid.NewGuid(),
				UserName = "Gosho",
				Email = "goshko@softuni.bg",
				FirstName = "Gosho",
				LastName = "Geshev",
				Position = "Dispatcher",
				CompanyName = "SoftUni",
				PhoneNumber = "1234567890"
			};

			var model = new ProfileViewModel
			{
				Id = testUser.Id.ToString(),
				Username = "UpdatedUser",
				Email = "updateduser@example.com",
				FirstName = "UpdatedFirstName",
				LastName = "UpdatedLastName",
				CompanyName = "UpdatedCompany",
				Position = "Dispatcher",
				PhoneNumber = "0987654321"
			};

			_mockUserService.Setup(u => u.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(testUser);
			_mockHtmlSanitizerService.Setup(s => s.Sanitize(It.IsAny<string>())).Returns<string>(x => x);
			_mockProfileHelperService.Setup(s => s.IsEmailTakenAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(true);

			var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _profileService.UpdateProfileInformation(model, testUser.Id));
			Assert.That(exception.Message, Is.EqualTo(EmailIsAlreadyTaken));
		}

		[Test]
		public async Task UpdateProfileInformation_ShouldThrowException_WhenUsernameIsTaken()
		{
			var testUser = new User
			{
				Id = Guid.NewGuid(),
				UserName = "Gosho",
				Email = "goshko@softuni.bg",
				FirstName = "Gosho",
				LastName = "Geshev",
				Position = "Dispatcher",
				CompanyName = "SoftUni",
				PhoneNumber = "1234567890"
			};

			var model = new ProfileViewModel
			{
				Id = testUser.Id.ToString(),
				Username = "ExistingUser",
				Email = "goshko@softuni.bg",
				FirstName = "UpdatedFirstName",
				LastName = "UpdatedLastName",
				CompanyName = "UpdatedCompany",
				Position = "Dispatcher",
				PhoneNumber = "0987654321"
			};

			_mockUserService.Setup(u => u.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(testUser);
			_mockHtmlSanitizerService.Setup(s => s.Sanitize(It.IsAny<string>())).Returns<string>(x => x);
			_mockProfileHelperService.Setup(s => s.IsUsernameTakenAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(true);

			var exception = Assert.ThrowsAsync<InvalidDataException>(async () => await _profileService.UpdateProfileInformation(model, testUser.Id));
			Assert.That(exception.Message, Is.EqualTo(UserNameIsAlreadyTaken));
		}

		[Test]
		public async Task UpdateProfileInformation_ShouldThrowException_WhenNoChangesAreMade()
		{

			var testUser = new User
			{
				Id = Guid.NewGuid(),
				UserName = "Gosho",
				Email = "goshko@softuni.bg",
				FirstName = "Gosho",
				LastName = "Geshev",
				Position = "Dispatcher",
				CompanyName = "SoftUni",
				PhoneNumber = "1234567890"
			};

			var model = new ProfileViewModel
			{
				Id = testUser.Id.ToString(),
				Username = "Gosho",
				Email = "goshko@softuni.bg",
				FirstName = "Gosho",
				LastName = "Geshev",
				Position = "Dispatcher",
				CompanyName = "SoftUni",
				PhoneNumber = "1234567890"
			};

			_mockUserService.Setup(u => u.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(testUser);
			_mockHtmlSanitizerService.Setup(s => s.Sanitize(It.IsAny<string>())).Returns<string>(x => x);

			var exception = Assert.ThrowsAsync<Exception>(async () => await _profileService.UpdateProfileInformation(model, testUser.Id));
			Assert.That(exception.Message, Is.EqualTo(NoChangesMadeToProfile));
		}

		[Test]
		public async Task UpdateUserClaimsAsync_ShouldRemoveAndAddClaims_WhenMissingClaimsExist()
		{
			var user = new User
			{
				Id = Guid.NewGuid(),
				UserName = "gosho"
			};

			var model = new ProfileViewModel
			{
				FirstName = "UpdatedFirstName",
				LastName = "UpdatedLastName",
				Username = "UpdatedUser",
				Position = "Dispatcher"
			};

			var existingClaims = new List<Claim>
			{
				new Claim("FirstName", "OldFirstName"),
				new Claim("LastName", "OldLastName"),
				new Claim("Username", "OldUsername")
			};

			var missingClaims = new List<Claim>
			{
				new Claim("FirstName", "UpdatedFirstName"),
				new Claim("LastName", "UpdatedLastName"),
				new Claim("Username", "UpdatedUser")
			};

			_mockHtmlSanitizerService.Setup(s => s.Sanitize(It.IsAny<string>())).Returns<string>(x => x);
			_mockProfileHelperService.Setup(s => s.GetClaimsAsync(It.IsAny<BaseUser>())).ReturnsAsync(existingClaims);
			_mockProfileHelperService.Setup(s => s.GetMissingClaims(existingClaims, It.IsAny<string>(), It.IsAny<string>(),
				It.IsAny<string>(), It.IsAny<string>())).Returns(missingClaims);

			_mockUserManager.Setup(u => u.RemoveClaimsAsync(user, It.IsAny<IEnumerable<Claim>>())).ReturnsAsync(IdentityResult.Success);
			_mockUserManager.Setup(u => u.AddClaimsAsync(user, missingClaims)).ReturnsAsync(IdentityResult.Success);

			await _profileService.UpdateUserClaimsAsync(user, model);

			_mockHtmlSanitizerService.Verify(s => s.Sanitize(It.IsAny<string>()), Times.Exactly(3));
			_mockProfileHelperService.Verify(s => s.GetClaimsAsync(user), Times.Once);
			_mockUserManager.Verify(u => u.RemoveClaimsAsync(user, It.IsAny<IEnumerable<Claim>>()), Times.Once);
			_mockUserManager.Verify(u => u.AddClaimsAsync(user, missingClaims), Times.Once);
		}

		[Test]
		public async Task UpdateUserClaimsAsync_ShouldDoNothing_WhenAllClaimsAreUpToDate()
		{
			// Arrange
			var user = new User
			{
				Id = Guid.NewGuid(),
				UserName = "gosho"
			};

			var model = new ProfileViewModel
			{
				FirstName = "UpToDateFirstName",
				LastName = "UpToDateLastName",
				Username = "UpToDateUser",
				Position = "Dispatcher"
			};

			var sanitizedFirstName = "UpToDateFirstName"; 
			var sanitizedLastName = "UpToDateLastName";   
			var sanitizedUserName = "UpToDateUser";       

			_mockHtmlSanitizerService.Setup(s => s.Sanitize(It.IsAny<string>()))
				.Returns<string>(input =>
					input == model.FirstName ? sanitizedFirstName :
					input == model.LastName ? sanitizedLastName :
					input == model.Username ? sanitizedUserName :
					input); 

			var existingClaims = new List<Claim>
			{
				new Claim("FirstName", sanitizedFirstName),
				new Claim("LastName", sanitizedLastName),
				new Claim("UserName", sanitizedUserName),
				new Claim("Position", "Dispatcher")
			};

			_mockProfileHelperService.Setup(s => s.GetClaimsAsync(It.IsAny<BaseUser>())).ReturnsAsync(existingClaims);

			_mockProfileHelperService
				.Setup(s => s.GetMissingClaims(existingClaims, sanitizedFirstName, sanitizedLastName, sanitizedUserName, model.Position))
				.Returns(new List<Claim>()); 

			
			await _profileService.UpdateUserClaimsAsync(user, model);

			_mockProfileHelperService.Verify(s => s.GetClaimsAsync(user), Times.Once);
			_mockProfileHelperService.Verify(
				s => s.GetMissingClaims(existingClaims, sanitizedFirstName, sanitizedLastName, sanitizedUserName, model.Position),
				Times.Once);

			_mockUserManager.Verify(u => u.RemoveClaimsAsync(It.IsAny<BaseUser>(), It.IsAny<IEnumerable<Claim>>()), Times.Never);
			_mockUserManager.Verify(u => u.AddClaimsAsync(It.IsAny<BaseUser>(), It.IsAny<IEnumerable<Claim>>()), Times.Never);
		}

		[Test]
		public async Task ChangePasswordAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
		{
			var currentPassword = "OldPassword123!";
			var newPassword = "NewPassword123!";

			var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
				await _profileService.ChangePasswordAsync(null, currentPassword, newPassword));

			Assert.That(exception.ParamName, Is.EqualTo("user"));
			Assert.That(exception.Message, Does.Contain(UserCannotBeNull));
		}

		[Test]
		public async Task ChangePasswordAsync_ShouldReturnFailedResult_WhenPasswordsAreSame()
		{
			var user = new User();
			var password = "SamePassword123!";

			var result = await _profileService.ChangePasswordAsync(user, password, password);

			Assert.That(result.Succeeded, Is.False);
			Assert.That(result.Errors.First().Description, Is.EqualTo(CurrentAndNewPasswordCannotMatch));
		}

		[Test]
		public async Task ChangePasswordAsync_ShouldReturnSuccessResult_WhenPasswordsAreValid()
		{
			var user = new User();
			var currentPassword = "OldPassword123!";
			var newPassword = "NewPassword123!";

			_mockUserManager
				.Setup(m => m.ChangePasswordAsync(user, currentPassword, newPassword))
				.ReturnsAsync(IdentityResult.Success);

			var result = await _profileService.ChangePasswordAsync(user, currentPassword, newPassword);

			Assert.That(result.Succeeded, Is.False);
		}

		[Test]
		public async Task ChangePasswordAsync_ShouldReturnFailedResult_WhenPasswordChangeFails()
		{
			var user = new User();
			var currentPassword = "OldPassword123!";
			var newPassword = "NewPassword123!";

			_mockUserManager
				.Setup(m => m.ChangePasswordAsync(user, currentPassword, newPassword))
				.ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = CurrentAndNewPasswordCannotMatch }));

			var result = await _profileService.ChangePasswordAsync(user, currentPassword, newPassword);

			Assert.That(result.Succeeded, Is.False);
			Assert.That(result.Errors.First().Description, Is.EqualTo(CurrentAndNewPasswordCannotMatch));
		}


	}
}
