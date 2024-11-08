using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class BilledLoad
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		public Guid LoadId { get; set; }
		[ForeignKey(nameof(LoadId))]
		public Load Load { get; set; } = null!;

		[Required]
		[Precision(18,2)]
		public decimal BilledAmount { get; set; }

		public DateTime BilledDate { get; set; } = DateTime.Now;

    }
}
