using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class Truck
	{
		[Key]
		public Guid Id { get; set; }

		[Required] 
		public string TruckNumber { get; set; } = null!;

		[Required]
		public string Make { get; set; } = null!;

		[Required]
		public string Model { get; set; } = null!;

		[Required]
		public Guid DispatcherId { get; set; }
		[ForeignKey(nameof(DispatcherId))]
		public virtual Dispatcher Dispatcher { get; set; } = null!;
		public Guid? DriverId { get; set; }
		public virtual Driver? Driver { get; set; } = null!;
		public bool IsAvailable { get; set; } = true;
		public bool IsActive { get; set; } = true;


	}
}
