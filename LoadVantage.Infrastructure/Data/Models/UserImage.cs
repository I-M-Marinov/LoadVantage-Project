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

		[StringLength(UserImageMaxLength)]
		public string? ImageUrl { get; set; }

		[StringLength(UserImagePublicIdMaxLength)]
		[Required]
		public string PublicId { get; set; } = null!;
		public virtual ICollection<BaseUser> Users { get; set; } = new HashSet<BaseUser>();

	}
}
