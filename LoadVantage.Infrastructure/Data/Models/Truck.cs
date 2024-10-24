using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class Truck
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Type { get; set; } = null!; // Dry-Van, Reefer, Flat-Bed

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

		[Required]
		public Guid? DriverId { get; set; }
		public virtual Driver? Driver { get; set; } = null!;
		public bool IsAvailable { get; set; } = true;


	}
}
