using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class Truck
	{
		[Key]
		[Comment("Unique identifier for a truck")]
		public Guid Id { get; set; }

		[Required]
		[Comment("Additional reference number for a truck, usually inside the company it is used in.")]

		public string TruckNumber { get; set; } = null!;

		[Required]
		[Comment("Truck Make")]

		public string Make { get; set; } = null!;

		[Required]
		[Comment("Truck Model")]

		public string Model { get; set; } = null!;

		[Required]
		[Comment("Truck Production Year")]
		public int Year { get; set; } 

		[Required]
		[Comment("Unique identifier for the Dispatcher")]
		public Guid DispatcherId { get; set; }
		[ForeignKey(nameof(DispatcherId))]
		public virtual Dispatcher Dispatcher { get; set; } = null!;
		[Comment("Unique identifier for the Driver")]
		public Guid? DriverId { get; set; }
		public virtual Driver? Driver { get; set; } = null!;
		[Comment("Signifies if the truck is ready to go on the road.")]
		public bool IsAvailable { get; set; } = true;
		[Comment("Signifies if the truck is active or decommissioned.")]
		public bool IsActive { get; set; } = true;


	}
}
