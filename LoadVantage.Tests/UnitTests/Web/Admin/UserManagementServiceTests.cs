using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Moq;
using NUnit.Framework;

using LoadVantage.Common.Enums;

using LoadVantage.Core.Contracts;

using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data;

using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Services;
using LoadVantage.Areas.Admin.Models.User;

using Role = LoadVantage.Infrastructure.Data.Models.Role;

using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.GeneralConstants.AdministratorManagement;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using static LoadVantage.Common.GeneralConstants.UserImage;




namespace LoadVantage.Tests.UnitTests.Web.Admin
{
    public class UserManagementServiceTests
    {
        private LoadVantageDbContext _dbContext;
        private Mock<UserManager<BaseUser>> _userManager;
        private Mock<RoleManager<Role>> _roleManager;
        private Mock<IUserService> _userService;
        private Mock<IHtmlSanitizerService> _htmlSanitizer;
        private Mock<IAdminUserService> _adminUserServiceMock;

        private UserManagementService _userManagementService;

        [SetUp]
        public void Setup()
        {
            _userManager = new Mock<UserManager<BaseUser>>(
                new Mock<IUserStore<BaseUser>>().Object,
                null, null, null, null, null, null, null, null);

            _userManager
                .Setup(um => um.CreateAsync(It.IsAny<BaseUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManager
                .Setup(um => um.AddToRoleAsync(It.IsAny<BaseUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _roleManager = new Mock<RoleManager<Role>>(
                Mock.Of<IRoleStore<Role>>(), null, null, null, null);

            _roleManager
                .Setup(r => r.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new Role { Id = Guid.NewGuid(), Name = AdminRoleName });

            _userService = new Mock<IUserService>();

            _userService
                .Setup(u => u.AddUserClaimsAsync(It.IsAny<BaseUser>(), It.IsAny<List<Claim>>()))
                .ReturnsAsync(IdentityResult.Success);

            _htmlSanitizer = new Mock<IHtmlSanitizerService>();
            _htmlSanitizer
                .Setup(h => h.Sanitize(It.IsAny<string>()))
                .Returns<string>(s => s);

            _adminUserServiceMock = new Mock<IAdminUserService>();

            var options = new DbContextOptionsBuilder<LoadVantageDbContext>()
                .UseInMemoryDatabase(databaseName: "LoadVantageTestInMemoryDB")
                .Options;

            _dbContext = new LoadVantageDbContext(options);

            _userManagementService = new UserManagementService(
                _dbContext,
                _userManager.Object,
                _roleManager.Object,
                _userService.Object,
                _htmlSanitizer.Object,
                _adminUserServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetUsersAsync_ValidInput_ReturnsCorrectData()
        {
            var user1 = new Administrator()
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                Email = "bruce.wayne@batman.com",
                PhoneNumber = "1234567890",
                Position = "Administrator",
                CompanyName = "Example Inc.",
                UserName = "batman",
                IsActive = true,
                UserImage = new UserImage
                {
                    ImageUrl = "http://batman.com/caped-crusader.jpg",
                    PublicId = "this is batman man"
                },
                Role = new Role
                {
                    Name = "Administrator"
                }
            };

            var user2 = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "Harvey",
                LastName = "Spectre",
                Email = "harvey_wins@law.com",
                PhoneNumber = null,
                Position = "Broker",
                CompanyName = "Spectre Litt",
                UserName = "spectre",
                IsActive = false,
                UserImage = null,
                Role = new Role
                {
                    Name = "User"
                }
            };

            await _dbContext.Users.AddRangeAsync(user1, user2);
            await _dbContext.SaveChangesAsync();


            var result = await _userManagementService.GetUsersAsync(pageNumber: 1, pageSize: 10);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));

            var resultUser1 = result.FirstOrDefault(u => u.FirstName == "Bruce");
            Assert.That(resultUser1, Is.Not.Null);
            Assert.That(resultUser1.Email, Is.EqualTo("bruce.wayne@batman.com"));
            Assert.That(resultUser1.PhoneNumber, Is.EqualTo("1234567890"));
            Assert.That(resultUser1.Position, Is.EqualTo("Administrator"));
            Assert.That(resultUser1.Role, Is.EqualTo("Administrator"));
            Assert.That(resultUser1.UserImageUrl, Is.EqualTo("http://batman.com/caped-crusader.jpg"));
            Assert.That(resultUser1.IsActive, Is.True);

            var resultUser2 = result.FirstOrDefault(u => u.FirstName == "Harvey");
            Assert.That(resultUser2, Is.Not.Null);
            Assert.That(resultUser2.Email, Is.EqualTo("harvey_wins@law.com"));
            Assert.That(resultUser2.PhoneNumber, Is.EqualTo("N/A"));
            Assert.That(resultUser2.Position, Is.EqualTo("Broker"));
            Assert.That(resultUser2.Role, Is.EqualTo("User"));
            Assert.That(resultUser2.UserImageUrl, Is.EqualTo("N/A"));
            Assert.That(resultUser2.IsActive, Is.False);
        }

        [Test]
        public async Task SearchUsersAsync_NullOrWhiteSpaceSearchTerm_ReturnsAllUsers()
        {

            var user1 = new Administrator()
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                Email = "bruce.wayne@batman.com",
                PhoneNumber = "1234567890",
                Position = "Administrator",
                CompanyName = "Example Inc.",
                UserName = "batman",
                IsActive = true,
                UserImage = new UserImage
                {
                    ImageUrl = "http://batman.com/caped-crusader.jpg",
                    PublicId = "this is batman man"
                },
                Role = new Role
                {
                    Name = "Administrator"
                }
            };

            var user2 = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "Harvey",
                LastName = "Spectre",
                Email = "harvey_wins@law.com",
                PhoneNumber = null,
                Position = "Broker",
                CompanyName = "Spectre Litt",
                UserName = "spectre",
                IsActive = false,
                UserImage = new UserImage
                {
                    ImageUrl = "http://spectre-litt.com/king-harvey.jpg",
                    PublicId = "get litt up"
                },
                Role = new Role
                {
                    Name = "User"
                }
            };

            await _dbContext.Users.AddRangeAsync(user1, user2);
            await _dbContext.SaveChangesAsync();

            var result = await _userManagementService.SearchUsersAsync("  ");

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task SearchUsersAsync_SearchByName_ReturnsMatchingUsers()
        {
            var user1 = new Administrator()
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                Email = "bruce.wayne@batman.com",
                PhoneNumber = "1234567890",
                Position = "Administrator",
                CompanyName = "Example Inc.",
                UserName = "batman",
                IsActive = true,
                UserImage = new UserImage
                {
                    ImageUrl = "http://batman.com/caped-crusader.jpg",
                    PublicId = "this is batman man"
                },
                Role = new Role
                {
                    Name = "Administrator"
                }
            };

            await _dbContext.Users.AddAsync(user1);
            await _dbContext.SaveChangesAsync();

            var result = await _userManagementService.SearchUsersAsync("Bruce Wayne");

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().FirstName, Is.EqualTo("Bruce"));
            Assert.That(result.First().LastName, Is.EqualTo("Wayne"));
        }

