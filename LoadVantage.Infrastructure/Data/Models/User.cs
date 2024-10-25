using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.ValidationConstants;

namespace LoadVantage.Infrastructure.Data.Models
{
	
		public class User : BaseUser
		{

			public User() 
				: base() 
			{
				Id = Guid.NewGuid();
			} 
            
            public virtual string Role { get; set; } = UserRoleName; // Default role name

            public override string GetRoleName() => Role;


            [Required]
			[StringLength(UserNameMaxLength)]
			public override string UserName { get; set; } = null!;

			[Required]
			[StringLength(EmailMaxLength)]
			public override string Email { get; set; } = null!;

			[Required]
			public override string PasswordHash { get; set; } = null!; 

	    }
}


