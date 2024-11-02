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
        public Guid Id { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Required]
        [StringLength(LoadOriginCityMaxLength)]

        public string OriginCity { get; set; } = null!;

        [Required]
        [StringLength(StateMinMaxLength)]

        public string OriginState { get; set; } = null!;

        [Required]
        [StringLength(LoadDestinationCityMaxLength)]

        public string DestinationCity { get; set; } = null!;

        [Required]
        [StringLength(StateMinMaxLength)]
        public string DestinationState { get; set; } = null!;

        [Required]
        public DateTime PickupTime { get; set; }

        [Required]
        public DateTime DeliveryTime { get; set; }

        public double? Distance { get; set; } // Nullable in case the distance is yet to be calculated

        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Required]
        [Range(WeightMinValue,WeightMaxValue)]
        public double Weight { get; set; }

        // Track current load status
        public LoadStatus Status { get; set; } = LoadStatus.Created;

        [Required]
        public Guid BrokerId { get; set; }

        [ForeignKey(nameof(BrokerId))]
        public virtual Broker Broker { get; set; } = null!;

        // Navigation properties to specialized tables
        public virtual PostedLoad? PostedLoad { get; set; }
        public virtual BookedLoad? BookedLoad { get; set; }
        public virtual BilledLoad? BilledLoad { get; set; }
    }
}
