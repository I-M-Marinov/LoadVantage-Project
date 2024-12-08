using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Core.Contracts;

using Moq;
using NUnit.Framework;

using LoadVantage.Areas.Admin.Services;
using LoadVantage.Areas.Admin.Models.Profile;
using LoadVantage.Common.Enums;
using LoadVantage.Infrastructure.Data.Models;

using Dispatcher = LoadVantage.Infrastructure.Data.Models.Dispatcher;

namespace LoadVantage.Tests.UnitTests.Web.Admin
{
    public class AdminLoadBoardServiceTests
    {

        private Mock<IAdminProfileService> _adminProfileService;
        private Mock<ILoadHelperService> _mockLoadHelperService;
        private IAdminLoadBoardService _adminLoadBoardService;

        [SetUp]
        public void SetUp()
        {

            _mockLoadHelperService = new Mock<ILoadHelperService>();
            _adminProfileService = new Mock<IAdminProfileService>();


            _adminLoadBoardService = new AdminLoadBoardService(
                _adminProfileService.Object,
                _mockLoadHelperService.Object
            );
        }

        [Test]
        public async Task GetLoadBoardManager_CallsGetAllLoads()
        {
            var userId = Guid.NewGuid();

            _mockLoadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(new List<Load>());
            _adminProfileService.Setup(s => s.GetAdminInformation(userId)).ReturnsAsync(new AdminProfileViewModel());

            await _adminLoadBoardService.GetLoadBoardManager(userId);

            _mockLoadHelperService.Verify(s => s.GetAllLoads(), Times.Once);
        }

        [Test]
        public async Task GetLoadBoardManager_CallsGetAdminInformation()
        {
            var userId = Guid.NewGuid();

            _mockLoadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(new List<Load>());
            _adminProfileService.Setup(s => s.GetAdminInformation(userId)).ReturnsAsync(new AdminProfileViewModel());

            await _adminLoadBoardService.GetLoadBoardManager(userId);

            _adminProfileService.Verify(s => s.GetAdminInformation(userId), Times.Once);
        }

