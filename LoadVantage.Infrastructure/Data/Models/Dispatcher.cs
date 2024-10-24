
using static LoadVantage.Common.GeneralConstants.UserRoles;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class Dispatcher : User
	{
		public override string GetRoleName() => DispatcherRoleName;
	}
}
