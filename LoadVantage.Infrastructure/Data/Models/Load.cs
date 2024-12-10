using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LoadVantage.Common.Enums;
using Microsoft.EntityFrameworkCore;
using static LoadVantage.Common.ValidationConstants.LoadValidations;


namespace LoadVantage.Infrastructure.Data.Models
{
    public class Load
    {
        [Key]
        [Comment("Unique identifier for a Load")]
		public Guid Id { get; set; }

        [Required]
        [Comment("Date and Time the load was created.")]
		public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(LoadOriginCityMaxLength)]
        [Comment("The city the load is originating from")]

		public string OriginCity { get; set; } = null!;

        [Required]
        [StringLength(StateMinMaxLength)]
        [Comment("The state the load is originating from")]

		public string OriginState { get; set; } = null!;

        [Required]
        [StringLength(LoadDestinationCityMaxLength)]
        [Comment("The city the load is consigned to")]

		public string DestinationCity { get; set; } = null!;

        [Required]
        [StringLength(StateMinMaxLength)]
        [Comment("The state the load is consigned to")]

		public string DestinationState { get; set; } = null!;

        [Required]
        [Comment("Date and Time when the load needs to be picked up")]
		public DateTime PickupTime { get; set; }

        [Required]
        [Comment("Date and Time when the load needs to be delivered at")]
		public DateTime DeliveryTime { get; set; }

		[Comment("Distance in miles between the Origin and Destinations city and state")]
		public double? Distance { get; set; } // Nullable in case the distance is yet to be calculated

        [Required]
        [Precision(18, 2)]
        [Comment("Decimal amount paid for moving the load from origin to the destination.")]
		public decimal Price { get; set; }

        [Required]
        [Range(WeightMinValue,WeightMaxValue)]
        [Comment("Weight of the load in lbs")]
		public double Weight { get; set; }

		// Track current load status
		[Comment("Status of the load")]
		public LoadStatus Status { get; set; }

        [Required]
        [Comment("Unique identifier for the Broker")]

		public Guid BrokerId { get; set; }

        [ForeignKey(nameof(BrokerId))]
        public virtual Broker Broker { get; set; } = null!;

        // Navigation properties to specialized tables
        public virtual PostedLoad? PostedLoad { get; set; }
        public virtual BookedLoad? BookedLoad { get; set; }
        public virtual DeliveredLoad? DeliveredLoad { get; set; }
    }
}
