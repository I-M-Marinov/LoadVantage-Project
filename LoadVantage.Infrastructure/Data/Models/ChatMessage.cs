using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class ChatMessage
	{
		[Required]
		public Guid Id { get; set; } 

		[Required]
		public Guid SenderId { get; set; }
		[Required]
		public User Sender { get; set; }
		[Required]
		public Guid ReceiverId { get; set; }
		[Required]
		public User Receiver { get; set; }

		[Required]
		[MaxLength(2000)]
		public string Message { get; set; }

		public DateTime Timestamp { get; set; }

		public bool IsRead { get; set; } = false;
	}
}
