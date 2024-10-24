using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static LoadVantage.Common.ValidationConstants;

namespace LoadVantage.Infrastructure.Data.Models
{
	
		public abstract class User : IdentityUser<Guid> 
		{
			[Key] 
			public Guid UserId { get; set; } = new Guid();

			[Required]
			[StringLength(UserNameMaxLength)]
			public override string UserName { get; set; } = null!;

			[Required]
			[StringLength(EmailMaxLength)]
			public override string Email { get; set; } = null!;

			[Required] 
			public override string PasswordHash { get; set; } = null!;

			[Required]
			[StringLength(FirstNameMaxLength)]
			public string FirstName { get; set; } = null!;

			[Required]
			[StringLength(LastNameMaxLength)]
			public string LastName { get; set; } = null!;

			public string FullName => $"{FirstName} {LastName}"; // FullName combines FirstName and LastName with an interval in between
			public abstract string GetRoleName(); // to be implemented by child classes
	}
}


