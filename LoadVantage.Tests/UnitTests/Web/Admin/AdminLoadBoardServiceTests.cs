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

		[Test]
		public void GetCreatedLoads_ReturnsEmptyList_WhenNoLoadsProvided()
		{
			var allLoads = new List<Load>();

			var result = _adminLoadBoardService.GetCreatedLoads(allLoads);

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void GetCreatedLoads_ReturnsEmptyList_WhenNoCreatedLoadsExist()
		{
			var allLoads = new List<Load>
			{
				new Load { Status = LoadStatus.Available },
				new Load { Status = LoadStatus.Booked }
			};

			var result = _adminLoadBoardService.GetCreatedLoads(allLoads);

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void GetCreatedLoads_ReturnsOnlyCreatedLoads()
		{
			var allLoads = new List<Load>
			{
				new Load { Id = Guid.NewGuid(), Status = LoadStatus.Created },
				new Load { Id = Guid.NewGuid(), Status = LoadStatus.Available },
				new Load { Id = Guid.NewGuid(), Status = LoadStatus.Created }
			};

			var result = _adminLoadBoardService.GetCreatedLoads(allLoads);

			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result.All(l => l.Status == LoadStatus.Created.ToString()));
		}

		[Test]
		public void GetCreatedLoads_ReturnsLoadsOrderedByPickupTimeThenByOriginCityAndState()
		{
			var allLoads = new List<Load>
			{
				new Load { Id = Guid.NewGuid(), Status = LoadStatus.Created, PickupTime = DateTime.Now.AddHours(2), OriginCity = "CityB", OriginState = "StateB" },
				new Load { Id = Guid.NewGuid(), Status = LoadStatus.Created, PickupTime = DateTime.Now, OriginCity = "CityA", OriginState = "StateA" },
				new Load { Id = Guid.NewGuid(), Status = LoadStatus.Created, PickupTime = DateTime.Now, OriginCity = "CityA", OriginState = "StateB" }
			};

			var result = _adminLoadBoardService.GetCreatedLoads(allLoads);

			Assert.That(result[0].PickupTime, Is.LessThanOrEqualTo(result[1].PickupTime));
			Assert.That(result[1].PickupTime, Is.LessThanOrEqualTo(result[2].PickupTime));
			Assert.That(result[0].OriginCity, Is.LessThanOrEqualTo(result[1].OriginCity));
			Assert.That(result[1].OriginState, Is.GreaterThanOrEqualTo(result[2].OriginState));
		}

		[Test]
		public void GetCreatedLoads_ReturnsCorrectAdminLoadViewModelProperties()
		{
			var broker = new Broker
			{
                FirstName = "Gosho",
                LastName = "Broker",
                UserName = "gosho",
				Email = "gosho@otpochivka.com", 
				PhoneNumber = "1234567890"
			};
			var dispatcher = new Dispatcher
			{
				FirstName = "Gosho",
				LastName = "Dispatcher",
				Email = "gosho@dispatcher.com", 
				PhoneNumber = "0987654321"
			};
			var driver = new Driver
			{
				FirstName = "Gosho",
				LastName = "Driver",
				LicenseNumber = "DL12345"
			};

			var allLoads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Created,
					OriginCity = "Sacramento",
					OriginState = "CA",
					DestinationCity = "Chicago",
					DestinationState = "IL",
					PickupTime = DateTime.Now,
					DeliveryTime = DateTime.Now.AddHours(5),
					Price = 1000m,
					Distance = 500,
					Weight = 1500,
					BrokerId = Guid.NewGuid(),
					Broker = broker,
					BookedLoad = new BookedLoad
					{
						DispatcherId = Guid.NewGuid(),
						Dispatcher = dispatcher,
						DriverId = Guid.NewGuid(),
						Driver = driver
					}
				}
			};

			var result = _adminLoadBoardService.GetCreatedLoads(allLoads);

			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[0].OriginCity, Is.EqualTo("Sacramento"));
			Assert.That(result[0].OriginState, Is.EqualTo("CA"));
			Assert.That(result[0].DestinationCity, Is.EqualTo("Chicago"));
			Assert.That(result[0].DestinationState, Is.EqualTo("IL"));
			Assert.That(result[0].PickupTime, Is.Not.Null);
			Assert.That(result[0].DeliveryTime, Is.Not.Null);
			Assert.That(result[0].PostedPrice, Is.EqualTo(1000m));
			Assert.That(result[0].Distance, Is.EqualTo(500));
			Assert.That(result[0].Weight, Is.EqualTo(1500));
			Assert.That(result[0].BrokerInfo.BrokerName, Is.EqualTo("Gosho Broker"));
			Assert.That(result[0].BrokerInfo.BrokerEmail, Is.EqualTo("gosho@otpochivka.com"));
			Assert.That(result[0].BrokerInfo.BrokerPhone, Is.EqualTo("1234567890"));
			Assert.That(result[0].DispatcherInfo.DispatcherName, Is.EqualTo("Gosho Dispatcher"));
			Assert.That(result[0].DispatcherInfo.DispatcherEmail, Is.EqualTo("gosho@dispatcher.com"));
			Assert.That(result[0].DispatcherInfo.DispatcherPhone, Is.EqualTo("0987654321"));
			Assert.That(result[0].DriverInfo.DriverName, Is.EqualTo("Gosho Driver"));
			Assert.That(result[0].DriverInfo.DriverLicenseNumber, Is.EqualTo("DL12345"));
		}

		[Test]
		public void GetPostedLoads_ReturnsEmptyList_WhenNoLoadsProvided()
		{
			var allLoads = new List<Load>();

			var result = _adminLoadBoardService.GetPostedLoads(allLoads);

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void GetPostedLoads_ReturnsEmptyList_WhenNoAvailableLoadsExist()
		{
			var allLoads = new List<Load>
			{
				new Load { Status = LoadStatus.Created },
				new Load { Status = LoadStatus.Booked }
			};

			var result = _adminLoadBoardService.GetPostedLoads(allLoads);

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void GetPostedLoads_ReturnsOnlyAvailableLoads()
		{
			var allLoads = new List<Load>
			{
				new Load { Id = Guid.NewGuid(), Status = LoadStatus.Available },
				new Load { Id = Guid.NewGuid(), Status = LoadStatus.Created },
				new Load { Id = Guid.NewGuid(), Status = LoadStatus.Available }
			};

			var result = _adminLoadBoardService.GetPostedLoads(allLoads);

			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result.All(l => l.Status == LoadStatus.Available.ToString()));
		}

		[Test]
		public void GetPostedLoads_ReturnsLoadsOrderedByPickupTimeThenByOriginCityAndState()
		{
			var allLoads = new List<Load>
			{
				new Load { Id = Guid.NewGuid(), Status = LoadStatus.Available, PickupTime = DateTime.Now.AddHours(2), OriginCity = "CityB", OriginState = "StateB" },
				new Load { Id = Guid.NewGuid(), Status = LoadStatus.Available, PickupTime = DateTime.Now, OriginCity = "CityA", OriginState = "StateA" },
				new Load { Id = Guid.NewGuid(), Status = LoadStatus.Available, PickupTime = DateTime.Now, OriginCity = "CityA", OriginState = "StateB" }
			};

			var result = _adminLoadBoardService.GetPostedLoads(allLoads);

			Assert.That(result[0].PickupTime, Is.LessThanOrEqualTo(result[1].PickupTime));
			Assert.That(result[1].PickupTime, Is.LessThanOrEqualTo(result[2].PickupTime));
			Assert.That(result[0].OriginCity, Is.LessThanOrEqualTo(result[1].OriginCity));
			Assert.That(result[1].OriginState, Is.GreaterThanOrEqualTo(result[2].OriginState));
		}

		[Test]
		public void GetPostedLoads_ReturnsCorrectAdminLoadViewModelProperties()
		{
			var broker = new Broker
			{
				FirstName = "Bruce",
				LastName = "Wayne",
				UserName = "batman",
				Email = "batman@chucknorris.com",
				PhoneNumber = "+1-800-659-9634"
			};

			var dispatcher = new Dispatcher
			{
				FirstName = "Chuck",
				LastName = "Norris",
				UserName = "chucky",
				Email = "gmail@chucknorris.com",
				PhoneNumber = "+1-630-159-9224"
			};

			var driver = new Driver
			{
				FirstName = "Bruce",
				LastName = "Wayne",
				LicenseNumber = "DL54321"
			};

			var allLoads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Available,
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now,
					DeliveryTime = DateTime.Now.AddHours(5),
					Price = 2000m,
					Distance = 600,
					Weight = 1800,
					BrokerId = Guid.NewGuid(),
					Broker = broker,
					BookedLoad = new BookedLoad
					{
						DispatcherId = Guid.NewGuid(),
						Dispatcher = dispatcher,
						DriverId = Guid.NewGuid(),
						Driver = driver
					}
				}
			};

			var result = _adminLoadBoardService.GetPostedLoads(allLoads);

			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[0].OriginCity, Is.EqualTo("CityA"));
			Assert.That(result[0].OriginState, Is.EqualTo("StateA"));
			Assert.That(result[0].DestinationCity, Is.EqualTo("CityB"));
			Assert.That(result[0].DestinationState, Is.EqualTo("StateB"));
			Assert.That(result[0].PickupTime, Is.Not.Null);
			Assert.That(result[0].DeliveryTime, Is.Not.Null);
			Assert.That(result[0].PostedPrice, Is.EqualTo(2000m));
			Assert.That(result[0].Distance, Is.EqualTo(600));
			Assert.That(result[0].Weight, Is.EqualTo(1800));

			Assert.That(result[0].BrokerInfo.BrokerName, Is.EqualTo("Bruce Wayne"));
			Assert.That(result[0].BrokerInfo.BrokerEmail, Is.EqualTo("batman@chucknorris.com"));
			Assert.That(result[0].BrokerInfo.BrokerPhone, Is.EqualTo("+1-800-659-9634"));

			Assert.That(result[0].DispatcherInfo.DispatcherName, Is.EqualTo("Chuck Norris"));
			Assert.That(result[0].DispatcherInfo.DispatcherEmail, Is.EqualTo("gmail@chucknorris.com"));
			Assert.That(result[0].DispatcherInfo.DispatcherPhone, Is.EqualTo("+1-630-159-9224"));

			Assert.That(result[0].DriverInfo.DriverName, Is.EqualTo("Bruce Wayne"));
			Assert.That(result[0].DriverInfo.DriverLicenseNumber, Is.EqualTo("DL54321"));
		}

		[Test]
		public void GetBookedLoads_ReturnsCorrectAdminLoadViewModelProperties()
		{
			var broker = new Broker
			{
				FirstName = "Bruce",
				LastName = "Wayne",
				UserName = "batman",
				Email = "batman@chucknorris.com",
				PhoneNumber = "+1-800-659-9634"
			};

			var dispatcher = new Dispatcher
			{
				FirstName = "Chuck",
				LastName = "Norris",
				UserName = "chucky",
				Email = "gmail@chucknorris.com",
				PhoneNumber = "+1-630-159-9224"
			};

			var driver = new Driver
			{
				FirstName = "Bruce",
				LastName = "Wayne",
				LicenseNumber = "DL54321"
			};

			var allLoads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Booked,
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now,
					DeliveryTime = DateTime.Now.AddHours(5),
					Price = 1500m,
					Distance = 500,
					Weight = 2000,
					BrokerId = Guid.NewGuid(),
					Broker = broker,
					BookedLoad = new BookedLoad
					{
						DispatcherId = Guid.NewGuid(),
						Dispatcher = dispatcher,
						DriverId = Guid.NewGuid(),
						Driver = driver
					}
				},
		        new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Available,
					OriginCity = "CityC",
					OriginState = "StateC",
					DestinationCity = "CityD",
					DestinationState = "StateD",
					PickupTime = DateTime.Now.AddHours(1),
					DeliveryTime = DateTime.Now.AddHours(6),
					Price = 2000m,
					Distance = 700,
					Weight = 1800,
					BrokerId = Guid.NewGuid(),
					Broker = broker
				}
			};

			var result = _adminLoadBoardService.GetBookedLoads(allLoads);

			Assert.That(result.Count, Is.EqualTo(1));

			var bookedLoad = result[0];
			Assert.That(bookedLoad.Status, Is.EqualTo("Booked"));
			Assert.That(bookedLoad.OriginCity, Is.EqualTo("CityA"));
			Assert.That(bookedLoad.OriginState, Is.EqualTo("StateA"));
			Assert.That(bookedLoad.DestinationCity, Is.EqualTo("CityB"));
			Assert.That(bookedLoad.DestinationState, Is.EqualTo("StateB"));
			Assert.That(bookedLoad.PickupTime, Is.Not.Null);
			Assert.That(bookedLoad.DeliveryTime, Is.Not.Null);
			Assert.That(bookedLoad.PostedPrice, Is.EqualTo(1500m));
			Assert.That(bookedLoad.Distance, Is.EqualTo(500));
			Assert.That(bookedLoad.Weight, Is.EqualTo(2000));

			Assert.That(bookedLoad.BrokerInfo.BrokerName, Is.EqualTo("Bruce Wayne"));
			Assert.That(bookedLoad.BrokerInfo.BrokerEmail, Is.EqualTo("batman@chucknorris.com"));
			Assert.That(bookedLoad.BrokerInfo.BrokerPhone, Is.EqualTo("+1-800-659-9634"));

			Assert.That(result[0].DispatcherInfo.DispatcherName, Is.EqualTo("Chuck Norris"));
			Assert.That(result[0].DispatcherInfo.DispatcherEmail, Is.EqualTo("gmail@chucknorris.com"));
			Assert.That(result[0].DispatcherInfo.DispatcherPhone, Is.EqualTo("+1-630-159-9224"));


			Assert.That(bookedLoad.DriverInfo.DriverName, Is.EqualTo("Bruce Wayne"));
			Assert.That(bookedLoad.DriverInfo.DriverLicenseNumber, Is.EqualTo("DL54321"));
		}

		[Test]
		public void GetBookedLoads_ReturnsLoadsOrderedByPickupTimeOriginCityAndState()
		{
			var broker = new Broker 
			{ 
				FirstName = "Bruce", 
				LastName = "Wayne", 
				UserName = "batman", 
				Email = "batman@chucknorris.com", 
				PhoneNumber = "+1-800-659-9634"
			};

			var dispatcher = new Dispatcher 
			{ 
				FirstName = "Bruce", 
				LastName = "Wayne", 
				UserName = "batman", 
				Email = "batman@chucknorris.com", 
				PhoneNumber = "+1-800-659-9634"
			};

			var driver = new Driver 
			{ 
				FirstName = "Bruce", 
				LastName = "Wayne", 
				LicenseNumber = "DL54321"
			};

			var allLoads = new List<Load>
	{
		new Load
		{
			Id = Guid.NewGuid(),
			Status = LoadStatus.Booked,
			OriginCity = "CityA",
			OriginState = "StateA",
			DestinationCity = "CityB",
			DestinationState = "StateB",
			PickupTime = DateTime.Now.AddHours(1),
			DeliveryTime = DateTime.Now.AddHours(6),
			Price = 1500m,
			Distance = 500,
			Weight = 2000,
			BrokerId = Guid.NewGuid(),
			Broker = broker,
			BookedLoad = new BookedLoad { DispatcherId = Guid.NewGuid(), Dispatcher = dispatcher, DriverId = Guid.NewGuid(), Driver = driver }
		},
		new Load
		{
			Id = Guid.NewGuid(),
			Status = LoadStatus.Booked,
			OriginCity = "CityB",
			OriginState = "StateB",
			DestinationCity = "CityC",
			DestinationState = "StateC",
			PickupTime = DateTime.Now,
			DeliveryTime = DateTime.Now.AddHours(5),
			Price = 2000m,
			Distance = 700,
			Weight = 1800,
			BrokerId = Guid.NewGuid(),
			Broker = broker,
			BookedLoad = new BookedLoad { DispatcherId = Guid.NewGuid(), Dispatcher = dispatcher, DriverId = Guid.NewGuid(), Driver = driver }
		}
	};

			var result = _adminLoadBoardService.GetBookedLoads(allLoads);

			// Verify the loads are ordered by PickupTime, OriginCity, and OriginState
			Assert.That(result[0].PickupTime, Is.LessThan(result[1].PickupTime));
			Assert.That(result[0].OriginCity, Is.EqualTo("CityB"));
			Assert.That(result[1].OriginCity, Is.EqualTo("CityA"));
		}

		[Test]
		public void GetBookedLoads_ReturnsEmptyListWhenNoBookedLoads()
		{
			var allLoads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Available,
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now,
					DeliveryTime = DateTime.Now.AddHours(5),
					Price = 1500m,
					Distance = 500,
					Weight = 2000
				}
			};

			var result = _adminLoadBoardService.GetBookedLoads(allLoads);

			// Verify that no booked loads are returned
			Assert.That(result.Count, Is.EqualTo(0));
		}

		[Test]
		public void GetBookedLoads_HandlesEmptyBrokerInfo()
		{
			var dispatcher = new Dispatcher
			{
				FirstName = "Bruce", 
				LastName = "Wayne", 
				UserName = "batman", 
				Email = "batman@chucknorris.com", 
				PhoneNumber = "+1-800-659-9634"
			};

			var driver = new Driver
			{
				FirstName = "Bruce", 
				LastName = "Wayne", 
				LicenseNumber = "DL54321"
			};

			var allLoads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Booked,
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now,
					DeliveryTime = DateTime.Now.AddHours(5),
					Price = 1500m,
					Distance = 500,
					Weight = 2000,
					BrokerId = Guid.Empty, 
					BookedLoad = new BookedLoad
					{
						DispatcherId = Guid.NewGuid(),
						Dispatcher = dispatcher, 
						DriverId = Guid.NewGuid(), 
						Driver = driver
					}
				}
			};

			var result = _adminLoadBoardService.GetBookedLoads(allLoads);

			Assert.That(result[0].BrokerInfo, Is.Null);
		}

		[Test]
		public void GetBookedLoads_HandlesMultipleBookedLoadsWithDifferentDispatchersAndDrivers()
		{
			var broker = new Broker
			{
				FirstName = "Bruce", 
				LastName = "Wayne", 
				UserName = "batman", 
				Email = "batman@chucknorris.com", 
				PhoneNumber = "+1-800-659-9634"
			};

			var dispatcher1 = new Dispatcher
			{
				FirstName = "Dispatcher", 
				LastName = "One", 
				UserName = "dispatcher1", 
				Email = "dispatcher1@chucknorris.com", 
				PhoneNumber = "+1-800-123-4567"
			};

			var driver1 = new Driver
			{
				DriverId = Guid.NewGuid(), 
				FirstName = "Driver",
				LastName = "One", 
				LicenseNumber = "DL12345"
			};

			var dispatcher2 = new Dispatcher
			{
				FirstName = "Dispatcher", 
				LastName = "Two", 
				UserName = "dispatcher2", 
				Email = "dispatcher2@chucknorris.com", 
				PhoneNumber = "+1-800-234-5678"
			};

			var driver2 = new Driver
			{
				DriverId = Guid.NewGuid(), 
				FirstName = "Driver", 
				LastName = "Two", 
				LicenseNumber = "DL67890"
			};

			var allLoads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Booked,
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now,
					DeliveryTime = DateTime.Now.AddHours(5),
					Price = 1500m,
					Distance = 500,
					Weight = 2000,
					BrokerId = Guid.NewGuid(),
					Broker = broker,
					BookedLoad = new BookedLoad { DispatcherId = dispatcher1.Id, Dispatcher = dispatcher1, DriverId = driver1.DriverId, Driver = driver1 }
				},
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Booked,
					OriginCity = "CityC",
					OriginState = "StateC",
					DestinationCity = "CityD",
					DestinationState = "StateD",
					PickupTime = DateTime.Now.AddHours(1),
					DeliveryTime = DateTime.Now.AddHours(6),
					Price = 2000m,
					Distance = 700,
					Weight = 1800,
					BrokerId = Guid.NewGuid(),
					Broker = broker,
					BookedLoad = new BookedLoad { DispatcherId = dispatcher2.Id, Dispatcher = dispatcher2, DriverId = driver2.DriverId, Driver = driver2 }
				}
			};

			var result = _adminLoadBoardService.GetBookedLoads(allLoads);

			Assert.That(result[0].DispatcherInfo.DispatcherName, Is.EqualTo(dispatcher1.FullName));
			Assert.That(result[0].DriverInfo.DriverName, Is.EqualTo(driver1.FullName));
			Assert.That(result[1].DispatcherInfo.DispatcherName, Is.EqualTo(dispatcher2.FullName));
			Assert.That(result[1].DriverInfo.DriverName, Is.EqualTo(driver2.FullName));
		}

		[Test]
		public void GetDeliveredLoads_FiltersAndSortsDeliveredLoads()
		{
			var broker = new Broker 
			{ 
				Id = Guid.NewGuid(), 
				FirstName = "Bruce",
				LastName = "Wayne",
				Email = "batman@chucknorris.com", 
				PhoneNumber = "+1-800-659-9634"
			};

			var dispatcher = new Dispatcher 
			{ 
				Id = Guid.NewGuid(), 
				FirstName = "Alfred", 
				LastName = "Pennyworth",
				Email = "alfred@wayneenterprise.com", 
				PhoneNumber = "+1-800-555-1234"
			};

			var driver = new Driver 
			{
				DriverId = Guid.NewGuid(), 
				FirstName = "Dick",
				LastName = "Grayson",
				LicenseNumber = "XYZ12345"
			};

			var allLoads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Delivered,
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now.AddHours(2),
					DeliveryTime = DateTime.Now.AddHours(1),
					Price = 1500m,
					Distance = 500,
					Weight = 44000,
					BrokerId = broker.Id,
					Broker = broker,
					DeliveredLoad = new DeliveredLoad
					{
						DeliveredDate = DateTime.Now
					},
					BookedLoad = new BookedLoad
					{
						DispatcherId = dispatcher.Id,
						Dispatcher = dispatcher,
						DriverId = driver.DriverId,
						Driver = driver
					}
				},
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Delivered,
					OriginCity = "CityC",
					OriginState = "StateC",
					DestinationCity = "CityD",
					DestinationState = "StateD",
					PickupTime = DateTime.Now.AddHours(4),
					DeliveryTime = DateTime.Now.AddHours(3),
					Price = 2000m,
					Distance = 600,
					Weight = 50000,
					BrokerId = broker.Id,
					Broker = broker,
					DeliveredLoad = new DeliveredLoad
					{
						DeliveredDate = DateTime.Now
					},
					BookedLoad = new BookedLoad
					{
						DispatcherId = dispatcher.Id,
						Dispatcher = dispatcher,
						DriverId = driver.DriverId,
						Driver = driver
					}
				}
			};

			var result = _adminLoadBoardService.GetDeliveredLoads(allLoads);

			Assert.That(result.All(load => load.Status == "Delivered"), Is.True);
			Assert.That(result[0].DeliveryTime < result[1].DeliveryTime);
		}

		[Test]
		public void GetDeliveredLoads_ReturnsEmptyList_WhenNoDeliveredLoads()
		{
			var allLoads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Booked, 
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now.AddHours(2),
					DeliveryTime = DateTime.Now.AddHours(1),
					Price = 1500m,
					Distance = 500,
					Weight = 44000,
					BrokerId = Guid.NewGuid(),
					Broker = new Broker
					{
						Id = Guid.NewGuid(),
						FirstName = "Bruce",
						LastName = "Wayne",
						Email = "batman@chucknorris.com",
						PhoneNumber = "+1-800-659-9634"
					}
				}
			};

			var result = _adminLoadBoardService.GetDeliveredLoads(allLoads);

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void GetDeliveredLoads_PopulatesFieldsCorrectly()
		{

			var broker = new Broker
			{
				Id = Guid.NewGuid(),
				FirstName = "Bruce",
				LastName = "Wayne",
				Email = "batman@chucknorris.com",
				PhoneNumber = "+1-800-659-9634"
			};

			var dispatcher = new Dispatcher
			{
				Id = Guid.NewGuid(),
				FirstName = "Alfred",
				LastName = "Pennyworth",
				Email = "alfred@wayneenterprise.com",
				PhoneNumber = "+1-800-555-1234"
			};

			var driver = new Driver
			{
				DriverId = Guid.NewGuid(),
				FirstName = "Dick",
				LastName = "Grayson",
				LicenseNumber = "XYZ12345"
			};

			var allLoads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Delivered,
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now.AddHours(2),
					DeliveryTime = DateTime.Now.AddHours(1),
					Price = 1500m,
					Distance = 500,
					Weight = 44000,
					BrokerId = broker.Id,
					Broker = broker,
					DeliveredLoad = new DeliveredLoad { DeliveredDate = DateTime.Now },
					BookedLoad = new BookedLoad
					{
						DispatcherId = dispatcher.Id,
						Dispatcher = dispatcher,
						DriverId = driver.DriverId,
						Driver = driver
					}
				}
			};

			var result = _adminLoadBoardService.GetDeliveredLoads(allLoads);

			Assert.That(result[0].BrokerInfo.BrokerName, Is.EqualTo("Bruce Wayne"));
			Assert.That(result[0].BrokerInfo.BrokerEmail, Is.EqualTo("batman@chucknorris.com"));
			Assert.That(result[0].BrokerInfo.BrokerPhone, Is.EqualTo("+1-800-659-9634"));

			Assert.That(result[0].DispatcherInfo.DispatcherName, Is.EqualTo("Alfred Pennyworth"));
			Assert.That(result[0].DispatcherInfo.DispatcherEmail, Is.EqualTo("alfred@wayneenterprise.com"));
			Assert.That(result[0].DispatcherInfo.DispatcherPhone, Is.EqualTo("+1-800-555-1234"));

			Assert.That(result[0].DriverInfo.DriverName, Is.EqualTo("Dick Grayson"));
			Assert.That(result[0].DriverInfo.DriverLicenseNumber, Is.EqualTo("XYZ12345"));
		}

		[Test]
		public void GetCancelledLoads_ReturnsCorrectLoads()
		{
			var loads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Cancelled,
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now.AddDays(-2),
					DeliveryTime = DateTime.Now.AddDays(-1),
					Price = 100.0m,
					Distance = 500,
					Weight = 1500,
					BrokerId = Guid.NewGuid(),
					Broker = new Broker
					{
						FirstName = "Bruce", 
						LastName = "Wayne", 
						UserName = "batman", 
						Email = "batman@chucknorris.com", 
						PhoneNumber = "+1-800-659-9634"
					}
				},
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Cancelled,
					OriginCity = "CityC",
					OriginState = "StateC",
					DestinationCity = "CityD",
					DestinationState = "StateD",
					PickupTime = DateTime.Now.AddDays(-1),
					DeliveryTime = DateTime.Now,
					Price = 200.0m,
					Distance = 700,
					Weight = 1800,
					BrokerId = Guid.NewGuid(),
					Broker = new Broker
					{
						FirstName = "Clark", 
						LastName = "Kent", 
						UserName = "superman", 
						Email = "superman@dailyplanet.com", 
						PhoneNumber = "+1-800-555-1234"
					}
				},
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Available,
					OriginCity = "CityE",
					OriginState = "StateE",
					DestinationCity = "CityF",
					DestinationState = "StateF",
					PickupTime = DateTime.Now.AddDays(-3),
					DeliveryTime = DateTime.Now.AddDays(-2),
					Price = 150.0m,
					Distance = 600,
					Weight = 1700,
					BrokerId = Guid.NewGuid(),
					Broker = new Broker
					{
						FirstName = "Diana", 
						LastName = "Prince", 
						UserName = "wonderwoman", 
						Email = "wonderwoman@amazon.com", 
						PhoneNumber = "+1-800-555-6789"
					}
				}
			};

			var cancelledLoads = _adminLoadBoardService.GetCancelledLoads(loads);

			Assert.That(cancelledLoads.Count, Is.EqualTo(2));
			Assert.That(cancelledLoads.All(l => l.Status == LoadStatus.Cancelled.ToString()), Is.True);
			Assert.That(cancelledLoads[0].PickupTime, Is.LessThan(cancelledLoads[1].PickupTime));
			Assert.That(cancelledLoads[0].Broker.FirstName, Is.EqualTo("Bruce"));
			Assert.That(cancelledLoads[0].Broker.LastName, Is.EqualTo("Wayne"));
			Assert.That(cancelledLoads[0].Broker.UserName, Is.EqualTo("batman"));
			Assert.That(cancelledLoads[0].Broker.Email, Is.EqualTo("batman@chucknorris.com"));
			Assert.That(cancelledLoads[0].Broker.PhoneNumber, Is.EqualTo("+1-800-659-9634"));
			Assert.That(cancelledLoads[1].Broker.FirstName, Is.EqualTo("Clark"));
			Assert.That(cancelledLoads[1].Broker.LastName, Is.EqualTo("Kent"));
			Assert.That(cancelledLoads[1].Broker.UserName, Is.EqualTo("superman"));
			Assert.That(cancelledLoads[1].Broker.Email, Is.EqualTo("superman@dailyplanet.com"));
			Assert.That(cancelledLoads[1].Broker.PhoneNumber, Is.EqualTo("+1-800-555-1234"));
		}

		[Test]
		public void GetCancelledLoads_AreOrderedByPickupTime()
		{
			var loads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Cancelled,
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now.AddDays(-2),
					DeliveryTime = DateTime.Now.AddDays(-1),
					Price = 100.0m,
					Distance = 500,
					Weight = 1500,
					BrokerId = Guid.NewGuid(),
					Broker = new Broker
					{
						FirstName = "Bruce", 
						LastName = "Wayne", 
						UserName = "batman", 
						Email = "batman@chucknorris.com", 
						PhoneNumber = "+1-800-659-9634"
					}
				},
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Cancelled,
					OriginCity = "CityC",
					OriginState = "StateC",
					DestinationCity = "CityD",
					DestinationState = "StateD",
					PickupTime = DateTime.Now.AddDays(-1),
					DeliveryTime = DateTime.Now,
					Price = 200.0m,
					Distance = 700,
					Weight = 1800,
					BrokerId = Guid.NewGuid(),
					Broker = new Broker
					{
						FirstName = "Clark", 
						LastName = "Kent", 
						UserName = "superman", 
						Email = "superman@dailyplanet.com", 
						PhoneNumber = "+1-800-555-1234"
					}
				}
			};

			var cancelledLoads = _adminLoadBoardService.GetCancelledLoads(loads);

			Assert.That(cancelledLoads[0].PickupTime, Is.LessThan(cancelledLoads[1].PickupTime));
		}

		[Test]
		public void GetCancelledLoads_OnlyReturnsCancelledLoads()
		{
			var loads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Cancelled,
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now.AddDays(-2),
					DeliveryTime = DateTime.Now.AddDays(-1),
					Price = 100.0m,
					Distance = 500,
					Weight = 1500,
					BrokerId = Guid.NewGuid(),
					Broker = new Broker
					{
						FirstName = "Bruce", 
						LastName = "Wayne", 
						UserName = "batman", 
						Email = "batman@chucknorris.com", 
						PhoneNumber = "+1-800-659-9634"
					}
				},
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Booked,
					OriginCity = "CityC",
					OriginState = "StateC",
					DestinationCity = "CityD",
					DestinationState = "StateD",
					PickupTime = DateTime.Now.AddDays(-1),
					DeliveryTime = DateTime.Now,
					Price = 200.0m,
					Distance = 700,
					Weight = 1800,
					BrokerId = Guid.NewGuid(),
					Broker = new Broker
					{
						FirstName = "Clark", 
						LastName = "Kent", 
						UserName = "superman", 
						Email = "superman@dailyplanet.com", 
						PhoneNumber = "+1-800-555-1234"
					}
				}
			};

			var cancelledLoads = _adminLoadBoardService.GetCancelledLoads(loads);

			Assert.That(cancelledLoads.Count, Is.EqualTo(1));
			Assert.That(cancelledLoads[0].Status, Is.EqualTo(LoadStatus.Cancelled.ToString()));
		}

		[Test]
		public void GetCancelledLoads_PopulatesBrokerInfo()
		{
			var loads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Cancelled,
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now.AddDays(-2),
					DeliveryTime = DateTime.Now.AddDays(-1),
					Price = 100.0m,
					Distance = 500,
					Weight = 1500,
					BrokerId = Guid.NewGuid(),
					Broker = new Broker { FirstName = "Bruce", LastName = "Wayne", UserName = "batman", Email = "batman@chucknorris.com", PhoneNumber = "+1-800-659-9634" }
				}
			};

			var cancelledLoads = _adminLoadBoardService.GetCancelledLoads(loads);

			Assert.That(cancelledLoads[0].BrokerInfo, Is.Not.Null);
			Assert.That(cancelledLoads[0].BrokerInfo.BrokerName, Is.EqualTo("Bruce Wayne"));
			Assert.That(cancelledLoads[0].BrokerInfo.BrokerEmail, Is.EqualTo("batman@chucknorris.com"));
			Assert.That(cancelledLoads[0].BrokerInfo.BrokerPhone, Is.EqualTo("+1-800-659-9634"));
		}

		[Test]
		public void GetCancelledLoads_DoesNotPopulateBrokerInfo_WhenBrokerIdIsEmpty()
		{
			var loads = new List<Load>
			{
				new Load
				{
					Id = Guid.NewGuid(),
					Status = LoadStatus.Cancelled,
					OriginCity = "CityA",
					OriginState = "StateA",
					DestinationCity = "CityB",
					DestinationState = "StateB",
					PickupTime = DateTime.Now.AddDays(-2),
					DeliveryTime = DateTime.Now.AddDays(-1),
					Price = 100.0m,
					Distance = 500,
					Weight = 1500,
					BrokerId = Guid.Empty,
					Broker = null
				}
			};

			var cancelledLoads = _adminLoadBoardService.GetCancelledLoads(loads);

			Assert.That(cancelledLoads[0].BrokerInfo, Is.Null);
		}

	}

}
