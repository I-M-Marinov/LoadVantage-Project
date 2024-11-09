using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LoadVantage.Common.ValidationConstants.UserValidations;


namespace LoadVantage.Infrastructure.Data.Models
{
	public class UserImage
	{
		[Key]
		public Guid Id { get; set; }

		public Guid UserId { get; set; }

		public User? User { get; set; }

		[StringLength(UserImageMaxLength)]
		public string? ImageUrl { get; set; }

		[StringLength(UserImagePublicIdMaxLength)]
		[Required]
		public string? PublicId { get; set; } = null!;

	}
}
