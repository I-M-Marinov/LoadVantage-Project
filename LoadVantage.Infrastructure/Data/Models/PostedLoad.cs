using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.ValidationConstants;
using Microsoft.EntityFrameworkCore;
using LoadVantage.Common.Enums;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class PostedLoad
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		public DateTime PostedDate { get; set; } = DateTime.Now;

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

		[Required]
		[Precision(18, 2)]
		public decimal PostedPrice { get; set; }

		[Required]
		public double Weight { get; set; }
        public LoadStatus Status { get; set; } = LoadStatus.Available;

        [Required]
		public Guid BrokerId { get; set; }

		[ForeignKey(nameof(BrokerId))] 
		public virtual Broker Broker { get; set; } = null!;

		[Required]
        public bool IsBooked { get; set; } = false; 
        public virtual BookedLoad? BookedLoad { get; set; } 

    }
}
