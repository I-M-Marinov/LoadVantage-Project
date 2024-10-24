using Microsoft.AspNetCore.Identity;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class Role : IdentityRole<Guid>
	{
		public Role() 
		{

		}

		public Role(string roleName) 
			:base(roleName)
		{

		}
	}
}
