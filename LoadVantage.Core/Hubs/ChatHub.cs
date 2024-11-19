using Azure.Messaging;
using LoadVantage.Core.Models.Chat;
using LoadVantage.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LoadVantage.Core.Hubs
{
	[Authorize]
	public class ChatHub : Hub
	{
		public async Task SendMessage(string senderId, string receiverId, string message)
		{
			var chatMessageViewModel = new ChatMessageViewModel
			{
				SenderId = Guid.Parse(senderId),
				ReceiverId = Guid.Parse(receiverId),
				Content = message,
				Timestamp = DateTime.UtcNow
			};


			// Send the message to both users
			await Clients.User(senderId).SendAsync("ReceiveMessage", chatMessageViewModel);
			await Clients.User(receiverId).SendAsync("ReceiveMessage", chatMessageViewModel);
		}

	}
}