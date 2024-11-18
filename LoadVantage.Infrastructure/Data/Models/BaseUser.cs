using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using static LoadVantage.Common.ValidationConstants.UserValidations;

namespace LoadVantage.Infrastructure.Data.Models
{
	public abstract class BaseUser : IdentityUser<Guid>
	{
        public BaseUser(string companyName)
        {
            CompanyName = companyName; 
        }

        public BaseUser()
        {
        }

        [Required]
        [StringLength(PositionMaxLength)]
        public virtual string? Position { get; set; }

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