
namespace LoadVantage.Common
{
	public static class GeneralConstants
	{
		public static class UserRoles
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

		public static class TempMessages
		{
			public const string LoginWithNewAccount = "Login with your new account";
			public const string LoggedOutOfAccount = "You have been logged out of your account";
		}

        public static class SecretString
        {
			public const string PasswordSecretWord = "password";
        }

        public static class Conversion
        {
            public const double OneMileInMeters = 1609.34;
        }

        public static class ErrorMessages
        {
            public const string LoadInformationCouldNotBeRetrieved = "Error retrieving the load information.";
            public const string LoadCouldNotBeRetrieved = "Load could not be retrieved.";
            public const string LoadIdInvalid = "Load id is not valid.";
            public const string ErrorCreatingLoad = "There was an error creating the load. Please check your input and try again.";
        }

        public static class SuccessMessages
        {
            public const string LoadCancelledSuccessfully = "Load was cancelled successfully.";
        }
    }
}
