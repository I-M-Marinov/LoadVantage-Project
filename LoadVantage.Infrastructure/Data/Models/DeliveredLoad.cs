using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static LoadVantage.Common.ValidationConstants.LoadValidations;
namespace LoadVantage.Infrastructure.Data.Models
{
	public class DeliveredLoad
	{
		[Key]
		[Comment("Unique identifier for a Delivered Load")]
		public Guid Id { get; set; } 

		[Required]
		[Comment("Unique identifier for the Load")]
		public Guid LoadId { get; set; }
		[ForeignKey(nameof(LoadId))]
		public Load Load { get; set; } = null!; 

		[Required]
		[Comment("Unique identifier for the Driver")]
		public Guid DriverId { get; set; }
		[ForeignKey(nameof(DriverId))]
		public Driver Driver { get; set; } = null!; 

		[Required]
		[Comment("Unique identifier for the Dispatcher")]
		public Guid DispatcherId { get; set; }
		[ForeignKey(nameof(DispatcherId))]
		public Dispatcher Dispatcher { get; set; } = null!; 

		[Required]
		[Comment("Unique identifier for the Broker")]
		public Guid BrokerId { get; set; }
		[ForeignKey(nameof(BrokerId))]
		public Broker Broker { get; set; } = null!;

		[Comment("The date and time the load was delivered.")]
		public DateTime DeliveredDate { get; set; } = DateTime.UtcNow.ToLocalTime();

		[Comment("Unique identifier for the Booked Load")]
		public Guid? BookedLoadId { get; set; }
		[ForeignKey(nameof(BookedLoadId))]
		public BookedLoad BookedLoad { get; set; }

	}

}
