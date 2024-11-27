using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.ValidationConstants.UserValidations;


namespace LoadVantage.Infrastructure.Data.Models
{
	public class ChatMessage
	{
		[Required]
		public Guid Id { get; set; } 

		[Required]
		public Guid SenderId { get; set; }
		[Required]
		public BaseUser Sender { get; set; }
		[Required]
		public Guid ReceiverId { get; set; }
		[Required]
		public BaseUser Receiver { get; set; }

		[Required]
		[MaxLength(MessageMaxValue)]
		public string Message { get; set; }

		public DateTime Timestamp { get; set; }

		public bool IsRead { get; set; } = false;
	}
}
