using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static LoadVantage.Common.ValidationConstants.UserValidations;

namespace LoadVantage.Infrastructure.Data.Models
{
	
		public class User : BaseUser
		{

			public User(string companyName)
                : base(companyName)
            {
                Id = Guid.NewGuid();
            }

            public User()
            {
                Id = Guid.NewGuid();
            }

            public required Role Role { get; set; }

            [ForeignKey(nameof(Role))]
			public Guid RoleId { get; set; } 

			public override string GetRoleName() => Role.ToString();

            [Required]
			[StringLength(UserNameMaxLength)]
			public override string? UserName { get; set; } = null!;

			[Required]
			[StringLength(EmailMaxLength)]
			public override string? Email { get; set; } = null!;

			[Required]
			public override string? PasswordHash { get; set; } = null!;
            [Phone]
            [StringLength(UserPhoneNumberMaxLength)]
            public override string? PhoneNumber { get; set; } = null!;

        }
}


