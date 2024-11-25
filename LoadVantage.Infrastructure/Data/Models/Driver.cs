using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.ValidationConstants;
using static LoadVantage.Common.ValidationConstants.UserValidations;


namespace LoadVantage.Infrastructure.Data.Models
{
	public class Driver
	{
		[Key]
		public Guid DriverId { get; set; }

		[Required]
		[StringLength(FirstNameMaxLength)]

		public string FirstName { get; set; } = null!;

		[Required]
		[StringLength(LastNameMaxLength)]

		public string LastName { get; set; } = null!;

		[Required]
		[StringLength(LicenseNumberMaxLength)]

		public string LicenseNumber { get; set; } = null!;

		public Guid? TruckId { get; set; }

		[ForeignKey(nameof(TruckId))]
		public virtual Truck? Truck { get; set; }

		public Guid? DispatcherId { get; set; }

		[ForeignKey(nameof(DispatcherId))]
		public virtual Dispatcher? Dispatcher { get; set; }
		public bool IsAvailable { get; set; } = true;
		public bool IsFired { get; set; } = false;
		public bool IsBusy { get; set; } = false;
		public string FullName => $"{FirstName} {LastName}"; // FullName combines FirstName and LastName

	}
}
