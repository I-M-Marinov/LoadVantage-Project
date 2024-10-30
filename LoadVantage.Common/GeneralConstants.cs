
namespace LoadVantage.Common
{
	public static class GeneralConstants
	{
		public class UserRoles
		{
			public const string UserRoleName = "User";
			public const string AdminRoleName = "Administrator";

			public const string DispatcherPositionName = "Dispatcher";
			public const string BrokerPositionName = "Broker";
			public const string DriverPositionName = "Driver";
			public const string AdminPositionName = "Administrator";

            public static readonly string[] ValidPositions = [DispatcherPositionName, BrokerPositionName, AdminPositionName];
            public static readonly string[] ValidRoles = [AdminRoleName, UserRoleName];
        }

		public class TempMessages
		{
			public const string LoginWithNewAccount = "Login with your new account";
			public const string LoggedOutOfAccount = "You have been logged out of your account";
		}

        public class SecretString
        {
			public const string PasswordSecretWord = "password";
        }

        public class Conversion
        {
            public const double OneMileInMeters = 1609.34;
        }
    }
}
