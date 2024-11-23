using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Extensions
{
	public static class UserExtensions
	{
		public static string GetUserType(this User user)
		{
			return user is Broker ? nameof(Broker) : nameof(Dispatcher);
		}
	}
}