        [Test]
        public async Task SearchUsersAsync_SearchByEmail_ReturnsMatchingUsers()
        {
            var user = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "Harvey",
                LastName = "Spectre",
                Email = "harvey_wins@law.com",
                PhoneNumber = null,
                Position = "Broker",
                CompanyName = "Spectre Litt",
                UserName = "spectre",
                IsActive = false,
                UserImage = new UserImage
                {
                    ImageUrl = "http://spectre-litt.com/king-harvey.jpg",
                    PublicId = "get litt up"
                },
                Role = new Role
                {
                    Name = "User"
                }
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var result = await _userManagementService.SearchUsersAsync("harvey_wins@law.com");

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Email, Is.EqualTo("harvey_wins@law.com"));
        }

        [Test]
        public async Task SearchUsersAsync_SearchByPhoneNumber_ReturnsMatchingUsers()
        {

            var user = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "Harvey",
                LastName = "Spectre",
                Email = "harvey_wins@law.com",
                PhoneNumber = "+1-800-659-6481",
                Position = "Broker",
                CompanyName = "Spectre Litt",
                UserName = "spectre",
                IsActive = false,
                UserImage = new UserImage
                {
                    ImageUrl = "http://spectre-litt.com/king-harvey.jpg",
                    PublicId = "get litt up"
                },
                Role = new Role
                {
                    Name = "User"
                }
            };


            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var result = await _userManagementService.SearchUsersAsync("+1-800-659-6481");

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().PhoneNumber, Is.EqualTo("+1-800-659-6481"));
        }

        [Test]
        public async Task SearchUsersAsync_SearchByUsername_ReturnsMatchingUsers()
        {
            var user = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "Harvey",
                LastName = "Spectre",
                Email = "harvey_wins@law.com",
                PhoneNumber = "+1-800-659-6481",
                Position = "Broker",
                CompanyName = "Spectre Litt",
                UserName = "spectre",
                IsActive = false,
                UserImage = new UserImage
                {
                    ImageUrl = "http://spectre-litt.com/king-harvey.jpg",
                    PublicId = "get litt up"
                },
                Role = new Role
                {
                    Name = "User"
                }
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var result = await _userManagementService.SearchUsersAsync("spectre");

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().UserName, Is.EqualTo("spectre"));
        }

        [Test]
        public async Task SearchUsersAsync_SearchByPosition_ReturnsMatchingUsers()
        {
            var user = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "Harvey",
                LastName = "Spectre",
                Email = "harvey_wins@law.com",
                PhoneNumber = "+1-800-659-6481",
                Position = "Broker",
                CompanyName = "Spectre Litt",
                UserName = "spectre",
                IsActive = false,
                UserImage = new UserImage
                {
                    ImageUrl = "http://spectre-litt.com/king-harvey.jpg",
                    PublicId = "get litt up"
                },
                Role = new Role
                {
                    Name = "User"
                }
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var result = await _userManagementService.SearchUsersAsync("Broker");

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Position, Is.EqualTo("Broker"));
        }

        [Test]
        public async Task SearchUsersAsync_SearchByCompanyName_ReturnsMatchingUsers()
        {

            var user = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "Harvey",
                LastName = "Spectre",
                Email = "harvey_wins@law.com",
                PhoneNumber = "+1-800-659-6481",
                Position = "Broker",
                CompanyName = "Spectre Litt",
                UserName = "spectre",
                IsActive = false,
                UserImage = new UserImage
                {
                    ImageUrl = "http://spectre-litt.com/king-harvey.jpg",
                    PublicId = "get litt up"
                },
                Role = new Role
                {
                    Name = "User"
                }
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var result = await _userManagementService.SearchUsersAsync("Spectre Litt");

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().CompanyName, Is.EqualTo("Spectre Litt"));
        }

        [Test]
        public async Task SearchUsersAsync_NoMatches_ReturnsEmptyList()
        {

            var user = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "Harvey",
                LastName = "Spectre",
                Email = "harvey_wins@law.com",
                PhoneNumber = "+1-800-659-6481",
                Position = "Broker",
                CompanyName = "Spectre Litt",
                UserName = "spectre",
                IsActive = false,
                UserImage = new UserImage
                {
                    ImageUrl = "http://spectre-litt.com/king-harvey.jpg",
                    PublicId = "get litt up"
                },
                Role = new Role
                {
                    Name = "User"
                }
            };


            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();


            var result = await _userManagementService.SearchUsersAsync("asdskljnashjlkdahshd");

            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.TypeOf<List<UserManagementViewModel>>());
        }

        [Test]
        public async Task CreateAdministratorAsync_ValidModel_CreatesAdministratorAndReturnsViewModel()
        {
            var adminRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Administrator"
            };

            var adminModel = new AdminCreateUserViewModel
            {
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "Administrator",
                Email = "bruce.wayne@batman.com",
                Password = "iAmBatman123",
                Role = adminRole.ToString(),
                UserImageUrl = "http://batman.com/caped-crusader.jpg"
            };


            _roleManager.Setup(r => r.FindByNameAsync("Administrator")).ReturnsAsync(adminRole);

            _userManager.Setup(u => u.CreateAsync(It.IsAny<User>(), adminModel.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManager.Setup(u => u.AddToRoleAsync(It.IsAny<User>(), "Admin"))
                .ReturnsAsync(IdentityResult.Success);

            _userService.Setup(u => u.AddUserClaimsAsync(It.IsAny<User>(), It.IsAny<List<Claim>>()))
                .ReturnsAsync(IdentityResult.Success);

            _htmlSanitizer.Setup(h => h.Sanitize(It.IsAny<string>())).Returns<string>(s => s);


            var result = await _userManagementService.CreateAdministratorAsync(adminModel);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.FirstName, Is.EqualTo(adminModel.FirstName));
            Assert.That(result.LastName, Is.EqualTo(adminModel.LastName));
            Assert.That(result.Email, Is.EqualTo(adminModel.Email));
            Assert.That(result.UserName, Is.EqualTo(adminModel.UserName));
            Assert.That(result.Role, Is.EqualTo(adminModel.Role));
        }


        [Test]
        public void CreateAdministratorAsync_UserCreationFails_ThrowsInvalidOperationException()
        {
            var adminRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Administrator"
            };

            var adminModel = new AdminCreateUserViewModel
            {
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "Administrator",
                Email = "bruce.wayne@batman.com",
                Password = "iAmBatman123",
                Role = "Administrator"
            };

            _roleManager.Setup(r => r.FindByNameAsync("Admin")).ReturnsAsync(adminRole);

            _userManager.Setup(u => u.CreateAsync(It.IsAny<BaseUser>(), adminModel.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password is too weak." }));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _userManagementService.CreateAdministratorAsync(adminModel));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("User creation failed"));
        }

        [Test]
        public async Task CreateUserAsync_ShouldCreateUserSuccessfully()
        {
            var model = new AdminCreateUserViewModel
            {
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "User",
                Email = "bruce.wayne@batman.com",
                Password = "iAmBatman123",
                Role = "User"
            };

            var result = await _userManagementService.CreateUserAsync(model);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.Not.Empty);
            Assert.That(result.FirstName, Is.EqualTo(model.FirstName));
            Assert.That(result.LastName, Is.EqualTo(model.LastName));
            Assert.That(result.Email, Is.EqualTo(model.Email));
            Assert.That(result.PhoneNumber, Is.EqualTo(model.PhoneNumber));
            Assert.That(result.CompanyName, Is.EqualTo(model.CompanyName));
            Assert.That(result.UserName, Is.EqualTo(model.UserName));
            Assert.That(result.Position, Is.EqualTo(model.Position));
            Assert.That(result.Role, Is.EqualTo(model.Role));
            Assert.That(result.Password, Is.EqualTo(model.Password));

            _userManager.Verify(um => um.CreateAsync(It.IsAny<BaseUser>(), It.Is<string>(p => p == model.Password)), Times.Once);
            _userManager.Verify(um => um.AddToRoleAsync(It.IsAny<BaseUser>(), It.Is<string>(r => r == model.Role)), Times.Once);
            _userService.Verify(us => us.AddUserClaimsAsync(It.IsAny<BaseUser>(), It.IsAny<List<Claim>>()), Times.Once);
        }


        [Test]
        public void CreateUserAsync_ShouldThrowArgumentNullException_WhenModelIsNull()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _userManagementService.CreateUserAsync(null));

            Assert.That(ex.Message, Does.Contain(ModelCannotBeNull));
        }

        [Test]
        public void CreateUserAsync_ShouldThrowInvalidOperationException_WhenUserCreationFails()
        {
            _userManager
                .Setup(um => um.CreateAsync(It.IsAny<BaseUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = UserCreationFailed }));

            var model = new AdminCreateUserViewModel
            {
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "User",
                Email = "bruce.wayne@batman.com",
                Password = "iAmBatman123",
                Role = "User"
            };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userManagementService.CreateUserAsync(model));

            Assert.That(ex.Message, Does.Contain(UserCreationFailed));
        }

        [Test]
        public void CreateUserAsync_ShouldThrowInvalidOperationException_WhenRoleAssignmentFails()
        {
            _userManager
                .Setup(um => um.AddToRoleAsync(It.IsAny<BaseUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = RoleAssignmentFailed }));

            var model = new AdminCreateUserViewModel
            {
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "User",
                Email = "bruce.wayne@batman.com",
                Password = "iAmBatman123",
                Role = "User"
            };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userManagementService.CreateUserAsync(model));

            Assert.That(ex.Message, Does.Contain(RoleAssignmentFailed));
        }

        [Test]
        public void DeactivateUserAsync_ShouldThrowArgumentException_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userManagementService.DeactivateUserAsync(userId));

            Assert.That(ex.Message, Does.Contain(UserNotFound));
        }

        [Test]
        public async Task DeactivateUserAsync_ShouldThrowInvalidOperationException_WhenBrokerHasLoadsInTransit()
        {

            var broker = new Broker
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "User",
                Email = "bruce.wayne@batman.com",
                IsActive = true
            };


            var load = new Load
            {
                BrokerId = broker.Id,
                OriginCity = "Boardman",
                OriginState = "OH",
                DestinationCity = "Miami",
                DestinationState = "FL",
                Status = LoadStatus.Booked,
                BookedLoad = new BookedLoad
                {
                    DriverId = Guid.NewGuid()
                }
            };

            await _dbContext.Users.AddAsync(broker);
            await _dbContext.Loads.AddAsync(load);
            await _dbContext.SaveChangesAsync();


            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userManagementService.DeactivateUserAsync(broker.Id));

            Assert.That(ex.Message, Does.Contain(CannotDeactivateBroker));
        }

        [Test]
        public async Task DeactivateUserAsync_ShouldThrowInvalidOperationException_WhenDispatcherHasActiveDrivers()
        {
            var dispatcher = new Dispatcher
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "Dispatcher",
                Email = "bruce.wayne@batman.com",
                IsActive = true
            };

            var driver = new Driver
            {
                DispatcherId = dispatcher.Id,
                TruckId = Guid.NewGuid(),
                LicenseNumber = "213213546",
                FirstName = "Robin",
                LastName = "Hood",
                IsBusy = true
            };

            await _dbContext.Users.AddAsync(dispatcher);
            await _dbContext.Drivers.AddAsync(driver);
            await _dbContext.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userManagementService.DeactivateUserAsync(dispatcher.Id));

            Assert.That(ex.Message, Does.Contain(CannotDeactivateDispatcher));
        }

        [Test]
        public async Task DeactivateUserAsync_ShouldDeactivateUserAndAnonymizeInformation()
        {
            var userImage = new UserImage
            {
                Id = Guid.NewGuid(),
                PublicId = "www.some-website.com/picture",
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "Dispatcher",
                Email = "bruce.wayne@batman.com",
                IsActive = true,
                UserImage = userImage,
                UserImageId = userImage.Id
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var result = await _userManagementService.DeactivateUserAsync(user.Id);

            var updatedUser = await _dbContext.Users.FindAsync(user.Id);

            Assert.That(result, Is.True);
            Assert.That(updatedUser.FirstName, Is.EqualTo("N/A"));
            Assert.That(updatedUser.LastName, Is.EqualTo("N/A"));
            Assert.That(updatedUser.Email, Is.EqualTo("N/A"));
            Assert.That(updatedUser.CompanyName, Is.EqualTo("N/A"));
            Assert.That(updatedUser.PhoneNumber, Is.EqualTo("N/A"));
            Assert.That(updatedUser.IsActive, Is.False);

            _userService.Verify(u => u.DeleteUserImageAsync(user.Id, userImage.Id), Times.Once);
        }

        [Test]
        public async Task DeactivateUserAsync_ShouldSkipImageDeletion_WhenUserHasDefaultProfilePicture()
        {
            var userImage = new UserImage
            {
                Id = DefaultImageId,
                PublicId = DefaultPublicId,
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "Dispatcher",
                Email = "bruce.wayne@batman.com",
                IsActive = true,
                UserImage = userImage,
                UserImageId = userImage.Id
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var result = await _userManagementService.DeactivateUserAsync(user.Id);

            var updatedUser = await _dbContext.Users.FindAsync(user.Id);

            Assert.That(result, Is.True);
            Assert.That(updatedUser.FirstName, Is.EqualTo("N/A"));
            Assert.That(updatedUser.LastName, Is.EqualTo("N/A"));
            Assert.That(updatedUser.Email, Is.EqualTo("N/A"));
            Assert.That(updatedUser.CompanyName, Is.EqualTo("N/A"));
            Assert.That(updatedUser.PhoneNumber, Is.EqualTo("N/A"));
            Assert.That(updatedUser.IsActive, Is.False);

            _userService.Verify(u => u.DeleteUserImageAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task ReactivateUserAsync_ShouldReactivateUserSuccessfully_WhenUserExists()
        {
            var userId = Guid.NewGuid();

            var existingUser = new User
            {
                Id = userId,
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "Dispatcher",
                Email = "bruce.wayne@batman.com",
                IsActive = false,
                UserImageId = Guid.NewGuid()
            };

            await _dbContext.Users.AddAsync(existingUser);
            await _dbContext.SaveChangesAsync();

            var result = await _userManagementService.ReactivateUserAsync(userId);

            var reactivatedUser = await _dbContext.Users.FindAsync(userId);

            Assert.That(result, Is.True);
            Assert.That(reactivatedUser.IsActive, Is.True);
            Assert.That(reactivatedUser.UserImageId, Is.EqualTo(DefaultImageId));
        }

        [Test]
        public void ReactivateUserAsync_ShouldThrowArgumentException_WhenUserNotFound()
        {
            var nonExistingUserId = Guid.NewGuid();

            var nonExistingUser = new User
            {
                Id = nonExistingUserId,
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "Dispatcher",
                Email = "bruce.wayne@batman.com",
                IsActive = false,
                UserImageId = Guid.NewGuid()
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userManagementService.ReactivateUserAsync(nonExistingUser.Id));

            Assert.That(ex.Message, Does.Contain(UserNotFound));
        }

        [Test]
        public void UpdateUserAsync_ShouldThrowArgumentNullException_WhenModelIsNull()
        {
            AdminEditUserViewModel nullModel = null;

            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _userManagementService.UpdateUserAsync(nullModel));

            Assert.That(ex.Message, Does.Contain(ModelCannotBeNull));
        }

        [Test]
        public void UpdateUserAsync_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            var model = new AdminEditUserViewModel
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Email = "bruce.wayne@batman.com"
            };

            _userService
                .Setup(u => u.GetUserByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((BaseUser)null);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userManagementService.UpdateUserAsync(model));

            Assert.That(ex.Message, Does.Contain(UserNotFound));
        }

        [Test]
        public void UpdateUserAsync_ShouldThrowInvalidOperationException_WhenUpdateFails()
        {
            var userId = Guid.NewGuid();

            var model = new AdminEditUserViewModel
            {
                Id = userId.ToString(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Email = "bruce.wayne@batman.com"
            };

            var user = new User
            {
                Id = userId,
                UserName = "oldUsername",
                Email = "oldEmail@batman.com"
            };

            _userService
                .Setup(u => u.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _userService
                .Setup(u => u.UpdateUserAsync(It.IsAny<BaseUser>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = FailedToUpdateTheUser }));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userManagementService.UpdateUserAsync(model));

            Assert.That(ex.Message, Does.Contain(FailedToUpdateTheUser));
        }

        [Test]
        public async Task UpdateUserAsync_ShouldReturnTrue_WhenUpdateIsSuccessful()
        {
            var userId = Guid.NewGuid();

            var model = new AdminEditUserViewModel
            {
                Id = userId.ToString(),
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Email = "bruce.wayne@batman.com"
            };

            var user = new User
            {
                Id = userId,
                UserName = "oldUsername",
                Email = "oldEmail@batman.com"
            };

            _userService
                .Setup(u => u.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _userService
                .Setup(u => u.UpdateUserAsync(It.IsAny<BaseUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _userManagementService.UpdateUserAsync(model);

            Assert.That(result, Is.True);
            Assert.That(user.UserName, Is.EqualTo(model.UserName));
            Assert.That(user.Email, Is.EqualTo(model.Email));
            Assert.That(user.PhoneNumber, Is.EqualTo(model.PhoneNumber));
            Assert.That(user.FirstName, Is.EqualTo(model.FirstName));
            Assert.That(user.LastName, Is.EqualTo(model.LastName));
            Assert.That(user.CompanyName, Is.EqualTo(model.CompanyName));
        }

        [Test]
        public void ResetPasswordToDefaultAsync_ShouldThrowArgumentException_WhenUserNotFound()
        {
            var nonExistingUserId = Guid.NewGuid();

            _userService
                .Setup(u => u.GetUserByIdAsync(nonExistingUserId))
                .ReturnsAsync((BaseUser)null);

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userManagementService.ResetPasswordToDefaultAsync(nonExistingUserId));

            Assert.That(ex.Message, Does.Contain(UserNotFound));
        }

        [Test]
        public async Task ResetPasswordToDefaultAsync_ShouldReturnFailureResult_WhenAddUserDefaultPasswordFails()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "Dispatcher",
                Email = "bruce.wayne@batman.com",
                IsActive = false,
                UserImageId = Guid.NewGuid(),
                PasswordHash = "Password1"
            };

            var mockUserService = new Mock<IUserService>();
            var mockAdminUserService = new Mock<IAdminUserService>();

            mockUserService
                .Setup(u => u.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            mockAdminUserService
                .Setup(a => a.DeleteUserPassword(user))
                .ReturnsAsync(IdentityResult.Success);

            mockAdminUserService
                .Setup(a => a.AddUserDefaultPassword(user))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to add default password" }));

            var userManagementService = new UserManagementService(
                _dbContext,
                _userManager.Object,
                _roleManager.Object,
                mockUserService.Object,
                _htmlSanitizer.Object,
                mockAdminUserService.Object);


            var result = await userManagementService.ResetPasswordToDefaultAsync(userId);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors.First().Description, Is.EqualTo("Failed to add default password"));
        }

        [Test]
        public async Task ResetPasswordToDefaultAsync_ShouldReturnSuccess_WhenDeleteAndAddPasswordSucceed()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "Dispatcher",
                Email = "bruce.wayne@batman.com",
                IsActive = false,
                UserImageId = Guid.NewGuid(),
                PasswordHash = "Password1"
            };

            _userService
                .Setup(u => u.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            var mockAdminUserService = new Mock<IAdminUserService>();

            mockAdminUserService
                .Setup(a => a.DeleteUserPassword(user))
                .ReturnsAsync(IdentityResult.Success);

            mockAdminUserService
                .Setup(a => a.AddUserDefaultPassword(user))
                .ReturnsAsync(IdentityResult.Success);

            var userManagementService = new UserManagementService(
                _dbContext,
                _userManager.Object,
                _roleManager.Object,
                _userService.Object,
                _htmlSanitizer.Object,
                mockAdminUserService.Object);

            var result = await userManagementService.ResetPasswordToDefaultAsync(userId);

            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task ResetPasswordToDefaultAsync_ShouldReturnAddPasswordResult_WhenDeletePasswordSucceeds()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                FirstName = "Bruce",
                LastName = "Wayne",
                UserName = "batman",
                CompanyName = "Wayne Enterprises Inc.",
                PhoneNumber = "1234567890",
                Position = "Dispatcher",
                Email = "bruce.wayne@batman.com",
                IsActive = false,
                UserImageId = Guid.NewGuid(),
                PasswordHash = "Password1"
            };

            var mockAdminUserService = new Mock<IAdminUserService>();


            _userService
                .Setup(u => u.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            mockAdminUserService
                .Setup(a => a.DeleteUserPassword(user))
                .ReturnsAsync(IdentityResult.Success);

            var addPasswordResult = IdentityResult.Failed(new IdentityError { Description = "Failed to add default password" });
            mockAdminUserService
                .Setup(a => a.AddUserDefaultPassword(user))
                .ReturnsAsync(addPasswordResult);

            var userManagementService = new UserManagementService(
                _dbContext,
                _userManager.Object,
                _roleManager.Object,
                _userService.Object,
                _htmlSanitizer.Object,
                mockAdminUserService.Object);

            var result = await userManagementService.ResetPasswordToDefaultAsync(userId);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors.First().Description, Is.EqualTo("Failed to add default password"));
        }


    }
}
