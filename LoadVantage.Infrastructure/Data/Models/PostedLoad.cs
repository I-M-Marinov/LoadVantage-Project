using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class PostedLoad
	{
        [Key]
        [Comment("Unique identifier for a Posted Load")]
		public Guid Id { get; set; }

        [Required]
        [Comment("Unique identifier for the Load")]
		public Guid LoadId { get; set; }

        [ForeignKey(nameof(LoadId))]
        public virtual Load Load { get; set; } = null!;

        [Comment("Date and Time when the load was Posted")]
		public DateTime? PostedDate { get; set; }

    }
}
