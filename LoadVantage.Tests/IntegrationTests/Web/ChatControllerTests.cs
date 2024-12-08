using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;
using NUnit.Framework;

using LoadVantage.Controllers;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Chat;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Infrastructure.Data.Models;
using UserChatViewModel = LoadVantage.Core.Models.Chat.UserChatViewModel;

using static LoadVantage.Common.GeneralConstants.TempMessages;

namespace LoadVantage.Tests.IntegrationTests.Web
{
	public class ChatControllerTests
	{
		private Mock<IChatService> mockChatService;
		private Mock<IUserService> mockUserService;
		private Mock<IProfileService> mockProfileService;
		private ChatController controller;
		private Guid currentUserId;

		[SetUp]
		public void SetUp()
		{
			mockChatService = new Mock<IChatService>();
			mockUserService = new Mock<IUserService>();
			mockProfileService = new Mock<IProfileService>();

			controller = new ChatController
				(mockChatService.Object, 
					mockUserService.Object, 
					mockProfileService.Object);

			currentUserId = Guid.NewGuid();

			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.NameIdentifier, currentUserId.ToString())
			}));

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};
		}

		[Test]
		public async Task SendMessage_ValidModel_SendsMessageAndReturnsChatWindowView()
		{
			var receiverId = Guid.NewGuid();
			var messageContent = "Test message";

			var chatMessages = new List<ChatMessageViewModel>
			{
				new ChatMessageViewModel
				{
					Id = Guid.NewGuid(),
					SenderId = currentUserId,
					ReceiverId = receiverId,
					Content = "Hello!",
					Timestamp = DateTime.Now,
					IsRead = true
				}
			};

			var chatUsers = new List<UserChatViewModel>
			{
				new UserChatViewModel
				{
					Id = receiverId,
					FullName = "Receiver User",
					ProfilePictureUrl = "http://example.com/profile.jpg",
					PhoneNumber = "123-456-7890",
					Company = "Test Company",
					Position = "Dispatcher"
				}
			};

			mockChatService.Setup(service => service.SendMessageAsync(currentUserId, receiverId, messageContent))
				.Returns(Task.CompletedTask);

			mockChatService.Setup(service => service.GetChatUsersAsync(currentUserId, false, null))
				.ReturnsAsync(chatUsers);

			mockChatService.Setup(service => service.GetMessagesAsync(currentUserId, receiverId))
				.ReturnsAsync(chatMessages);

			mockUserService.Setup(service => service.GetChatUserInfoAsync(receiverId))
				.ReturnsAsync(new UserChatViewModel() { FullName = "Receiver", ProfilePictureUrl = "url" });

			mockProfileService.Setup(service => service.GetUserInformation(currentUserId))
				.ReturnsAsync(new ProfileViewModel() { FirstName = "Test", LastName = "User"});

			var result = await controller.SendMessage(receiverId, messageContent) as ViewResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result?.ViewName, Is.EqualTo("ChatWindow"));

			var model = result?.Model as ChatViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model?.CurrentChatUserId, Is.EqualTo(receiverId));
			Assert.That(model?.Users, Has.Count.EqualTo(1));
			Assert.That(model?.Users[0].FullName, Is.EqualTo("Receiver User"));
			Assert.That(model?.Users[0].ProfilePictureUrl, Is.EqualTo("http://example.com/profile.jpg"));
			Assert.That(model?.Users[0].PhoneNumber, Is.EqualTo("123-456-7890"));
			Assert.That(model?.Users[0].Company, Is.EqualTo("Test Company"));
			Assert.That(model?.Users[0].Position, Is.EqualTo("Dispatcher"));

			Assert.That(model?.Messages, Has.Count.EqualTo(1));
			Assert.That(model?.Messages[0].Content, Is.EqualTo("Hello!"));
			Assert.That(model?.Messages[0].SenderId, Is.EqualTo(currentUserId));
			Assert.That(model?.Messages[0].ReceiverId, Is.EqualTo(receiverId));
			Assert.That(model?.Messages[0].IsRead, Is.True);

			Assert.That(model?.UserInfo.FullName, Is.EqualTo("Receiver"));
			Assert.That(model?.Profile.FullName, Is.EqualTo("Test User"));
		}

		[Test]
		public async Task ChatWithBrokerWindow_ValidBrokerId_ReturnsChatWindowViewWithModel()
		{
			var brokerId = Guid.NewGuid();
			var expectedChatViewModel = new ChatViewModel
			{
				CurrentChatUserId = brokerId,
				Users = new List<UserChatViewModel>
				{
					new UserChatViewModel
					{
						Id = brokerId,
						FullName = "Test Broker",
						ProfilePictureUrl = "http://example.com/broker.jpg",
						PhoneNumber = "555-555-5555",
						Company = "Broker Company",
						Position = "Broker"
					}
				},
				Messages = new List<ChatMessageViewModel>
				{
					new ChatMessageViewModel
					{
						Id = Guid.NewGuid(),
						SenderId = Guid.NewGuid(),
						ReceiverId = brokerId,
						Content = "Test Message",
						Timestamp = DateTime.UtcNow,
						IsRead = false
					}
				},

				UserInfo = new UserChatViewModel() { FullName = "Test User" },
				Profile = new ProfileViewModel() { FirstName = "Test", LastName = "User" }
			};

			mockChatService.Setup(service => service.BuildChatViewModel(brokerId))
				.ReturnsAsync(expectedChatViewModel);

			var result = await controller.ChatWithBrokerWindow(brokerId) as ViewResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result?.ViewName, Is.EqualTo("ChatWindow"));

			var model = result?.Model as ChatViewModel;

			Assert.That(model, Is.Not.Null);
			Assert.That(model?.CurrentChatUserId, Is.EqualTo(brokerId));
			Assert.That(model?.Users, Has.Count.EqualTo(1));
			Assert.That(model?.Users[0].FullName, Is.EqualTo("Test Broker"));
			Assert.That(model?.Users[0].ProfilePictureUrl, Is.EqualTo("http://example.com/broker.jpg"));
			Assert.That(model?.Users[0].PhoneNumber, Is.EqualTo("555-555-5555"));
			Assert.That(model?.Users[0].Company, Is.EqualTo("Broker Company"));
			Assert.That(model?.Users[0].Position, Is.EqualTo("Broker"));

			Assert.That(model?.Messages, Has.Count.EqualTo(1));
			Assert.That(model?.Messages[0].Content, Is.EqualTo("Test Message"));
			Assert.That(model?.Messages[0].SenderId, Is.Not.EqualTo(brokerId));

			mockChatService.Verify(service => service.BuildChatViewModel(brokerId), Times.Once);
		}

		[Test]
		public async Task ChatWindow_NoRecentChat_ReturnsEmptyModelWithMessage()
		{
			var currentUser = new User { Id = Guid.NewGuid() };

			mockUserService.Setup(service => service.GetCurrentUserAsync())
				.ReturnsAsync(currentUser);

			mockChatService.Setup(service => service.GetLastChatAsync(currentUser.Id))
				.ReturnsAsync((ChatMessage?)null);

			mockProfileService.Setup(service => service.GetUserInformation(currentUser.Id))
				.ReturnsAsync(new ProfileViewModel() { FirstName = "Test", LastName = "User" });

			var result = await controller.ChatWindow() as ViewResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result?.ViewName, Is.EqualTo("ChatWindow"));

			var model = result?.Model as ChatViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model?.Profile?.FullName, Is.EqualTo("Test User"));

			Assert.That(result?.ViewData["Message"], Is.EqualTo(NoRecentChats));
		}

		[Test]
		public async Task ChatWindow_RecentChat_ReturnsChatWindowWithModel()
		{
			var currentUser = new User { Id = Guid.NewGuid() };

			var otherUserId = Guid.NewGuid();

			var lastChat = new ChatMessage
			{
				Id = Guid.NewGuid(),
				SenderId = currentUser.Id,
				ReceiverId = otherUserId,
				Message = "Hello",
				Timestamp = DateTime.UtcNow,
				IsRead = false
			};

			var expectedModel = new ChatViewModel
			{
				CurrentChatUserId = otherUserId,
				Users = new List<UserChatViewModel>
				{
					new UserChatViewModel
					{
						Id = otherUserId,
						FullName = "Other User",
						ProfilePictureUrl = "http://example.com/otheruser.jpg"
					}
				},
				Messages = new List<ChatMessageViewModel>
				{
					new ChatMessageViewModel
					{
						Id = lastChat.Id,
						SenderId = lastChat.SenderId,
						ReceiverId = lastChat.ReceiverId,
						Content = lastChat.Message,
						Timestamp = lastChat.Timestamp,
						IsRead = lastChat.IsRead
					}
				},
				Profile = new ProfileViewModel() { FirstName = "Test", LastName = "User" }
			};

			mockUserService.Setup(service => service.GetCurrentUserAsync())
				.ReturnsAsync(currentUser);

			mockChatService.Setup(service => service.GetLastChatAsync(currentUser.Id))
				.ReturnsAsync(lastChat);

			mockChatService.Setup(service => service.BuildChatViewModel(otherUserId))
				.ReturnsAsync(expectedModel);

			mockProfileService.Setup(service => service.GetUserInformation(currentUser.Id))
				.ReturnsAsync(new ProfileViewModel() { FirstName = "Test", LastName = "User" });

			var result = await controller.ChatWindow() as ViewResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result?.ViewName, Is.EqualTo("ChatWindow"));

			var model = result?.Model as ChatViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model?.Users, Has.Count.EqualTo(1));
			Assert.That(model?.Messages, Has.Count.EqualTo(1));
			Assert.That(model?.Messages[0].Content, Is.EqualTo("Hello"));
			Assert.That(model?.Users[0].FullName, Is.EqualTo("Other User"));

			mockChatService.Verify(service => service.BuildChatViewModel(otherUserId), Times.Once);
		}

		[Test]
		public async Task ChatWindow_LastChatSentByCurrentUser_ReturnsCorrectModel()
		{
			var currentUser = new User { Id = Guid.NewGuid() };
			var otherUserId = Guid.NewGuid();

			var lastChat = new ChatMessage
			{
				Id = Guid.NewGuid(),
				SenderId = currentUser.Id,
				ReceiverId = otherUserId,
				Message = "Test Message",
				Timestamp = DateTime.UtcNow,
				IsRead = false
			};

			var expectedModel = new ChatViewModel
			{
				CurrentChatUserId = otherUserId,
				Users = new List<UserChatViewModel>
				{
					new UserChatViewModel
					{
						Id = otherUserId,
						FullName = "Other User",
						ProfilePictureUrl = "http://example.com/otheruser.jpg"
					}
				},
				Messages = new List<ChatMessageViewModel>
				{
					new ChatMessageViewModel
					{
						Id = lastChat.Id,
						SenderId = lastChat.SenderId,
						ReceiverId = lastChat.ReceiverId,
						Content = lastChat.Message,
						Timestamp = lastChat.Timestamp,
						IsRead = lastChat.IsRead
					}
				},
				Profile = new ProfileViewModel() { FirstName = "Test", LastName = "User" }
			};

			mockUserService.Setup(service => service.GetCurrentUserAsync())
				.ReturnsAsync(currentUser);

			mockChatService.Setup(service => service.GetLastChatAsync(currentUser.Id))
				.ReturnsAsync(lastChat);

			mockChatService.Setup(service => service.BuildChatViewModel(otherUserId))
				.ReturnsAsync(expectedModel);

			mockProfileService.Setup(service => service.GetUserInformation(currentUser.Id))
				.ReturnsAsync(new ProfileViewModel() { FirstName = "Test", LastName = "User" });

			var result = await controller.ChatWindow() as ViewResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result?.ViewName, Is.EqualTo("ChatWindow"));

			var model = result?.Model as ChatViewModel;
			Assert.That(model, Is.Not.Null);
			Assert.That(model?.Users, Has.Count.EqualTo(1));
			Assert.That(model?.Messages, Has.Count.EqualTo(1));
			Assert.That(model?.Messages[0].Content, Is.EqualTo("Test Message"));
			Assert.That(model?.Users[0].FullName, Is.EqualTo("Other User"));

			mockChatService.Verify(service => service.BuildChatViewModel(otherUserId), Times.Once);
		}

		[Test]
		public async Task GetMessages_ReturnsCorrectMessages()
		{
			var currentUser = new User { Id = Guid.NewGuid() };

			var chatUserId = Guid.NewGuid();

			var messages = new List<ChatMessageViewModel>
			{
				new ChatMessageViewModel()
				{
					Id = Guid.NewGuid(),
					SenderId = currentUser.Id,
					ReceiverId = chatUserId,
					Content = "Hello!",
					Timestamp = DateTime.UtcNow,
					IsRead = false
				},
				new ChatMessageViewModel
				{
					Id = Guid.NewGuid(),
					SenderId = chatUserId,
					ReceiverId = currentUser.Id,
					Content = "Hi there!",
					Timestamp = DateTime.UtcNow,
					IsRead = false
				}
			};

			mockUserService.Setup(service => service.GetCurrentUserAsync())
				.ReturnsAsync(currentUser);

			mockChatService.Setup(service => service.GetMessagesAsync(currentUser.Id, chatUserId))
				.ReturnsAsync(messages);

			var result = await controller.GetMessages(chatUserId) as PartialViewResult;

			
			Assert.That(result, Is.Not.Null);
			Assert.That(result?.ViewName, Is.EqualTo("_ChatMessagesPartialView"));

			var model = result?.Model as List<ChatMessageViewModel>;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Count, Is.EqualTo(2)); 

			Assert.That(model[0].Content, Is.EqualTo("Hello!"));
			Assert.That(model[1].Content, Is.EqualTo("Hi there!"));
			Assert.That(model[0].SenderId, Is.EqualTo(currentUser.Id));
			Assert.That(model[1].SenderId, Is.EqualTo(chatUserId));

			mockChatService.Verify(service => service.GetMessagesAsync(currentUser.Id, chatUserId), Times.Once);
		}

		[Test]
		public async Task GetMessages_NoMessages_ReturnsEmptyModel()
		{
			var currentUser = new User { Id = Guid.NewGuid() };

			var chatUserId = Guid.NewGuid();

			mockUserService.Setup(service => service.GetCurrentUserAsync())
				.ReturnsAsync(currentUser);

			mockChatService.Setup(service => service.GetMessagesAsync(currentUser.Id, chatUserId))
				.ReturnsAsync(new List<ChatMessageViewModel>());

			var result = await controller.GetMessages(chatUserId) as PartialViewResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result?.ViewName, Is.EqualTo("_ChatMessagesPartialView"));

			var model = result?.Model as List<ChatMessageViewModel>;
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Count, Is.EqualTo(0)); 

			mockChatService.Verify(service => service.GetMessagesAsync(currentUser.Id, chatUserId), Times.Once);
		}


		[Test]
		public async Task GetUnreadMessages_NoUnreadMessages_ReturnsEmptyMessages()
		{
			var currentUser = new User { Id = Guid.NewGuid() };

			mockUserService.Setup(service => service.GetCurrentUserAsync())
				.ReturnsAsync(currentUser);

			mockChatService.Setup(service => service.GetUnreadMessagesAsync(currentUser.Id))
				.ReturnsAsync((new List<ChatMessage>(), 0));

			var result = await controller.GetUnreadMessages() as JsonResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result?.Value, Is.Not.Null);

		}

		[Test]
		public async Task GetUserInfo_ReturnsPartialViewWithCorrectUserInfo()
		{
			var userId = Guid.NewGuid();

			var userInfo = new UserChatViewModel
			{
				Id = userId,
				FullName = "TestUser",
				ProfilePictureUrl = "http://example.com/profile.jpg",
			};

			mockUserService.Setup(service => service.GetChatUserInfoAsync(userId))
				.ReturnsAsync(userInfo);

			var result = await controller.GetUserInfo(userId) as PartialViewResult;

			
			Assert.That(result, Is.Not.Null);  

			Assert.That(result.ViewName, Is.EqualTo("_ChatUserInfoPartial"));  

			Assert.That(result.Model, Is.InstanceOf<UserChatViewModel>());

			var returnedUserInfo = result.Model as UserChatViewModel;

			Assert.That(returnedUserInfo, Is.Not.Null);
			Assert.That(returnedUserInfo.Id, Is.EqualTo(userId));
			Assert.That(returnedUserInfo.FullName, Is.EqualTo("TestUser"));
			Assert.That(returnedUserInfo.ProfilePictureUrl, Is.EqualTo("http://example.com/profile.jpg"));

			mockUserService.Verify(service => service.GetChatUserInfoAsync(userId), Times.Once);
		}



	}


}




