using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static LoadVantage.Common.ValidationConstants.LoadValidations;
namespace LoadVantage.Infrastructure.Data.Models
{
	public class DeliveredLoad
	{
		[Key]
		public Guid Id { get; set; } // Primary key

		[Required]
		public Guid LoadId { get; set; }
		[ForeignKey(nameof(LoadId))]
		public Load Load { get; set; } = null!; 

		[Required]
		public Guid DriverId { get; set; }
		[ForeignKey(nameof(DriverId))]
		public Driver Driver { get; set; } = null!; 

		[Required]
		public Guid DispatcherId { get; set; }
		[ForeignKey(nameof(DispatcherId))]
		public Dispatcher Dispatcher { get; set; } = null!; 

		[Required]
		public Guid BrokerId { get; set; }
		[ForeignKey(nameof(BrokerId))]
		public Broker Broker { get; set; } = null!; 

		public DateTime DeliveredDate { get; set; } = DateTime.UtcNow.ToLocalTime();

		[MaxLength(DeliveredLoadNotesMaxLength)]
		public string? Notes { get; set; }

		public Guid? BookedLoadId { get; set; }
		[ForeignKey(nameof(BookedLoadId))]
		public BookedLoad BookedLoad { get; set; }

	}

}
