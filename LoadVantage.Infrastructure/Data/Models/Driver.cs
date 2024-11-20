using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.ValidationConstants;
using static LoadVantage.Common.ValidationConstants.UserValidations;


namespace LoadVantage.Infrastructure.Data.Models
{
	public class Driver
	{
		[Key]
		public Guid DriverId { get; set; } = new Guid();

		[Required]
		[StringLength(FirstNameMaxLength)]

		public string FirstName { get; set; } = null!;

		[Required]
		[StringLength(LastNameMaxLength)]

		public string LastName { get; set; } = null!;

		[Required]
		[StringLength(LicenseNumberMaxLength)]

		public string LicenseNumber { get; set; } = null!;

		public int? TruckId { get; set; }

		[ForeignKey(nameof(TruckId))]
		public virtual Truck? Truck { get; set; }
		public bool IsAvailable { get; set; } = true;
		public string FullName => $"{FirstName} {LastName}"; // FullName combines FirstName and LastName

	}
}
