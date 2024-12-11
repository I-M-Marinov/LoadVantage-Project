using LoadVantage.Core.Models.Profile;

namespace LoadVantage.Core.Models.Chat
{
	public class ChatViewModel
	{
		public List<UserChatViewModel> Users { get; set; } = null!;  
		public Guid CurrentChatUserId { get; set; }
		public List<ChatMessageViewModel> Messages { get; set; } = null!;
		public UserChatViewModel UserInfo { get; set; } = null!;
		public ProfileViewModel Profile { get; set; } = null!;
	}

}
