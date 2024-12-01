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
        private readonly IUserService userService;
        private readonly IProfileService profileService;


        public ChatService(IHubContext<ChatHub> _chatHub, LoadVantageDbContext _dbContext, IUserService _userService, IProfileService _profileService)
		{
			chatHub = _chatHub;
			context = _dbContext;
            userService = _userService;
            profileService = _profileService;

        }

        public async Task<List<UserChatViewModel>> GetChatUsersAsync(Guid currentUserId,bool includeNewChat = false, Guid? newChatUserId = null)
		{
			var userIds = await context.ChatMessages
				.Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
				.Select(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
				.Distinct()
				.ToListAsync();

			if (includeNewChat && newChatUserId.HasValue)
			{
				userIds.Add(newChatUserId.Value); 
				userIds = userIds.Distinct().ToList(); // eliminate duplicates
			}

			var users = await context.Users
				.Where(u => userIds.Contains(u.Id))
				.Select(u => new UserChatViewModel
				{
					Id = u.Id,
					FullName = u.FullName,
					ProfilePictureUrl = context.UsersImages
						.Where(ui => ui.Id == u.UserImageId)
						.Select(ui => ui.ImageUrl)
						.FirstOrDefault()!,
					Company = u.CompanyName!,
					PhoneNumber = u.PhoneNumber!
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
				Id = m.Id,
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
			var chatMessage = new ChatMessage
			{
				Id = Guid.NewGuid(),
				SenderId = senderId,
				ReceiverId = receiverId,
				Message = content,
				Timestamp = DateTime.UtcNow.ToLocalTime()
			};

			await context.ChatMessages.AddAsync(chatMessage);
			await context.SaveChangesAsync();

			var chatMessageViewModel = new ChatMessageViewModel
			{
				Id = chatMessage.Id,
				SenderId = chatMessage.SenderId,
				ReceiverId = chatMessage.ReceiverId,
				Content = chatMessage.Message,
				Timestamp = chatMessage.Timestamp
			};


			await chatHub.Clients.User(senderId.ToString().Trim()).SendAsync("ReceiveMessage", chatMessageViewModel);
			await chatHub.Clients.User(receiverId.ToString().Trim()).SendAsync("ReceiveMessage", chatMessageViewModel);
		}

		public async Task<(List<ChatMessage> Messages, int UnreadCount)> GetUnreadMessagesAsync(Guid userId)
		{
			// Get unread messages for the user from the database
			var unreadMessages = await context.ChatMessages
				.Where(m => m.ReceiverId == userId && !m.IsRead)
				.OrderByDescending(m => m.Timestamp)
				.ToListAsync();

			int unreadCount = unreadMessages.Count;

			return (unreadMessages, unreadCount);
		}

		public async Task MarkMessagesAsReadAsync(Guid senderId, Guid receiverId)
		{
			var messages = await context.ChatMessages
				.Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
				.ToListAsync();

			foreach (var message in messages)
			{
				message.IsRead = true;
			}

			await context.SaveChangesAsync();
		}

		public async Task<ChatMessage?> GetLastChatAsync(Guid userId)
		{
			ChatMessage? result = await context.ChatMessages
				.Where(c => c.SenderId == userId || c.ReceiverId == userId)
				.OrderByDescending(cm => cm.Timestamp) 
				.FirstOrDefaultAsync();

			return result;
		}

        public async Task<ChatViewModel> BuildChatViewModel(Guid userId)
        {
            var currentUser = await userService.GetCurrentUserAsync();

            // Fetch chat users, messages, and user info and the current user's profile 
            var chatUsers = await GetChatUsersAsync(currentUser.Id);
            var userInfo = await userService.GetChatUserInfoAsync(userId);
            var messages = await GetMessagesAsync(currentUser.Id, userId);
            var profile = await profileService.GetUserInformation(currentUser.Id);

            // Build the ChatViewModel
            var chatViewModel = new ChatViewModel
            {
                Users = chatUsers ?? new List<UserChatViewModel>(),
                CurrentChatUserId = userId,
                Messages = messages.Select(m => new ChatMessageViewModel
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    IsRead = m.IsRead
                }).ToList(),
                UserInfo = userInfo,
                Profile = profile
            };

            return chatViewModel;
        }

        

	}


}

