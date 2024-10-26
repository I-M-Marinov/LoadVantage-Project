using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static LoadVantage.Common.ValidationConstants;

namespace LoadVantage.Infrastructure.Data.Models
{
	public abstract class BaseUser : IdentityUser<Guid>
	{
		[Required]
		[StringLength(FirstNameMaxLength)]
		public string FirstName { get; set; } = null!;

		[Required]
		[StringLength(LastNameMaxLength)]
		public string LastName { get; set; } = null!;

        [StringLength(CompanyNameMaxLength)]
        public string? CompanyName { get; set; }

		public string FullName => $"{FirstName} {LastName}"; // FullName combines FirstName and LastName

		public abstract string GetRoleName(); // to be implemented by child classes
	}
}