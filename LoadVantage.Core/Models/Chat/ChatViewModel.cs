using LoadVantage.Core.Models.Profile;

namespace LoadVantage.Core.Models.Chat
{
	public class ChatViewModel
	{
		public List<UserChatViewModel> Users { get; set; } 
		public Guid CurrentChatUserId { get; set; }
		public List<ChatMessageViewModel> Messages { get; set; }
		public UserChatViewModel UserInfo { get; set; }
		public ProfileViewModel Profile { get; set; }
	}

}
