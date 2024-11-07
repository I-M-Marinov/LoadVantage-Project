using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LoadVantage.Core.ValidationAttributes;
using static LoadVantage.Common.ValidationConstants.LoadValidations;

namespace LoadVantage.Core.Models.Load
{
    public class LoadViewModel
    {
        public Guid Id { get; set; } 

        [Required(ErrorMessage = OriginCityRequired)]
        [DisplayName("Origin city")]
        [StringLength(LoadOriginCityMaxLength, MinimumLength = LoadOriginCityMinLength, ErrorMessage = OriginCityLengthNotValid)]

        public string OriginCity { get; set; } = null!;

        [Required(ErrorMessage = OriginStateRequired)]
        [DisplayName("Origin state")]
        [StringLength(StateMinMaxLength, MinimumLength = StateMinMaxLength, ErrorMessage = StateLengthNotValid)]

        public string OriginState { get; set; } = null!;

        [Required(ErrorMessage = DestinationCityRequired)]
        [DisplayName("Destination city")]
        [StringLength(LoadDestinationCityMaxLength, MinimumLength = LoadDestinationCityMinLength, ErrorMessage = DestinationCityLengthNotValid)]

        public string DestinationCity { get; set; } = null!;

        [Required(ErrorMessage = DestinationStateRequired)]
        [DisplayName("Destination state")]
		[StringLength(StateMinMaxLength, MinimumLength = StateMinMaxLength, ErrorMessage = StateLengthNotValid)]
		public string DestinationState { get; set; } = null!;

        [Required(ErrorMessage = PickupTimeRequired)]
        [DisplayName("Pickup time")]
        [DataType(DataType.DateTime, ErrorMessage = PickupTimeInvalidFormat)]
        public DateTime PickupTime { get; set; }

        [Required(ErrorMessage = DeliveryTimeRequired)]
        [DisplayName("Delivery time")]
        [DataType(DataType.DateTime, ErrorMessage = DeliveryTimeInvalidFormat)]
        [DateGreaterThan(nameof(PickupTime), ErrorMessage = DeliveryTimeCannotBeBeforePickupTime)] // Custom attribute  
        public DateTime DeliveryTime { get; set; }

        [DisplayName("Distance")]
        public double? Distance { get; set; }

        [Required(ErrorMessage = PriceRequired)]
        [DisplayName("Price")]
        [Range(LoadPriceMinValue, LoadPriceMaxValue, ErrorMessage = PriceRangeInvalid)]
        public decimal PostedPrice { get; set; }

        [Required(ErrorMessage = WeightRequired)]
        [DisplayName("Weight")]
        [Range(WeightMinValue, WeightMaxValue, ErrorMessage = WeightRangeInvalid)]
        public double Weight { get; set; }

        [DisplayName("Status")]
        public string? Status { get; set; } 
        public Guid BrokerId { get; set; }
        public Guid? DispatcherId { get; set; }
    }
}
