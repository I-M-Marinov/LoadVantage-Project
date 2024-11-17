using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LoadVantage.Core.Hubs
{
	[Authorize]
	public class ChatHub : Hub
	{
		// Send a message to a specific user
		public async Task SendMessage(string receiverId, string message)
		{
			// Send message to the receiver
			await Clients.User(receiverId).SendAsync("ReceiveMessage", message);
		}

		// Send a notification to all connected clients
		public async Task SendNotification(string user, string notification)
		{
			await Clients.All.SendAsync("ReceiveNotification", user, notification);
		}
	}
}