        [Test]
        public async Task GetLoadBoardManager_ReturnsEmptyCategories_WhenNoLoads()
        {
            var userId = Guid.NewGuid();
            var allLoads = new List<Load>();

            _mockLoadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(allLoads);
            _adminProfileService.Setup(s => s.GetAdminInformation(userId)).ReturnsAsync(new AdminProfileViewModel());

            var result = await _adminLoadBoardService.GetLoadBoardManager(userId);

            Assert.That(result.CreatedLoads.Count, Is.EqualTo(0));
            Assert.That(result.PostedLoads.Count, Is.EqualTo(0));
            Assert.That(result.BookedLoads.Count, Is.EqualTo(0));
            Assert.That(result.DeliveredLoads.Count, Is.EqualTo(0));
            Assert.That(result.CancelledLoads.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetLoadBoardManager_ReturnsNullForProfile_WhenNoProfileFound()
        {
            var userId = Guid.NewGuid();
            var allLoads = new List<Load>
            {
                new Load { Status = LoadStatus.Created }
            };

            _mockLoadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(allLoads);
            _adminProfileService.Setup(s => s.GetAdminInformation(userId)).ReturnsAsync((AdminProfileViewModel)null);

            var result = await _adminLoadBoardService.GetLoadBoardManager(userId);

            Assert.That(result.Profile, Is.Null);
        }

        [Test]
        public async Task GetLoadBoardManager_ReturnsValidAdminLoadBoardViewModel()
        {
            var userId = Guid.NewGuid();

            var brokerId = Guid.NewGuid();
            var dispatcherId = Guid.NewGuid();
            var driverId = Guid.NewGuid();


            var allLoads = new List<Load>
            {
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Chicago",
                    OriginState = "IL",
                    DestinationCity = "Lansing",
                    DestinationState = "MI",
                    Status = LoadStatus.Created,
                    Broker = new Broker()
                    {
                        Id = brokerId,
                        FirstName = "Bruce",
                        LastName = "Wayne",
                        CompanyName = "Wayne Enterprises",
                        UserName = "batman",
                        Email = "batman@chucknorris.com",
                        Position = "Broker"
                    }
                },
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Sacramento",
                    OriginState = "CA",
                    DestinationCity = "Pittsburgh",
                    DestinationState = "PA",
                    Status = LoadStatus.Available,
                    Broker = new Broker()
                    {
                        Id = brokerId,
                        FirstName = "Bruce",
                        LastName = "Wayne",
                        CompanyName = "Wayne Enterprises",
                        UserName = "batman",
                        Email = "batman@chucknorris.com",
                        Position = "Broker"
                    }
                },
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Mattoon",
                    OriginState = "IL",
                    DestinationCity = "Pittsburgh",
                    DestinationState = "PA",
                    Status = LoadStatus.Booked,
                    Broker = new Broker()
                    {
                        Id = brokerId,
                        FirstName = "Bruce",
                        LastName = "Wayne",
                        CompanyName = "Wayne Enterprises",
                        UserName = "batman",
                        Email = "batman@chucknorris.com",
                        Position = "Broker"
                    },
                    BookedLoad = new BookedLoad()
                    {
                        Dispatcher = new Dispatcher()
                        {
                            Id = dispatcherId,
                            FirstName = "Gosho",
                            LastName = "Geshev",
                            UserName = "geshata",
                            Email = "gosho@goshata.com",
                            Position = "Dispatcher"
                        },
                        Driver =  new Driver()
                        {
                            DriverId = driverId,
                            FirstName = "Marko",
                            LastName = "Totev",
                            LicenseNumber = "BGH132456"
                        },
                    }

                },
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Chicago",
                    OriginState = "IL",
                    DestinationCity = "Avenel",
                    DestinationState = "NJ",
                    Status = LoadStatus.Delivered,
                    Broker = new Broker()
                    {
                        Id = brokerId,
                        FirstName = "Bruce",
                        LastName = "Wayne",
                        CompanyName = "Wayne Enterprises",
                        UserName = "batman",
                        Email = "batman@chucknorris.com",
                        Position = "Broker"
                    },
                    BookedLoad = new BookedLoad()
                    {

                        Dispatcher = new Dispatcher()
                        {
                            Id = dispatcherId,
                            FirstName = "Gosho",
                            LastName = "Geshev",
                            UserName = "geshata",
                            Email = "gosho@goshata.com",
                            Position = "Dispatcher"
                        },
                        DispatcherId = dispatcherId,
                        Driver =  new Driver()
                        {
                            DriverId = driverId,
                            FirstName = "Marko",
                            LastName = "Totev",
                            LicenseNumber = "BGH132456"
                        },
                        DriverId = driverId
                    },
                    DeliveredLoad = new DeliveredLoad
                    {
                        Id = Guid.NewGuid(),
                        DeliveredDate = default,
                        BrokerId = brokerId,
                        DispatcherId = dispatcherId,
                        DriverId = driverId
                    }
                },
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Chicago",
                    OriginState = "IL",
                    DestinationCity = "Pittsburgh",
                    DestinationState = "PA",
                    Status = LoadStatus.Cancelled,
                    Broker = new Broker()
                    {
                        Id = brokerId,
                        FirstName = "Bruce",
                        LastName = "Wayne",
                        CompanyName = "Wayne Enterprises",
                        UserName = "batman",
                        Email = "batman@chucknorris.com",
                        Position = "Broker"
                    }
                }
            };

            _mockLoadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(allLoads);
            _adminProfileService.Setup(s => s.GetAdminInformation(userId)).ReturnsAsync(new AdminProfileViewModel());

            var result = await _adminLoadBoardService.GetLoadBoardManager(userId);


            Assert.That(result.UserId, Is.EqualTo(userId));
            Assert.That(result.CreatedLoads.Count, Is.EqualTo(1));
            Assert.That(result.PostedLoads.Count, Is.EqualTo(1));
            Assert.That(result.BookedLoads.Count, Is.EqualTo(1));
            Assert.That(result.DeliveredLoads.Count, Is.EqualTo(1));
            Assert.That(result.CancelledLoads.Count, Is.EqualTo(1));

            var bookedLoad = result.BookedLoads.First();
            Assert.That(bookedLoad.Broker, Is.Not.Null);
            Assert.That(bookedLoad.Dispatcher, Is.Not.Null);
            Assert.That(bookedLoad.DriverId, Is.Null.Or.Not.Null);

            var deliveredLoad = result.DeliveredLoads.First();
            Assert.That(deliveredLoad.Broker, Is.Not.Null);
            Assert.That(deliveredLoad.Dispatcher, Is.Not.Null);
            Assert.That(deliveredLoad.DriverId, Is.Not.Null);

            Assert.That(result.Profile, Is.Not.Null);
        }

        [Test]
        public async Task GetLoadBoardManager_CorrectlyCategorizesLoads()
        {
            var userId = Guid.NewGuid();

            var brokerId = Guid.NewGuid();
            var dispatcherId = Guid.NewGuid();
            var driverId = Guid.NewGuid();


            var allLoads = new List<Load>
            {
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Chicago",
                    OriginState = "IL",
                    DestinationCity = "Lansing",
                    DestinationState = "MI",
                    Status = LoadStatus.Created,
                    Broker = new Broker()
                    {
                        Id = brokerId,
                        FirstName = "Bruce",
                        LastName = "Wayne",
                        CompanyName = "Wayne Enterprises",
                        UserName = "batman",
                        Email = "batman@chucknorris.com",
                        Position = "Broker"
                    }
                },
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Sacramento",
                    OriginState = "CA",
                    DestinationCity = "Pittsburgh",
                    DestinationState = "PA",
                    Status = LoadStatus.Available,
                    Broker = new Broker()
                    {
                        Id = brokerId,
                        FirstName = "Bruce",
                        LastName = "Wayne",
                        CompanyName = "Wayne Enterprises",
                        UserName = "batman",
                        Email = "batman@chucknorris.com",
                        Position = "Broker"
                    }
                },
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Mattoon",
                    OriginState = "IL",
                    DestinationCity = "Pittsburgh",
                    DestinationState = "PA",
                    Status = LoadStatus.Booked,
                    Broker = new Broker()
                    {
                        Id = brokerId,
                        FirstName = "Bruce",
                        LastName = "Wayne",
                        CompanyName = "Wayne Enterprises",
                        UserName = "batman",
                        Email = "batman@chucknorris.com",
                        Position = "Broker"
                    },
                    BookedLoad = new BookedLoad()
                    {
                        Dispatcher = new Dispatcher()
                        {
                            Id = dispatcherId,
                            FirstName = "Gosho",
                            LastName = "Geshev",
                            UserName = "geshata",
                            Email = "gosho@goshata.com",
                            Position = "Dispatcher"
                        },
                        Driver =  new Driver()
                        {
                            DriverId = driverId,
                            FirstName = "Marko",
                            LastName = "Totev",
                            LicenseNumber = "BGH132456"
                        },
                    }

                },
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Chicago",
                    OriginState = "IL",
                    DestinationCity = "Avenel",
                    DestinationState = "NJ",
                    Status = LoadStatus.Delivered,
                    Broker = new Broker()
                    {
                        Id = brokerId,
                        FirstName = "Bruce",
                        LastName = "Wayne",
                        CompanyName = "Wayne Enterprises",
                        UserName = "batman",
                        Email = "batman@chucknorris.com",
                        Position = "Broker"
                    },
                    BookedLoad = new BookedLoad()
                    {

                        Dispatcher = new Dispatcher()
                        {
                            Id = dispatcherId,
                            FirstName = "Gosho",
                            LastName = "Geshev",
                            UserName = "geshata",
                            Email = "gosho@goshata.com",
                            Position = "Dispatcher"
                        },
                        DispatcherId = dispatcherId,
                        Driver =  new Driver()
                        {
                            DriverId = driverId,
                            FirstName = "Marko",
                            LastName = "Totev",
                            LicenseNumber = "BGH132456"
                        },
                        DriverId = driverId
                    },
                    DeliveredLoad = new DeliveredLoad
                    {
                        Id = Guid.NewGuid(),
                        DeliveredDate = default,
                        BrokerId = brokerId,
                        DispatcherId = dispatcherId,
                        DriverId = driverId
                    }
                },
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Chicago",
                    OriginState = "IL",
                    DestinationCity = "Pittsburgh",
                    DestinationState = "PA",
                    Status = LoadStatus.Cancelled,
                    Broker = new Broker()
                    {
                        Id = brokerId,
                        FirstName = "Bruce",
                        LastName = "Wayne",
                        CompanyName = "Wayne Enterprises",
                        UserName = "batman",
                        Email = "batman@chucknorris.com",
                        Position = "Broker"
                    }
                }
            };

            _mockLoadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(allLoads);
            _adminProfileService.Setup(s => s.GetAdminInformation(userId)).ReturnsAsync(new AdminProfileViewModel());

            var result = await _adminLoadBoardService.GetLoadBoardManager(userId);

            Assert.That(result.CreatedLoads.Count, Is.EqualTo(1));
            Assert.That(result.PostedLoads.Count, Is.EqualTo(1));
            Assert.That(result.BookedLoads.Count, Is.EqualTo(1));
            Assert.That(result.DeliveredLoads.Count, Is.EqualTo(1));
            Assert.That(result.CancelledLoads.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllPostedLoadsAsync_ReturnsOnlyAvailableLoads()
        {
            var userId = Guid.NewGuid();
            var allLoads = new List<Load>
            {
                new Load { Id = Guid.NewGuid(), Status = LoadStatus.Available },
                new Load { Id = Guid.NewGuid(), Status = LoadStatus.Created },
                new Load { Id = Guid.NewGuid(), Status = LoadStatus.Available }
            };

            _mockLoadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(allLoads);

            var result = await _adminLoadBoardService.GetAllPostedLoadsAsync(userId);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(l => l.Status == LoadStatus.Available.ToString()), Is.True);
        }

        [Test]
        public async Task GetAllPostedLoadsAsync_HandlesMissingBrokerDispatcherDriverInfo()
        {
            var userId = Guid.NewGuid();
            var allLoads = new List<Load>
            {
                new Load
                {
                    Id = Guid.NewGuid(),
                    Status = LoadStatus.Available,
                    BrokerId = Guid.Empty,
                    BookedLoad = null
                }
            };

            _mockLoadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(allLoads);

            var result = await _adminLoadBoardService.GetAllPostedLoadsAsync(userId);

            var postedLoad = result.Single();
            Assert.That(postedLoad.BrokerInfo, Is.Null);
            Assert.That(postedLoad.DispatcherInfo, Is.Null);
            Assert.That(postedLoad.DriverInfo, Is.Null);
        }

        [Test]
        public async Task GetAllPostedLoadsAsync_ReturnsEmptyList_WhenNoAvailableLoads()
        {
            var userId = Guid.NewGuid();

            var allLoads = new List<Load>
            {
                new Load { Id = Guid.NewGuid(), Status = LoadStatus.Booked },
                new Load { Id = Guid.NewGuid(), Status = LoadStatus.Created }
            };

            _mockLoadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(allLoads);

            var result = await _adminLoadBoardService.GetAllPostedLoadsAsync(userId);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllPostedLoadsAsync_PopulatesBrokerInfo_WhenBrokerExists()
        {

            var userId = Guid.NewGuid();
            var brokerId = Guid.NewGuid();
            var allLoads = new List<Load>
            {
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Mattoon",
                    OriginState = "IL",
                    DestinationCity = "Pittsburgh",
                    DestinationState = "PA",
                    Status = LoadStatus.Available,
                    BrokerId = brokerId,
                    Broker = new Broker
                    {
                        Id = brokerId,
                        FirstName = "Bruce",
                        LastName = "Wayne",
                        CompanyName = "Wayne Enterprises",
                        UserName = "batman",
                        Email = "batman@chucknorris.com",
                        Position = "Broker",
                        PhoneNumber = "123-456-7890"
                    }
                }
            };

            _mockLoadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(allLoads);

            var result = await _adminLoadBoardService.GetAllPostedLoadsAsync(userId);

            var postedLoad = result.Single();

            Assert.That(postedLoad.BrokerInfo, Is.Not.Null);
            Assert.That(postedLoad.BrokerInfo.BrokerName, Is.EqualTo("Bruce Wayne"));
            Assert.That(postedLoad.BrokerInfo.BrokerEmail, Is.EqualTo("batman@chucknorris.com"));
            Assert.That(postedLoad.BrokerInfo.BrokerPhone, Is.EqualTo("123-456-7890"));
        }

        [Test]
        public async Task GetAllPostedLoadsAsync_IncludesDispatcherInfo_WhenDispatcherExists()
        {
            var userId = Guid.NewGuid();
            var dispatcherId = Guid.NewGuid();
            var allLoads = new List<Load>
            {
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Mattoon",
                    OriginState = "IL",
                    DestinationCity = "Pittsburgh",
                    DestinationState = "PA",
                    Status = LoadStatus.Available,
                    BookedLoad = new BookedLoad
                    {
                        DispatcherId = dispatcherId,
                        Dispatcher = new Dispatcher
                        {
                            Id = dispatcherId,
                            FirstName = "Gosho",
                            LastName = "Geshev",
                            UserName = "geshata",
                            Email = "gosho@goshata.com",
                            Position = "Dispatcher",
                            PhoneNumber = "987-654-3210"
                        }
                    }
                }
            };

            _mockLoadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(allLoads);

            var result = await _adminLoadBoardService.GetAllPostedLoadsAsync(userId);

            var postedLoad = result.Single();
            Assert.That(postedLoad.DispatcherInfo, Is.Not.Null);
            Assert.That(postedLoad.DispatcherInfo.DispatcherName, Is.EqualTo("Gosho Geshev"));
            Assert.That(postedLoad.DispatcherInfo.DispatcherEmail, Is.EqualTo("gosho@goshata.com"));
            Assert.That(postedLoad.DispatcherInfo.DispatcherPhone, Is.EqualTo("987-654-3210"));
        }

        [Test]
        public async Task GetAllPostedLoadsAsync_IncludesDriverInfo_WhenDriverExists()
        {
            var userId = Guid.NewGuid();
            var driverId = Guid.NewGuid();
            var dispatcherId = Guid.NewGuid();
            var allLoads = new List<Load>
            {
                new Load
                {
                    Id = Guid.NewGuid(),
                    OriginCity = "Mattoon",
                    OriginState = "IL",
                    DestinationCity = "Pittsburgh",
                    DestinationState = "PA",
                    Status = LoadStatus.Available,

                    BookedLoad = new BookedLoad()
                    {
                        DispatcherId = dispatcherId,
                        Dispatcher = new Dispatcher()
                        {
                            Id = dispatcherId,
                            FirstName = "Gosho",
                            LastName = "Geshev",
                            UserName = "geshata",
                            Email = "gosho@goshata.com",
                            Position = "Dispatcher"
                        },
                        DriverId = driverId,
                        Driver = new Driver
                        {
                            DriverId = driverId,
                            FirstName = "Marko",
                            LastName = "Totev",
                            LicenseNumber = "BGH132456"
                        }
                    }
                }
            };

            _mockLoadHelperService.Setup(s => s.GetAllLoads()).ReturnsAsync(allLoads);

            var result = await _adminLoadBoardService.GetAllPostedLoadsAsync(userId);

            var postedLoad = result.Single();

            Assert.That(postedLoad.DriverInfo, Is.Not.Null);
            Assert.That(postedLoad.DriverInfo.DriverName, Is.EqualTo("Marko Totev"));
            Assert.That(postedLoad.DriverInfo.DriverLicenseNumber, Is.EqualTo("BGH132456"));
        }



    }

}
