using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadVantage.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class BilledLoad
	{
		public Guid Id { get; set; }

		[Required]
		public Guid LoadId { get; set; }
		[ForeignKey(nameof(LoadId))]
		public PostedLoad PostedLoad { get; set; } = null!;

		[Required]
		public Guid BookedLoadId { get; set; }
		[ForeignKey(nameof(BookedLoadId))]
		public BookedLoad BookedLoad { get; set; } = null!; 

		[Required]
		[Precision(18,2)]
		public decimal BilledAmount { get; set; }
		// Date the load was billed
		public DateTime BilledDate { get; set; } = DateTime.Now;
        public LoadStatus Status { get; set; } = LoadStatus.Delivered;

    }
}
