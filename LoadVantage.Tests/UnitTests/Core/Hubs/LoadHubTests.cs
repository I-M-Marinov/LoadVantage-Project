using Microsoft.AspNetCore.SignalR;

using Moq;
using NUnit.Framework;

using LoadVantage.Core.Hubs;
using LoadVantage.Core.Models.Load;

namespace LoadVantage.Tests.UnitTests.Core.Hubs
{
	[TestFixture]
	public class LoadHubTests
	{
		private LoadHub _loadHub;
		private Mock<IHubCallerClients> _mockClients;
		private Mock<TestClientProxy> _mockAllClients;

		[SetUp]
		public void SetUp()
		{
			_mockClients = new Mock<IHubCallerClients>();
			_mockAllClients = new Mock<TestClientProxy> { CallBase = true };

			_mockClients.Setup(clients => clients.All).Returns(_mockAllClients.Object);

			_loadHub = new LoadHub
			{
				Clients = _mockClients.Object
			};
		}

		[Test]
		public async Task SendLoadPostedNotification_ShouldSendLoadIdToAllClients()
		{
			var load = new LoadViewModel
			{
				Id = Guid.NewGuid()
			};

			await _loadHub.SendLoadPostedNotification(load);

			Assert.That(_mockAllClients.Object.SentMessages.ContainsKey("ReceiveLoadPostedNotification"));
			Assert.That(_mockAllClients.Object.SentMessages["ReceiveLoadPostedNotification"], Is.EqualTo(load.Id));
		}

		[Test]
		public async Task SendLoadStatusChangedNotification_ShouldSendReloadMessageToAllClients()
		{
			await _loadHub.SendLoadStatusChangedNotification();

			Assert.That(_mockAllClients.Object.SentMessages.ContainsKey("ReloadPostedLoadsTable"));
			Assert.That(_mockAllClients.Object.SentMessages["ReloadPostedLoadsTable"], Is.Null);
		}
	}

	// Custom TestClientProxy to intercept SendAsync calls
	public class TestClientProxy : IClientProxy
	{
		public Dictionary<string, object> SentMessages { get; } = new Dictionary<string, object>();

		public Task SendCoreAsync(string method, object[] args, CancellationToken cancellationToken = default)
		{
			SentMessages[method] = args.Length > 0 ? args[0] : null;
			return Task.CompletedTask;
		}
	}
}
