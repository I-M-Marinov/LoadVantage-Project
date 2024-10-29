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
        public Guid LoadId { get; set; }

        [ForeignKey(nameof(LoadId))]
        public virtual Load Load { get; set; } = null!;

        [Required]
        public DateTime PostedDate { get; set; } = DateTime.Now;


    }
}
