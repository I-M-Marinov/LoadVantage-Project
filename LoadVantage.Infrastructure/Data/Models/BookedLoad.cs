using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace LoadVantage.Infrastructure.Data.Models
{
    public class BookedLoad
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		public Guid LoadId { get; set; }

		[ForeignKey(nameof(LoadId))]
		public virtual Load Load { get; set; } = null!;

		[Required]
		public Guid DispatcherId { get; set; }

		[ForeignKey(nameof(DispatcherId))] 
		public virtual Dispatcher Dispatcher { get; set; } = null!;

		[Required]
		public Guid BrokerId { get; set; }

		[ForeignKey(nameof(BrokerId))] 
		public virtual Broker Broker { get; set; } = null!;

        [Required]
        public DateTime BookedDate { get; set; } = DateTime.Now; 

		public Guid? DriverId { get; set; }

		[ForeignKey(nameof(DriverId))]
		public virtual Driver? Driver { get; set; }


    }
}
