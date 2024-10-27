
namespace LoadVantage.Common
{
	public static class GeneralConstants
	{
		public class UserRoles
		{
			public const string UserRoleName = "User";
			public const string AdminRoleName = "Administrator";
			public const string DispatcherRoleName = "Dispatcher";
			public const string BrokerRoleName = "Broker";
			public const string DriverRoleName = "Driver";

            public static readonly string[] ValidRoles = new[] { AdminRoleName, DispatcherRoleName, BrokerRoleName };
        }

		public class TempMessages
		{
			public const string LoginWithNewAccount = "Login with your new account";
			public const string LoggedOutOfAccount = "You have been logged out of your account";
		}
    }
}
