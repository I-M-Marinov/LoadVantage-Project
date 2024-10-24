using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LoadVantage.Common.Enums;


namespace LoadVantage.Infrastructure.Data.Models
{
    public class BookedLoad
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int LoadId { get; set; }

		[ForeignKey(nameof(LoadId))]
		public virtual Load Load { get; set; } = null!;

		[Required]
		public Guid DispatcherId { get; set; }

		[ForeignKey(nameof(DispatcherId))] 
		public virtual Dispatcher Dispatcher { get; set; } = null!;

		[Required]
		public Guid BrokerId { get; set; }

		[ForeignKey(nameof(BrokerId))] 
		public virtual Broker Broker { get; set; } = null!;

		[Required]
		public Guid? DriverId { get; set; }

		[ForeignKey(nameof(DriverId))]
		public virtual Driver? Driver { get; set; }

		public LoadStatus Status { get; set; } 
		public DateTime BookedDate { get; set; }

		

	}
}
