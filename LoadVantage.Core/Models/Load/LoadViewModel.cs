using System.ComponentModel.DataAnnotations;
using LoadVantage.Core.ValidationAttributes;
using static LoadVantage.Common.ValidationConstants.LoadValidations;

namespace LoadVantage.Core.Models.Load
{
    public class LoadViewModel
    {
        public Guid? Id { get; set; } 

        [Required(ErrorMessage = OriginCityRequired)]
        [StringLength(LoadOriginCityMaxLength, MinimumLength = LoadOriginCityMinLength, ErrorMessage = OriginCityLengthNotValid)]

        public string OriginCity { get; set; } = null!;

        [Required(ErrorMessage = OriginStateRequired)]
        [StringLength(StateMinMaxLength, ErrorMessage = StateLengthNotValid)]

        public string OriginState { get; set; } = null!;

        [Required(ErrorMessage = DestinationCityRequired)]
        [StringLength(LoadDestinationCityMaxLength, MinimumLength = LoadDestinationCityMinLength, ErrorMessage = DestinationCityLengthNotValid)]

        public string DestinationCity { get; set; } = null!;

        [Required(ErrorMessage = DestinationStateRequired)]
        [StringLength(StateMinMaxLength, ErrorMessage = StateLengthNotValid)]
        public string DestinationState { get; set; } = null!;

        [Required(ErrorMessage = PickupTimeRequired)]
        [DataType(DataType.DateTime, ErrorMessage = PickupTimeInvalidFormat)]
        public DateTime PickupTime { get; set; }

        [Required(ErrorMessage = DeliveryTimeRequired)]
        [DataType(DataType.DateTime, ErrorMessage = DeliveryTimeInvalidFormat)]
        [DateGreaterThan(nameof(PickupTime), ErrorMessage = DeliveryTimeCannotBeBeforePickupTime)] // Custom attribute  
        public DateTime DeliveryTime { get; set; }

        [Required(ErrorMessage = PriceRequired)]
        [Range(LoadPriceMinValue, LoadPriceMaxValue, ErrorMessage = PriceRangeInvalid)]
        public decimal PostedPrice { get; set; }

        [Required(ErrorMessage = WeightRequired)]
        [Range(WeightMinValue, WeightMaxValue, ErrorMessage = WeightRangeInvalid)]
        public double Weight { get; set; }

        public string? Status { get; set; } 
        public Guid BrokerId { get; set; }
        public Guid? DispatcherId { get; set; }
    }
}
