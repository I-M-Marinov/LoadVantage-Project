using LoadVantage.Areas.Admin.Models.Chat;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Chat;
using LoadVantage.Areas.Admin.Contracts;

namespace LoadVantage.Areas.Admin.Services
{
	public class AdminChatService : IAdminChatService
	{
		private readonly IUserService userService;
		private readonly IAdminProfileService adminProfileService;
		private readonly IChatService chatService;


		public AdminChatService(IUserService _userService, IAdminProfileService _adminProfileService, IChatService _chatService)
		{
			userService = _userService;
			adminProfileService = _adminProfileService;
			chatService = _chatService;
		}

		
		public async Task<AdminChatViewModel> BuildChatViewModel(Guid userId)
		{
			var currentUser = await userService.GetCurrentUserAsync();

			// Fetch chat users, messages, and user info and the current user's profile 
			var chatUsers = await chatService.GetChatUsersAsync(currentUser.Id);
			var userInfo = await userService.GetChatUserInfoAsync(userId);
			var messages = await chatService.GetMessagesAsync(currentUser.Id, userId);
			var profile = await adminProfileService.GetAdminInformation(currentUser.Id);

			// Build the ChatViewModel
			var chatViewModel = new AdminChatViewModel
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
