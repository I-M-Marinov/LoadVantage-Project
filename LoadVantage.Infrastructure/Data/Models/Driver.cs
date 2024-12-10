using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.ValidationConstants.DriverValidations;
using static LoadVantage.Common.ValidationConstants.UserValidations;
using Microsoft.EntityFrameworkCore;


namespace LoadVantage.Infrastructure.Data.Models
{
	public class Driver
	{
		[Key]
		[Comment("Unique identifier for a Driver")]
		public Guid DriverId { get; set; }

		[Required]
		[StringLength(FirstNameMaxLength)]
		[Comment("First Name of the Driver")]
		public string FirstName { get; set; } = null!;

		[Required]
		[StringLength(LastNameMaxLength)]
		[Comment("Last Name of the Driver")]

		public string LastName { get; set; } = null!;

		[Required]
		[StringLength(LicenseNumberMaxLength)]
		[Comment("License Number of the Driver")]

		public string LicenseNumber { get; set; } = null!;

		[Comment("Unique identifier for the Truck")]
		public Guid? TruckId { get; set; }

		[ForeignKey(nameof(TruckId))]
		public virtual Truck? Truck { get; set; }

		[Comment("Unique identifier for the Dispatcher")]
		public Guid? DispatcherId { get; set; }

		[ForeignKey(nameof(DispatcherId))]
		public virtual Dispatcher? Dispatcher { get; set; }
		[Comment("Signifies if a driver is available to be assigned to a truck.")]
		public bool IsAvailable { get; set; } = true;
		[Comment("Signifies if a driver is fired or not.")]
		public bool IsFired { get; set; } = false;
		[Comment("Signifies if a driver is currently on a job or not.")]
		public bool IsBusy { get; set; } = false;
		public string FullName => $"{FirstName} {LastName}"; // FullName combines FirstName and LastName

	}
}
