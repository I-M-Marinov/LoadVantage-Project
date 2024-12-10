using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LoadVantage.Common.Enums;
using Microsoft.EntityFrameworkCore;


namespace LoadVantage.Infrastructure.Data.Models
{
    public class BookedLoad
	{
		[Key]
		[Comment("Unique identifier for a Booked load")]
		public Guid Id { get; set; }

		[Required]
		[Comment("Unique identifier for the load")]
		public Guid LoadId { get; set; }

		[ForeignKey(nameof(LoadId))]
		public virtual Load Load { get; set; } = null!;

		[Required]
		[Comment("Unique identifier for the Dispatcher")]
		public Guid DispatcherId { get; set; }

		[ForeignKey(nameof(DispatcherId))] 
		public virtual Dispatcher Dispatcher { get; set; } = null!;

		[Required]
		[Comment("Unique identifier for the Broker")]
		public Guid BrokerId { get; set; }

		[ForeignKey(nameof(BrokerId))] 
		public virtual Broker Broker { get; set; } = null!;

        [Required]
        [Comment("The date and time the load was booked.")]
		public DateTime BookedDate { get; set; } = DateTime.Now;

		[Comment("Unique identifier for the Driver")]
		public Guid? DriverId { get; set; }

		[ForeignKey(nameof(DriverId))]
		public virtual Driver? Driver { get; set; }

	}
}
