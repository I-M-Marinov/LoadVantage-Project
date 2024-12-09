using Microsoft.AspNetCore.SignalR;

using Moq;
using NUnit.Framework;

using LoadVantage.Core.Hubs;
using LoadVantage.Core.Models.Chat;

namespace LoadVantage.Tests.UnitTests.Core.Hubs
{
	[TestFixture]
	public class ChatHubTests
	{
		private ChatHub _chatHub;
		private Mock<IHubCallerClients> _mockClients;
		private Mock<TestChatClientProxy> _mockSenderClient;
		private Mock<TestChatClientProxy> _mockReceiverClient;

		[SetUp]
		public void SetUp()
		{
			_mockClients = new Mock<IHubCallerClients>();
			_mockSenderClient = new Mock<TestChatClientProxy> { CallBase = true };
			_mockReceiverClient = new Mock<TestChatClientProxy> { CallBase = true };

			
			_mockClients.Setup(clients => clients.User("senderId")).Returns(_mockSenderClient.Object);
			_mockClients.Setup(clients => clients.User("receiverId")).Returns(_mockReceiverClient.Object);

			_chatHub = new ChatHub
			{
				Clients = _mockClients.Object
			};
		}

		[Test]
		public async Task SendMessage_ShouldSendMessageToSenderAndReceiver()
		{

			var senderId = Guid.NewGuid();
			var receiverId = Guid.NewGuid();
			var message = "Hello, this is a test message!";
			var expectedMessage = new ChatMessageViewModel
			{
				SenderId = senderId,
				ReceiverId = receiverId,
				Content = message,
				Timestamp = DateTime.UtcNow
			};

			_mockClients.Setup(clients => clients.User(senderId.ToString()))
				.Returns(_mockSenderClient.Object);
			_mockClients.Setup(clients => clients.User(receiverId.ToString()))
				.Returns(_mockReceiverClient.Object);


			await _chatHub.SendMessage(senderId.ToString(), receiverId.ToString(), message);

			Assert.That(_mockSenderClient.Object.SentMessages.ContainsKey("ReceiveMessage"));
			Assert.That(_mockSenderClient.Object.SentMessages["ReceiveMessage"], Is.TypeOf<ChatMessageViewModel>());
			var senderMessage = (ChatMessageViewModel)_mockSenderClient.Object.SentMessages["ReceiveMessage"];
			Assert.That(senderMessage.Content, Is.EqualTo(expectedMessage.Content));
			Assert.That(senderMessage.SenderId, Is.EqualTo(expectedMessage.SenderId));
			Assert.That(senderMessage.ReceiverId, Is.EqualTo(expectedMessage.ReceiverId));

			Assert.That(_mockReceiverClient.Object.SentMessages.ContainsKey("ReceiveMessage"));
			Assert.That(_mockReceiverClient.Object.SentMessages["ReceiveMessage"], Is.TypeOf<ChatMessageViewModel>());
			var receiverMessage = (ChatMessageViewModel)_mockReceiverClient.Object.SentMessages["ReceiveMessage"];
			Assert.That(receiverMessage.Content, Is.EqualTo(expectedMessage.Content));
			Assert.That(receiverMessage.SenderId, Is.EqualTo(expectedMessage.SenderId));
			Assert.That(receiverMessage.ReceiverId, Is.EqualTo(expectedMessage.ReceiverId));
		}

	}

	// Custom TestClientProxy to intercept SendAsync calls
	public class TestChatClientProxy : IClientProxy
	{
		public Dictionary<string, object> SentMessages { get; } = new Dictionary<string, object>();

		public Task SendCoreAsync(string method, object[] args, CancellationToken cancellationToken = default)
		{
			SentMessages[method] = args.Length > 0 ? args[0] : null;
			return Task.CompletedTask;
		}
	}
}
