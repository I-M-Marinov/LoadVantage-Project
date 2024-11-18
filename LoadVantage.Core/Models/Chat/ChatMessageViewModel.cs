using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Core.Models.Chat
{
	public class ChatMessageViewModel
	{
		public string Content { get; set; } = null!;
		public Guid SenderId { get; set; }
		public Guid ReceiverId { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsRead { get; set; }
	}

}
