namespace LoadVantage.Core.Models.Chat
{
	public class UserChatViewModel
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = null!;
		public string ProfilePictureUrl { get; set; } = null!;
		public string PhoneNumber { get; set; } = null!;
		public string Company { get; set; } = null!;
		public string? Position { get; set; }
	}
}