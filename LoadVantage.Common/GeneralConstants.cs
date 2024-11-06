
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

            // LOAD 

            public const string LoadInformationCouldNotBeRetrieved = "Error retrieving the load information.";
            public const string LoadCouldNotBeRetrieved = "Load could not be retrieved.";
            public const string LoadIdInvalid = "Load id is not valid.";
            public const string ErrorCancellingLoad = "There was an error cancelling the load";

            public const string InvalidCityOrStateSpecified = "Invalid city or state specified.";
            public const string ErrorRetrievingCoordinates = "Error retrieving coordinates.";
            public const string ErrorCreatingLoad = "There was an error creating the load. Please check your input and try again.";
            public const string ErrorEditingLoad = "Load was not edited succesfully. Please provide valid origin and destination city and state.";
            public const string ErrorUpdatingLoad = "Load was not updated !";

            public const string LoadIsNotInCorrectStatus = "This load cannot be posted. It is not with status \"Created\"";
            public const string LoadIsAlreadyPosted = "This load is already posted.";
            public const string LoadIsNotPosted = "This load is not posted yet.";
            public const string ErrorPostingLoad = "An error occured and the load was not posted.";
            public const string ErrorUnpostingLoad = "An error occured and the load was not unposted.";

            // USER

            public const string UserNotFound = "The user was not found.";
            public const string UserProfileUpdateFailed = "Failed to update user profile.";
            public const string ErrorUpdatingProfile = "An error occurred while updating the profile.";
            public const string UserNameIsAlreadyTaken = "The username is already taken.";

            // BROKER 

            public const string BrokerInformationNotRetrieved = "Broker information could not be retrieved.";

            // NOT ALLOWED

            public const string IdCannotBeChanged = "The Id cannot be changed.";
            public const string PositionCannotBeChanged = "The Position cannot be changed.";
        }

        public static class SuccessMessages
        {
            public const string LoadCreatedSuccessfully = "Load was created successfully.";
            public const string LoadUpdatedSuccessfully = "Load was updated successfully.";
            public const string LoadCancelledSuccessfully = "Load was cancelled successfully.";
            public const string LoadPostedSuccessfully = "Load was posted successfully.";
            public const string LoadUnpostedSuccessfully = "Load was unposted successfully.";

            // PROFILE

            public const string ProfileUpdatedSuccessfully = "Your profile was updated successfully.";

        }
    }
}
