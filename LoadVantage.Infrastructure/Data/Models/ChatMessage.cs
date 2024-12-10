using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.ValidationConstants.UserValidations;


namespace LoadVantage.Infrastructure.Data.Models
{
	public class ChatMessage
	{
		[Required]
		[Comment("Unique identifier for a chat message")]
		public Guid Id { get; set; } 

		[Required]
		[Comment("Unique identifier for a Sender")]
		public Guid SenderId { get; set; }
		[Required]
		public BaseUser Sender { get; set; }
		[Required]
		[Comment("Unique identifier for a Receiver")]
		public Guid ReceiverId { get; set; }
		[Required]
		public BaseUser Receiver { get; set; }

		[Required]
		[MaxLength(MessageMaxValue)]
		[Comment("String content sent between a Sender and a Receiver")]
		public string Message { get; set; }

		[Comment("Time and date of the message sent")]
		public DateTime Timestamp { get; set; }

		[Comment("Signifies if a message has been viewed or not.")]
		public bool IsRead { get; set; } = false;
	}
}
