using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class Role : IdentityRole<Guid>
	{
		public Role() 
		{
			Name = "User";
		}

		public Role(string roleName) 
			:base(roleName)
		{
			if (string.IsNullOrEmpty(roleName))
			{
				Name = "User";
			}
		}
	}
}
