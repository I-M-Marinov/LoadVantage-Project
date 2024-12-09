

namespace LoadVantage.Core.Models.Chat
{
	public class UserChatViewModel
	{
		public Guid Id { get; set; }
		public string FullName { get; set; }
		public string ProfilePictureUrl { get; set; }
		public string PhoneNumber { get; set; }
		public string Company { get; set; }
		public string? Position { get; set; }
	}
}