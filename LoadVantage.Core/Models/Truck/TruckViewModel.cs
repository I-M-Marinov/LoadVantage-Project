
using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.ValidationConstants.TruckValidations;


namespace LoadVantage.Core.Models.Truck
{
	public class TruckViewModel
	{
		public Guid Id { get; set; }

		[Required(ErrorMessage = TruckNumberRequired)]
		[Display(Name = "Truck Number")]
		[RegularExpression(TruckNumberRegexPattern, ErrorMessage = TruckNumberInvalid)]
		public string TruckNumber { get; set; } = null!;

		[Required(ErrorMessage = TruckMakeRequired)]
		[Display(Name = "Make")]
		[StringLength(TruckMakeMaxLength, MinimumLength = TruckMakeMinLength, ErrorMessage = TruckMakeInvalid)]

		public string Make { get; set; } = null!;

		[Required(ErrorMessage = TruckModelRequired)]
		[Display(Name = "Model")]
		[StringLength(TruckModelMaxLength, MinimumLength = TruckModelMinLength, ErrorMessage = TruckModelInvalid)]

		public string Model { get; set; } = null!;

		[Required(ErrorMessage = TruckYearRequired)]
		[Display(Name = "Year")]
		[Range(TruckYearMinValue, TruckYearMaxValue, ErrorMessage = TruckYearInvalid)]

		public string Year { get; set; } = null!;

		[Display(Name = "Driver Name")]
		public string? DriverName { get; set; }

		[Required(ErrorMessage = AvailabilityRequired)]
		[Display(Name = "Available")]
		public bool IsAvailable { get; set; }


	}

}
