using LoadVantage.Areas.Admin.Models.Profile;
using LoadVantage.Core.Models.Chat;

namespace LoadVantage.Areas.Admin.Models.AdminChat
{
	public class AdminChatViewModel
	{
		public List<UserChatViewModel> Users { get; set; } = null!;
		public Guid CurrentChatUserId { get; set; }
		public List<ChatMessageViewModel> Messages { get; set; } = null!;
		public UserChatViewModel UserInfo { get; set; } = null!;
		public AdminProfileViewModel Profile { get; set; } = null!;
	}
}
