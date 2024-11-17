using LoadVantage.Core.Contracts;
using Microsoft.AspNetCore.SignalR;
using LoadVantage.Core.Hubs;

namespace LoadVantage.Core.Services
{
	public class ChatService : IChatService
	{
		private readonly IHubContext<ChatHub> _chatHub;

		public ChatService(IHubContext<ChatHub> chatHub)
		{
			_chatHub = chatHub;
		}

		public async Task SendMessageAsync(string senderId, string receiverId, string message)
		{
			// Send message to the specific user via SignalR
			await _chatHub.Clients.User(receiverId).SendAsync("ReceiveMessage", new
			{
				SenderId = senderId,
				Message = message,
				Timestamp = DateTime.Now
			});
		}
	}
}
