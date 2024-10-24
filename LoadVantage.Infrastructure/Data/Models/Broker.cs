using static LoadVantage.Common.GeneralConstants.UserRoles;


namespace LoadVantage.Infrastructure.Data.Models
{
	public class Broker : User
	{
		public override string GetRoleName() => BrokerRoleName;
	}
}
