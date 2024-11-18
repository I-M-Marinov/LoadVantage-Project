using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LoadVantage.Core.Hubs
{
	[Authorize]
	public class ChatHub : Hub
	{
		public async Task SendMessage(string receiverId, string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				throw new ArgumentException("Message cannot be empty.", nameof(message));
			}

			await Clients.User(receiverId).SendAsync("ReceiveMessage", new { Message = message });
		}

		public async Task SendNotification(string receiverId, string notification)
		{
			if (string.IsNullOrWhiteSpace(notification))
			{
				throw new ArgumentException("Notification cannot be empty.", nameof(notification));
			}

			await Clients.User(receiverId).SendAsync("ReceiveNotification", notification);
		}
	}
}