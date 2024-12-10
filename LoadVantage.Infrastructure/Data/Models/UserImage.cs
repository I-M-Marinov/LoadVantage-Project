using Microsoft.EntityFrameworkCore;
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
		[Comment("Unique identifier for the User Image")]
		public Guid Id { get; set; }

		[StringLength(UserImageMaxLength)]
		[Comment("Url address pointing to the User Image")]
		public string? ImageUrl { get; set; }

		[StringLength(UserImagePublicIdMaxLength)]
		[Required]
		[Comment("Key used in Cloudinary to determine validity of the User Image")]
		public string PublicId { get; set; } = null!;
		public virtual ICollection<BaseUser> Users { get; set; } = new HashSet<BaseUser>();

	}
}
