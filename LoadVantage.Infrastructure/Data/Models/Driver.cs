using static LoadVantage.Common.GeneralConstants.UserRoles;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class Driver : User
	{
		public override string GetRoleName() => DriverRoleName;
	}
}
