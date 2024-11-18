using LoadVantage.Core.Contracts;
using Microsoft.AspNetCore.SignalR;
using LoadVantage.Core.Hubs;
using LoadVantage.Core.Models.Chat;
using LoadVantage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Core.Services
{
	public class ChatService : IChatService
	{
		private readonly IHubContext<ChatHub> chatHub;
		private readonly LoadVantageDbContext context;

		public ChatService(IHubContext<ChatHub> _chatHub, LoadVantageDbContext _dbContext)
		{
			chatHub = _chatHub;
			context = _dbContext;
		}

		public async Task<IEnumerable<ChatUserViewModel>> GetChatUsersAsync(Guid currentUserId,bool includeNewChat = false, Guid? newChatUserId = null)
		{
			var userIds = await context.ChatMessages
				.Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
				.Select(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
				.Distinct()
				.ToListAsync();

			// If a new chat has been created, add the new chat user to the list
			if (includeNewChat && newChatUserId.HasValue)
			{
				userIds.Add(newChatUserId.Value); // Ensure the new user is included
				userIds = userIds.Distinct().ToList(); // Remove any duplicates (if necessary)
			}

			// Fetch the user details for the userIds
			var users = await context.Users
				.Where(u => userIds.Contains(u.Id))
				.Select(u => new ChatUserViewModel
				{
					Id = u.Id,
					Name = u.FullName,
					ProfilePictureUrl = context.UsersImages
						.Where(ui => ui.UserId == u.Id)
						.Select(ui => ui.ImageUrl)
						.FirstOrDefault()!
				})
				.ToListAsync();

			return users;
		}


		public async Task<IEnumerable<ChatMessageViewModel>> GetMessagesAsync(Guid receiverId, Guid senderId)
		{

			var messages = await context.ChatMessages
				.Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
				            (m.SenderId == receiverId && m.ReceiverId == senderId))
				.OrderBy(m => m.Timestamp)
				.ToListAsync();

			var messageViewModels = messages.Select(m => new ChatMessageViewModel
			{
				SenderId = m.SenderId,
				ReceiverId = m.ReceiverId,
				Content = m.Message,
				Timestamp = m.Timestamp,
				IsRead = m.IsRead

			}).ToList();

			return messageViewModels;
		}

		public async Task SendMessageAsync(Guid senderId, Guid receiverId, string content)
		{
			var message = new ChatMessage
			{
				SenderId = senderId,
				ReceiverId = receiverId,
				Message = content,
				Timestamp = DateTime.UtcNow
			};

			await context.ChatMessages.AddAsync(message);
			await context.SaveChangesAsync();
		}


	}
}
