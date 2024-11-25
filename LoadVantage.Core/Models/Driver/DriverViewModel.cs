
using System.ComponentModel.DataAnnotations;
using LoadVantage.Core.Models.Truck;
using static LoadVantage.Common.GeneralConstants;
using static LoadVantage.Common.ValidationConstants.DriverValidations;


namespace LoadVantage.Core.Models.Driver
{
	public class DriverViewModel
	{

		public Guid Id { get; set; }

		[Required(ErrorMessage = DriverFirstNameRequired)]
		[Display(Name = "Driver's first name")]
		[StringLength(DriverFirstNameMaxLength, MinimumLength = DriverFirstNameMinLength, ErrorMessage = DriverFirstNameInvalid)]
		public string FirstName { get; set; } = null!;

		[Required(ErrorMessage = DriverLastNameRequired)]
		[Display(Name = "Driver's last name")]
		[StringLength(DriverLastNameMaxLength, MinimumLength = DriverLastNameMinLength, ErrorMessage = DriverLastNameInvalid)]

		public string LastName { get; set; } = null!;

		[Required(ErrorMessage = DriverLicenseNumberRequired)]
		[Display(Name = "Driver's license number ")]
		[RegularExpression(DriverLicenceNumberRegexPattern, ErrorMessage = DriverLicenseNumberInvalid)]
		public string LicenseNumber { get; set; } = null!;

		public string? TruckNumber { get; set; }

		[Required]
		[Display(Name = "Available")]
		public bool IsAvailable { get; set; }

		[Display(Name = "Fired")]
		public bool isFired { get; set; }

		[Display(Name = "On a load")]
		public bool IsBusy { get; set; }




	}
}
