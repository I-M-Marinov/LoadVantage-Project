using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

using Moq;
using NUnit.Framework;

using LoadVantage.Core.Hubs;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Core.Models.Chat;
using LoadVantage.Core.Models.Profile;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LoadVantage.Tests.Core.Services
{
	public class ChatServiceTests
	{
		private Mock<IHubContext<ChatHub>> _chatHubContext;
		private LoadVantageDbContext _dbContext;
		private Mock<IUserService> _userService;
		private Mock<IProfileService> _profileService;
		private Mock<IHtmlSanitizerService> _htmlSanitizerService;
		private ChatService _chatService;

		[SetUp]
		public void SetUp()
		{
			_chatHubContext = new Mock<IHubContext<ChatHub>>();

			var mockClients = new Mock<IHubClients>();
			var mockClientProxy = new Mock<IClientProxy>();

			mockClients
				.Setup(clients => clients.User(It.IsAny<string>()))
				.Returns(mockClientProxy.Object);

			_chatHubContext
				.Setup(hub => hub.Clients)
				.Returns(mockClients.Object);

			var options = new DbContextOptionsBuilder<LoadVantageDbContext>()
				.UseInMemoryDatabase(databaseName: "LoadVantageTestInMemoryDB")
				.Options;

			_dbContext = new LoadVantageDbContext(options);
			_profileService = new Mock<IProfileService>();
			_userService = new Mock<IUserService>();
			_htmlSanitizerService = new Mock<IHtmlSanitizerService>();

			_chatService = new ChatService(_chatHubContext.Object, _dbContext, _userService.Object, _profileService.Object, _htmlSanitizerService.Object);
		}

		[TearDown]
		public void TearDown()
		{
			_dbContext.Database.EnsureDeleted();
			_dbContext.Dispose();
		}

		[Test]
		public async Task GetChatUsersAsync_ShouldReturnChatUsers_WhenMessagesExist()
		{
			var currentUserId = Guid.NewGuid();
			var otherUserId = Guid.NewGuid();

			var currentUser = new User
			{
				Id = currentUserId,
				FirstName = "Current",
				LastName = "User",
				Email = "currentUser@gmail.com",
				UserName = "current_user",
				CompanyName = "Test Co.",
				PhoneNumber = "123456789"
			};
			var otherUser = new User
			{
				Id = otherUserId,
				FirstName = "Other",
				LastName = "User",
				Email = "otherUser@gmail.com",
				UserName = "other_user",
				CompanyName = "Other Co.",
				PhoneNumber = "987654321"
			};

			var chatMessage = new ChatMessage
			{
				SenderId = currentUserId, 
				ReceiverId = otherUserId,
				Message = "Bruce Wayne is Batman !"
			};

			await _dbContext.Users.AddRangeAsync(currentUser, otherUser);
			await _dbContext.ChatMessages.AddAsync(chatMessage);
			await _dbContext.SaveChangesAsync();

			var result = await _chatService.GetChatUsersAsync(currentUserId);

			Assert.That(result, Has.Count.EqualTo(1));
			Assert.That(result[0].Id, Is.EqualTo(otherUserId));
			Assert.That(result[0].FullName, Is.EqualTo("Other User"));
			Assert.That(result[0].Company, Is.EqualTo("Other Co."));
			Assert.That(result[0].PhoneNumber, Is.EqualTo("987654321"));
		}

		[Test]
		public async Task GetChatUsersAsync_ShouldIncludeNewChatUser_WhenIncludeNewChatIsTrue()
		{
			var currentUserId = Guid.NewGuid();
			var newChatUserId = Guid.NewGuid();

			var currentUser = new User
			{
				Id = currentUserId,
				FirstName = "Current",
				LastName = "User",
				Email = "currentUser@gmail.com",
				UserName = "current_user",
				CompanyName = "Test Co.",
				PhoneNumber = "123456789"
			};
			var newChatUser = new User
			{
				Id = newChatUserId,
				FirstName = "New Chat",
				LastName = "User",
				Email = "otherUser@gmail.com",
				UserName = "other_user",
				CompanyName = "New Co.",
				PhoneNumber = "111222333"
			};

			await _dbContext.Users.AddRangeAsync(currentUser, newChatUser);
			await _dbContext.SaveChangesAsync();

			var result = await _chatService.GetChatUsersAsync(currentUserId, includeNewChat: true, newChatUserId: newChatUserId);

			Assert.That(result, Has.Count.EqualTo(1));
			Assert.That(result[0].Id, Is.EqualTo(newChatUserId));
			Assert.That(result[0].FullName, Is.EqualTo("New Chat User"));
			Assert.That(result[0].Company, Is.EqualTo("New Co."));
			Assert.That(result[0].PhoneNumber, Is.EqualTo("111222333"));
		}

		[Test]
		public async Task GetChatUsersAsync_ShouldReturnEmptyList_WhenNoMessagesExist()
		{ 
			var currentUserId = Guid.NewGuid();

			var result = await _chatService.GetChatUsersAsync(currentUserId);

			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task GetChatUsersAsync_ShouldRemoveDuplicateUsers_WhenIncludeNewChatIsTrue()
		{
			var currentUserId = Guid.NewGuid();
			var otherUserId = Guid.NewGuid();

			var currentUser = new User
			{
				Id = currentUserId,
				FirstName = "Current",
				LastName = "User",
				Email = "currentUser@gmail.com",
				UserName = "current_user",
				CompanyName = "Test Co.",
				PhoneNumber = "123456789"
			};
			var otherUser = new User
			{
				Id = otherUserId,
				FirstName = "Other",
				LastName = "User",
				Email = "otherUser@gmail.com",
				UserName = "other_user",
				CompanyName = "Other Co.",
				PhoneNumber = "987654321"
			};

			var chatMessage = new ChatMessage
			{
				SenderId = currentUserId, 
				ReceiverId = otherUserId,
				Message = "The future is a foreign land is awesome"
			};

			await _dbContext.Users.AddRangeAsync(currentUser, otherUser);
			await _dbContext.ChatMessages.AddAsync(chatMessage);
			await _dbContext.SaveChangesAsync();

			var result = await _chatService.GetChatUsersAsync(currentUserId, includeNewChat: true, newChatUserId: otherUserId);

			Assert.That(result, Has.Count.EqualTo(1));
			Assert.That(result[0].Id, Is.EqualTo(otherUserId));
			Assert.That(result[0].FullName, Is.EqualTo("Other User"));
		}

		[Test]
		public async Task GetMessagesAsync_ShouldReturnMessagesBetweenSenderAndReceiverInOrder()
		{
			var senderId = Guid.NewGuid();
			var receiverId = Guid.NewGuid();

			var messages = new List<ChatMessage>
			{
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = senderId,
					ReceiverId = receiverId,
					Message = "Message 1",
					Timestamp = new DateTime(2023, 1, 1, 10, 0, 0),
					IsRead = true
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = receiverId,
					ReceiverId = senderId,
					Message = "Message 2",
					Timestamp = new DateTime(2023, 1, 1, 10, 5, 0),
					IsRead = false
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = senderId,
					ReceiverId = receiverId,
					Message = "Message 3",
					Timestamp = new DateTime(2023, 1, 1, 10, 10, 0),
					IsRead = false
				}
			};

			await _dbContext.ChatMessages.AddRangeAsync(messages);
			await _dbContext.SaveChangesAsync();

			var result = await _chatService.GetMessagesAsync(receiverId, senderId);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(3));
			Assert.That(result.First().Content, Is.EqualTo("Message 1"));
			Assert.That(result.Last().Content, Is.EqualTo("Message 3"));
			Assert.That(result.OrderBy(m => m.Timestamp), Is.EqualTo(result));
			Assert.That(result.Any(m => m.IsRead == false), Is.True);
		}

		[Test]
		public async Task GetMessagesAsync_ShouldReturnEmptyList_WhenNoMessagesExist()
		{
			var senderId = Guid.NewGuid();
			var receiverId = Guid.NewGuid();

			var result = await _chatService.GetMessagesAsync(senderId, receiverId);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(0));
		}

		[Test]
		public async Task GetMessagesAsync_ShouldReturnOnlyMessagesBetweenSpecifiedUsers()
		{
			var senderId = Guid.NewGuid();
			var receiverId = Guid.NewGuid();
			var otherUserId = Guid.NewGuid();

			var messages = new List<ChatMessage>
			{
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = senderId,
					ReceiverId = receiverId,
					Message = "Message 1",
					Timestamp = DateTime.UtcNow
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = otherUserId,
					ReceiverId = senderId,
					Message = "Whateveeeeeer",
					Timestamp = DateTime.UtcNow
				}
			};

			await _dbContext.ChatMessages.AddRangeAsync(messages);
			await _dbContext.SaveChangesAsync();

			var result = await _chatService.GetMessagesAsync(senderId, receiverId);

			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result.First().Content, Is.EqualTo("Message 1"));
		}

		[Test]
		public async Task GetMessagesAsync_ShouldReturnMessagesInChronologicalOrder()
		{
			var senderId = Guid.NewGuid();
			var receiverId = Guid.NewGuid();

			var messages = new List<ChatMessage>
			{
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = senderId,
					ReceiverId = receiverId,
					Message = "Message 2",
					Timestamp = new DateTime(2023, 1, 1, 10, 5, 0)
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = receiverId,
					ReceiverId = senderId,
					Message = "Message 1",
					Timestamp = new DateTime(2023, 1, 1, 10, 0, 0)
				}
			};

			await _dbContext.ChatMessages.AddRangeAsync(messages);
			await _dbContext.SaveChangesAsync();

			var result = await _chatService.GetMessagesAsync(senderId, receiverId);

			Assert.That(result.FirstOrDefault(model => model.SenderId == senderId).Content, Is.EqualTo("Message 2"));
			Assert.That(result.FirstOrDefault(model => model.SenderId == receiverId).Content, Is.EqualTo("Message 1"));
		}

		[Test]
		public async Task GetMessagesAsync_ShouldHandleLargeNumberOfMessages()
		{
			var senderId = Guid.NewGuid();
			var receiverId = Guid.NewGuid();

			var messages = Enumerable.Range(1, 10000).Select(i => new ChatMessage
			{
				Id = Guid.NewGuid(),
				SenderId = i % 2 == 0 ? senderId : receiverId,
				ReceiverId = i % 2 == 0 ? receiverId : senderId,
				Message = $"Message {i}",
				Timestamp = DateTime.UtcNow.AddMinutes(i)
			}).ToList();

			await _dbContext.ChatMessages.AddRangeAsync(messages);
			await _dbContext.SaveChangesAsync();
			
			var result = await _chatService.GetMessagesAsync(senderId, receiverId);

			Assert.That(result.Count, Is.EqualTo(10000));
			Assert.That(result.First().Content, Is.EqualTo("Message 1"));
			Assert.That(result.Last().Content, Is.EqualTo("Message 10000"));
		}

		[Test]
		public async Task SendMessageAsync_ShouldSaveMessageToDatabase()
		{
			var senderId = Guid.NewGuid();
			var receiverId = Guid.NewGuid();
			var content = "Test message";

			_htmlSanitizerService
				.Setup(s => s.Sanitize(It.IsAny<string>()))
				.Returns((string input) => input);

			await _chatService.SendMessageAsync(senderId, receiverId, content);

			var savedMessage = await _dbContext.ChatMessages.FirstOrDefaultAsync();

			Assert.That(savedMessage, Is.Not.Null);
			Assert.That(savedMessage.SenderId, Is.EqualTo(senderId));
			Assert.That(savedMessage.ReceiverId, Is.EqualTo(receiverId));
			Assert.That(savedMessage.Message, Is.EqualTo(content));
			Assert.That(savedMessage.Timestamp, Is.Not.EqualTo(default(DateTime)));
		}

		[Test]
		public async Task SendMessageAsync_ShouldSanitizeContent()
		{
			var senderId = Guid.NewGuid();
			var receiverId = Guid.NewGuid();
			var content = "<script>alert('XSS');</script>";

			_htmlSanitizerService
				.Setup(s => s.Sanitize(It.IsAny<string>()))
				.Returns("Sanitized Content");

			await _chatService.SendMessageAsync(senderId, receiverId, content);

			var savedMessage = await _dbContext.ChatMessages.FirstOrDefaultAsync();

			Assert.That(savedMessage, Is.Not.Null);
			Assert.That(savedMessage.Message, Is.EqualTo("Sanitized Content"));
		}

		[Test]
		public async Task SendMessageAsync_ShouldSendMessageToSender()
		{
			// 
			var senderId = Guid.NewGuid();
			var receiverId = Guid.NewGuid();

			var content = "Test message";

			_htmlSanitizerService
				.Setup(s => s.Sanitize(It.IsAny<string>()))
				.Returns(content);

			var mockClients = new Mock<IHubClients>();
			var mockSenderClientProxy = new Mock<IClientProxy>();
			var mockReceiverClientProxy = new Mock<IClientProxy>();

			mockClients
				.Setup(clients => clients.User(senderId.ToString().Trim()))
				.Returns(mockSenderClientProxy.Object);

			mockClients
				.Setup(clients => clients.User(receiverId.ToString().Trim()))
				.Returns(mockReceiverClientProxy.Object);

			_chatHubContext
				.Setup(hub => hub.Clients)
				.Returns(mockClients.Object);

			// Act
			await _chatService.SendMessageAsync(senderId, receiverId, content);

			mockSenderClientProxy.Verify(
				proxy => proxy.SendCoreAsync(
					"ReceiveMessage",
					It.Is<object[]>(args =>
						args.Length == 1 &&
						args[0] is ChatMessageViewModel &&
						((ChatMessageViewModel)args[0]).SenderId == senderId &&
						((ChatMessageViewModel)args[0]).ReceiverId == receiverId &&
						((ChatMessageViewModel)args[0]).Content == content
					),
					default
				),
				Times.Once
			);

			mockReceiverClientProxy.Verify(
				proxy => proxy.SendCoreAsync(
					"ReceiveMessage",
					It.Is<object[]>(args =>
						args.Length == 1 &&
						args[0] is ChatMessageViewModel &&
						((ChatMessageViewModel)args[0]).SenderId == senderId &&
						((ChatMessageViewModel)args[0]).ReceiverId == receiverId &&
						((ChatMessageViewModel)args[0]).Content == content
					),
					default
				),
				Times.Once
			);
		}

		[Test]
		public async Task SendMessageAsync_ShouldSendMessageToReceiver()
		{
			var senderId = Guid.NewGuid();
			var receiverId = Guid.NewGuid();
			var content = "Test message";

			_htmlSanitizerService
				.Setup(s => s.Sanitize(It.IsAny<string>()))
				.Returns(content);

			var mockClients = new Mock<IHubClients>();
			var mockSenderClientProxy = new Mock<IClientProxy>();
			var mockReceiverClientProxy = new Mock<IClientProxy>();

			mockClients
				.Setup(clients => clients.User(senderId.ToString().Trim()))
				.Returns(mockSenderClientProxy.Object);

			mockClients
				.Setup(clients => clients.User(receiverId.ToString().Trim()))
				.Returns(mockReceiverClientProxy.Object);

			_chatHubContext
				.Setup(hub => hub.Clients)
				.Returns(mockClients.Object);

			await _chatService.SendMessageAsync(senderId, receiverId, content);

			mockReceiverClientProxy.Verify(
				proxy => proxy.SendCoreAsync(
					"ReceiveMessage",
					It.Is<object[]>(args =>
						args.Length == 1 &&
						args[0] is ChatMessageViewModel &&
						((ChatMessageViewModel)args[0]).SenderId == senderId &&
						((ChatMessageViewModel)args[0]).ReceiverId == receiverId &&
						((ChatMessageViewModel)args[0]).Content == content
					),
					default
				),
				Times.Once
			);

			mockSenderClientProxy.Verify(
				proxy => proxy.SendCoreAsync(
					"ReceiveMessage",
					It.Is<object[]>(args =>
						args.Length == 1 &&
						args[0] is ChatMessageViewModel &&
						((ChatMessageViewModel)args[0]).SenderId == senderId &&
						((ChatMessageViewModel)args[0]).ReceiverId == receiverId &&
						((ChatMessageViewModel)args[0]).Content == content
					),
					default
				),
				Times.Once
			);
		}

		[Test]
		public async Task GetUnreadMessagesAsync_ShouldReturnUnreadMessagesForUser()
		{
			
			var userId = Guid.NewGuid();
			var otherUserId = Guid.NewGuid();

			var messages = new List<ChatMessage>
			{
				new ChatMessage
				{
					Id = Guid.NewGuid(), 
					ReceiverId = userId, 
					IsRead = false, 
					Timestamp = DateTime.UtcNow.AddMinutes(-1),
					Message = "Boss ?"
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(), 
					ReceiverId = userId, 
					IsRead = false, 
					Timestamp = DateTime.UtcNow.AddMinutes(-2),
					Message = "Your"
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(), 
					ReceiverId = userId, 
					IsRead = true, 
					Timestamp = DateTime.UtcNow.AddMinutes(-3),
					Message = "Is"
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(), 
					ReceiverId = otherUserId, 
					IsRead = false, 
					Timestamp = DateTime.UtcNow.AddMinutes(-4),
					Message = "Who"
				}
			};


			await _dbContext.ChatMessages.AddRangeAsync(messages);
			await _dbContext.SaveChangesAsync();


			var (unreadMessages, unreadCount) = await _chatService.GetUnreadMessagesAsync(userId);

			Assert.That(2, Is.EqualTo(unreadCount));
			Assert.That(2, Is.EqualTo(unreadMessages.Count));
			Assert.That(unreadMessages.All(m => m.ReceiverId == userId));
			Assert.That(unreadMessages.All(m => !m.IsRead));
			Assert.That(unreadMessages, Is.Ordered.Descending.By("Timestamp"));
		}

		[Test]
		public async Task MarkMessagesAsReadAsync_ShouldMarkUnreadMessagesAsRead()
		{
			var senderId = Guid.NewGuid();
			var receiverId = Guid.NewGuid();

			var messages = new List<ChatMessage>
			{
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = senderId,
					ReceiverId = receiverId,
					IsRead = false,
					Timestamp = DateTime.UtcNow.AddMinutes(-1),
					Message = "Boss ?"
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = senderId,
					ReceiverId = receiverId,
					IsRead = false,
					Timestamp = DateTime.UtcNow.AddMinutes(-2),
					Message = "Your"
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = senderId,
					ReceiverId = receiverId,
					IsRead = true,
					Timestamp = DateTime.UtcNow.AddMinutes(-3),
					Message = "Is"
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = receiverId,
					ReceiverId = senderId,
					IsRead = false,
					Timestamp = DateTime.UtcNow.AddMinutes(-4),
					Message = "Who"
				}
			};

			await _dbContext.ChatMessages.AddRangeAsync(messages);
			await _dbContext.SaveChangesAsync();


			await _chatService.MarkMessagesAsReadAsync(senderId, receiverId);

			var updatedMessages = await _dbContext.ChatMessages
				.Where(m => m.SenderId == senderId && m.ReceiverId == receiverId)
				.ToListAsync();

			Assert.That(updatedMessages.Count(m => !m.IsRead), Is.EqualTo(0));
			Assert.That(updatedMessages.Count(m => m.IsRead), Is.EqualTo(3));
		}

		[Test]
		public async Task GetLastChatAsync_ShouldReturnLastChatMessageForUser()
		{
			var userId = Guid.NewGuid();
			var otherUserId = Guid.NewGuid();

			var messages = new List<ChatMessage>
			{
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = userId,
					ReceiverId = otherUserId,
					IsRead = false,
					Timestamp = DateTime.UtcNow.AddMinutes(-5),
					Message = "Third Message"
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = otherUserId,
					ReceiverId = userId,
					IsRead = false,
					Timestamp = DateTime.UtcNow.AddMinutes(-10),
					Message = "First Message"
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = userId,
					ReceiverId = otherUserId,
					IsRead = false,
					Timestamp = DateTime.UtcNow.AddMinutes(-2),
					Message = "Second Message"
				},
				new ChatMessage
				{
					Id = Guid.NewGuid(),
					SenderId = otherUserId,
					ReceiverId = userId,
					IsRead = false,
					Timestamp = DateTime.UtcNow.AddMinutes(-1),
					Message = "Last Message"
				}
			};

			await _dbContext.ChatMessages.AddRangeAsync(messages);
			await _dbContext.SaveChangesAsync();

			var lastChat = await _chatService.GetLastChatAsync(userId);

			Assert.That(lastChat, Is.Not.Null);
			Assert.That(lastChat.SenderId, Is.EqualTo(userId).Or.EqualTo(otherUserId));
			Assert.That(lastChat.Timestamp, Is.EqualTo(messages.OrderByDescending(m => m.Timestamp).First().Timestamp));
		}

		[Test]
		public async Task GetLastChatAsync_ShouldReturnNullWhenNoMessagesExist()
		{
			var userId = Guid.NewGuid();

			var lastChat = await _chatService.GetLastChatAsync(userId);

			Assert.That(lastChat, Is.Null);
		}
		[Test]
		public async Task BuildChatViewModel_ShouldReturnValidChatViewModel()
		{
			var currentUserId = Guid.NewGuid();
			var userId = Guid.NewGuid();

			_dbContext.Users.AddRange(
				new User
				{
					Id = currentUserId, 
					FirstName = "Current", 
					LastName = "User",
					Email = "user1@mail.bg",
					UserName = "username1"
				},
				new User
				{
					Id = userId, 
					FirstName = "John", 
					LastName = "Doe",
					Email = "user2@mail.bg",
					UserName = "username2"
				}
			);

			_dbContext.ChatMessages.Add(new ChatMessage
			{
				Id = Guid.NewGuid(),
				SenderId = currentUserId,
				ReceiverId = userId,
				Message = "Hello!",
				Timestamp = DateTime.UtcNow,
				IsRead = false
			});

			await _dbContext.SaveChangesAsync();

			var profile = new ProfileViewModel
			{
				Id = currentUserId.ToString(),
				FirstName = "Current",
				LastName = "User",
				UserImageUrl = "profileurl"
			};

			var userInfo = new UserChatViewModel
			{
				FullName = "John Doe",
				ProfilePictureUrl = "url"
			};

			_userService.Setup(s => s.GetCurrentUserAsync())
				.ReturnsAsync(new User { Id = currentUserId });

			_userService.Setup(s => s.GetChatUserInfoAsync(userId))
				.ReturnsAsync(userInfo);

			_profileService.Setup(s => s.GetUserInformation(currentUserId))
				.ReturnsAsync(profile);

			var chatViewModel = await _chatService.BuildChatViewModel(userId);


			Assert.That(chatViewModel, Is.Not.Null);
			Assert.That(chatViewModel.CurrentChatUserId, Is.EqualTo(userId));
			Assert.That(chatViewModel.Messages, Has.Count.EqualTo(1));
			Assert.That(chatViewModel.Messages[0].Content, Is.EqualTo("Hello!"));
			Assert.That(chatViewModel.UserInfo, Is.EqualTo(userInfo));
			Assert.That(chatViewModel.Profile, Is.EqualTo(profile));
		}

		[Test]
		public async Task BuildChatViewModel_ShouldReturnEmptyUsersWhenNoChatUsersFound()
		{

			var userId = Guid.NewGuid();
			var currentUserId = Guid.NewGuid();

			var chatUsers = new List<UserChatViewModel>(); // No chat users

			var userInfo = new UserChatViewModel
			{
				FullName = "John Doe", 
				ProfilePictureUrl = "url"
			};

			var messages = new List<ChatMessageViewModel>(); // No messages

			var profile = new ProfileViewModel
			{
				Id = currentUserId.ToString(), 
				FirstName = "Bruce",
				LastName = "Wayne",
				UserImageUrl = "/wwwroot/images/batman.gif"
			};

			var chatServiceMock = new Mock<IChatService>();

			// Mock service calls
			_userService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(new User { Id = currentUserId });
			_userService.Setup(s => s.GetChatUserInfoAsync(userId)).ReturnsAsync(userInfo);
			chatServiceMock.Setup(s => s.GetMessagesAsync(currentUserId, userId)).ReturnsAsync(messages);
			_profileService.Setup(s => s.GetUserInformation(currentUserId)).ReturnsAsync(profile);


			var chatViewModel = await _chatService.BuildChatViewModel(userId);

			Assert.That(chatViewModel.Users, Is.Empty);
		}

		[Test]
		public async Task BuildChatViewModel_ShouldHandleNullValues()
		{

			var userId = Guid.NewGuid();
			var currentUserId = Guid.NewGuid();

			var chatServiceMock = new Mock<IChatService>();

			var profile = new ProfileViewModel(); // empty profile
			var chatUser = new UserChatViewModel(); 

			_userService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(new User { Id = currentUserId });

			_userService.Setup(s => s.GetChatUserInfoAsync(userId)).ReturnsAsync(chatUser);

			chatServiceMock.Setup(s => s.GetMessagesAsync(currentUserId, userId)).ReturnsAsync((List<ChatMessageViewModel>)null);

			_profileService.Setup(s => s.GetUserInformation(currentUserId)).ReturnsAsync(profile);

			var chatViewModel = await _chatService.BuildChatViewModel(userId);

			Assert.That(chatViewModel.UserInfo.Id, Is.Empty);
			Assert.That(chatViewModel.Profile.Id, Is.Null);
			Assert.That(chatViewModel.Messages.Count, Is.Zero);
		}



	}
}
