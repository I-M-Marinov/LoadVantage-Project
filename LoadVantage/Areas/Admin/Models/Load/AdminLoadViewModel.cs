using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LoadVantage.Areas.Admin.Models.Profile;
using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.Profile;
using LoadVantage.Core.ValidationAttributes;
using LoadVantage.Infrastructure.Data.Models;
using static LoadVantage.Common.ValidationConstants.LoadValidations;

namespace LoadVantage.Areas.Admin.Models.Load
{
    public class AdminLoadViewModel
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
        [DateGreaterThan(nameof(PickupTime), ErrorMessage = DeliveryTimeCannotBeBeforePickupTime)]
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
        public Broker? Broker { get; set; }
        public Guid? DispatcherId { get; set; }
        public Dispatcher? Dispatcher { get; set; }
		public Guid? DriverId { get; set; }
        public DateTime? BookedDate { get; set; } // Nullable for posted/billed
        public DateTime? DeliveredDate { get; set; } // Nullable for posted/booked

        public AdminProfileViewModel? AdminProfile { get; set; }
        public DriverInfoViewModel? DriverInfo { get; set; }
        public DispatcherInfoViewModel? DispatcherInfo { get; set; }
        public BrokerInfoViewModel? BrokerInfo { get; set; }
    }
}
