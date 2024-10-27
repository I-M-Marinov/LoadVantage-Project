using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.ValidationConstants;

namespace LoadVantage.Infrastructure.Data.Models
{
	
		public class User : BaseUser
		{

			public User()
            {
                Id = Guid.NewGuid();
            } 
            

            public Role Role { get; set; }

            [ForeignKey(nameof(Role))]
			public Guid RoleId { get; set; } 

			[Required]
            [StringLength(PositionMaxLength)]
            public string? Position { get; set; }

			public override string GetRoleName() => Role.ToString();


            [Required]
			[StringLength(UserNameMaxLength)]
			public override string? UserName { get; set; } = null!;

			[Required]
			[StringLength(EmailMaxLength)]
			public override string? Email { get; set; } = null!;

			[Required]
			public override string? PasswordHash { get; set; } = null!; 

	    }
}


