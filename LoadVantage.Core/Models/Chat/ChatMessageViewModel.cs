namespace LoadVantage.Core.Models.Chat
{
	public class ChatMessageViewModel
	{
		public Guid Id { get; set; }
		public string Content { get; set; } = null!;
		public Guid SenderId { get; set; }
		public Guid ReceiverId { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsRead { get; set; }
	}

}
