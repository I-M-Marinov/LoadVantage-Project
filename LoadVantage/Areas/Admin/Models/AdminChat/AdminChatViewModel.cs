using LoadVantage.Areas.Admin.Models.Profile;
using LoadVantage.Core.Models.Chat;

namespace LoadVantage.Areas.Admin.Models.AdminChat
{
	public class AdminChatViewModel
	{
		public List<UserChatViewModel> Users { get; set; }
		public Guid CurrentChatUserId { get; set; }
		public List<ChatMessageViewModel> Messages { get; set; }
		public UserChatViewModel UserInfo { get; set; }
		public AdminProfileViewModel Profile { get; set; }
	}
}
