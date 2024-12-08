using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

using Moq;
using NUnit.Framework;

using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace LoadVantage.Tests.UnitTests.Core.Services
{
    public class ProfileHelperServiceTests
    {
        private ProfileHelperService _profileHelperService;
        private Mock<IUserService> _mockUserService;
        private Mock<UserManager<BaseUser>> _mockUserManager;
        private LoadVantageDbContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<LoadVantageDbContext>()
                .UseInMemoryDatabase(databaseName: "LoadVantageTestInMemoryDB")
                .Options;

            _dbContext = new LoadVantageDbContext(options);

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

            _mockUserService = new Mock<IUserService>();


            _profileHelperService = new ProfileHelperService(
                _dbContext,
                _mockUserService.Object,
                _mockUserManager.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task IsUsernameTakenAsync_ShouldReturnTrue_WhenUsernameIsTakenByOtherUser()
        {
            var username = "existingUser";
            var currentUserId = Guid.NewGuid();

            var existingUser = new User()
            {
                Id = Guid.NewGuid(),
                UserName = username,
                FirstName = "Gosho",
                LastName = "Geshev",
                Email = "gosho@softuni.bg"
            };

            await _dbContext.Users.AddAsync(existingUser);
            await _dbContext.SaveChangesAsync();

            var result = await _profileHelperService.IsUsernameTakenAsync(username, currentUserId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsUsernameTakenAsync_ShouldReturnFalse_WhenUsernameIsNotTaken()
        {
            var username = "batman";
            var currentUserId = Guid.NewGuid();

            var result = await _profileHelperService.IsUsernameTakenAsync(username, currentUserId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsEmailTakenAsync_ShouldReturnTrue_WhenEmailIsTakenByOtherUser()
        {
            var email = "batman@chucknorris.com";
            var currentUserId = Guid.NewGuid();

            var existingUser = new User()
            {
                Id = Guid.NewGuid(),
                UserName = "goshko",
                FirstName = "Gosho",
                LastName = "Geshev",
                Email = email
            };

            await _dbContext.Users.AddAsync(existingUser);
            await _dbContext.SaveChangesAsync();

            var result = await _profileHelperService.IsEmailTakenAsync(email, currentUserId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsEmailTakenAsync_ShouldReturnFalse_WhenEmailIsNotTaken()
        {
            var email = "batman@chucknorris.com";
            var currentUserId = Guid.NewGuid();

            var result = await _profileHelperService.IsEmailTakenAsync(email, currentUserId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task FindUserByUsernameAsync_ShouldReturnUser_WhenUsernameExists()
        {
            var username = "existingUser";
            var expectedUser = new User()
            {
                Id = Guid.NewGuid(),
                UserName = username,
                FirstName = "Bruce",
                LastName = "Wayne",
                Email = "batman@wayneenterprises.com",
                Position = "Dispatcher"
            };

            _mockUserManager.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(expectedUser);

            var result = await _profileHelperService.FindUserByUsernameAsync(username);

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.UserName, Is.EqualTo(expectedUser.UserName));
        }


        [Test]
        public async Task FindUserByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var username = "batman";

            var result = await _profileHelperService.FindUserByUsernameAsync(username);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task FindUserByUsernameAsync_ShouldReturnNull_WhenUsernameDoesNotExist()
        {
            var username = "nonExistentUser";

            _mockUserManager.Setup(um => um.FindByNameAsync(username)).ReturnsAsync((BaseUser?)null);

            var result = await _profileHelperService.FindUserByUsernameAsync(username);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void FindUserByUsernameAsync_ShouldThrowArgumentException_WhenUsernameIsNullOrWhitespace()
        {
            string username = null;

            Assert.That(() => _profileHelperService.FindUserByUsernameAsync(username), Throws.ArgumentException.With.Message.Contains("username"));
            username = string.Empty;
            Assert.That(() => _profileHelperService.FindUserByUsernameAsync(username), Throws.ArgumentException.With.Message.Contains("username"));
        }

        [Test]
        public async Task GetClaimsAsync_ShouldReturnClaims_WhenUserHasClaims()
        {
            var user = new User()
            {
                Id = Guid.NewGuid(),
                UserName = "TestUser",
                FirstName = "Bruce",
                LastName = "Wayne",
                Email = "batman@wayneenterprises.com",
                Position = "Dispatcher"
            };

            var existingClaims = new List<Claim>
            {
                new Claim("FirstName", "Bruce"),
                new Claim("LastName", "Wayne"),
                new Claim("UserName", "TestUser"),
                new Claim("Position", "Dispatcher")
            };

            _mockUserService.Setup(s => s.GetUserClaimsAsync(user)).ReturnsAsync(existingClaims);

            var result = await _profileHelperService.GetClaimsAsync(user);

            Assert.That(result, Is.EqualTo(existingClaims));
        }

        [Test]
        public async Task GetClaimsAsync_ShouldReturnEmptyList_WhenNoClaimsExist()
        {
            var user = new User()
            {
                Id = Guid.NewGuid(),
                UserName = "TestUser",
                FirstName = "Bruce",
                LastName = "Wayne",
                Email = "batman@wayneenterprises.com",
                Position = "Dispatcher"
            };

            _mockUserService.Setup(s => s.GetUserClaimsAsync(user)).ReturnsAsync(new List<Claim>());

            var result = await _profileHelperService.GetClaimsAsync(user);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetMissingClaims_ShouldReturnMissingClaims_WhenClaimsAreMissing()
        {
            var existingClaims = new List<Claim>
            {
                new Claim("FirstName", "ExistingFirstName"),
                new Claim("LastName", "ExistingLastName")
            };

            var firstName = "NewFirstName";
            var lastName = "NewLastName";
            var userName = "NewUserName";
            var position = "Dispatcher";

            var result = _profileHelperService.GetMissingClaims(existingClaims, firstName, lastName, userName, position);


            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result.Exists(c => c.Type == "FirstName" && c.Value == firstName), Is.True);
            Assert.That(result.Exists(c => c.Type == "LastName" && c.Value == lastName), Is.True);
            Assert.That(result.Exists(c => c.Type == "UserName" && c.Value == userName), Is.True);
        }

        [Test]
        public void GetMissingClaims_ShouldReturnEmpty_WhenNoClaimsAreMissing()
        {
            var existingClaims = new List<Claim>
            {
                new Claim("FirstName", "ExistingFirstName"),
                new Claim("LastName", "ExistingLastName"),
                new Claim("UserName", "ExistingUserName"),
                new Claim("Position", "Dispatcher")
            };
            var firstName = "ExistingFirstName";
            var lastName = "ExistingLastName";
            var userName = "ExistingUserName";
            var position = "Dispatcher";

            var result = _profileHelperService.GetMissingClaims(existingClaims, firstName, lastName, userName, position);

            Assert.That(result, Is.Empty);
        }


    }
}
