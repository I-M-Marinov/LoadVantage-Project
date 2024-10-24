
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Infrastructure.Data.Models
{
	public abstract class User : IdentityUser<Guid>
	{
		[Key]
		public Guid UserID { get; set; } = Guid.NewGuid(); 

		[Required]
		[StringLength(100)]
		public override string UserName { get; set; }

		[Required]
		[StringLength(100)]
		public override string Email { get; set; }

		[Required]
		public string PasswordHash { get; set; } 

		public string FullName { get; set; }

		// to be implemented by child classes
		public abstract string GetRoleName();
	}

}
