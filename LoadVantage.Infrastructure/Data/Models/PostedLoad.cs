using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class PostedLoad
	{
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid LoadId { get; set; }

        [ForeignKey(nameof(LoadId))]
        public virtual Load Load { get; set; } = null!;

        [Required]
        public DateTime PostedDate { get; set; } = DateTime.Now;


    }
}
