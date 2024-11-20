
using static Microsoft.AspNetCore.Internal.AwaitableThreadPool;

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

			public static readonly string[] ValidPositions = { DispatcherPositionName, BrokerPositionName, AdminPositionName };
			public static readonly string[] ValidRoles = { AdminRoleName, UserRoleName };
		}

		public static class TempMessages
		{
			public const string LoginWithNewAccount = "Login with your new account";
			public const string LoggedOutOfAccount = "You have been logged out of your account";
			public const string NoRecentChats = "You have no recent chats.";
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

            public const string LoadCouldNotBeRetrieved = "Load could not be retrieved.";
            public const string LoadIdInvalid = "Load id is not valid.";
            public const string ErrorCancellingLoad = "There was an error cancelling the load";

            public const string InvalidCityOrStateSpecified = "Invalid city or state specified.";
            public const string ErrorRetrievingCoordinates = "Error retrieving coordinates.";
            public const string ErrorCreatingLoad = "There was an error creating the load. Please check your input and try again.";
            public const string ErrorRetrievingDetailsForLoad = "Could not retrieve the details for this load.";
            public const string ErrorUpdatingLoad = "Load was not updated !";

            public const string LoadIsNotInCorrectStatus = "This load cannot be posted. It is not with status \"Created\"";
            public const string LoadIsAlreadyPosted = "This load is already posted.";
            public const string ErrorPostingLoad = "An error occured and the load was not posted.";
            public const string ErrorUnpostingLoad = "An error occured and the load was not unposted.";
            public const string ErrorUnpostingLoads = "An error occured and the loads were not unposted.";

            public const string UnableToBookTheLoad = "Unable to book the load.It may no longer be available.";
            public const string UnableToMarkLoadDelivered = "Unable to mark the load as delivered. It may not be in the correct status";

			// USER

			public const string UserCannotBeNull = "User cannot be null.";
			public const string UserNotFound = "The user was not found.";
            public const string UserProfileUpdateFailed = "Failed to update user profile.";
            public const string ErrorUpdatingProfile = "An error occurred while updating the profile.";
            public const string UserNameIsAlreadyTaken = "The username is already taken.";
            public const string EmailIsAlreadyTaken = "The email is already taken.";
            public const string CurrentAndNewPasswordCannotMatch = "The new password cannot be the same as the current password.";

            // PROFILE 

            public const string NoChangesMadeToProfile = "No changes were made to your profile.";


		}

		public static class SuccessMessages
        {

            // LOAD 

            public const string LoadCreatedSuccessfully = "Load was created successfully.";
            public const string LoadUpdatedSuccessfully = "Load was updated successfully.";
            public const string LoadWasNotUpdated = "No changes were made to the load.";
            public const string LoadCancelledSuccessfully = "Load was cancelled successfully.";
            public const string LoadPostedSuccessfully = "Load was posted successfully.";
            public const string LoadUnpostedSuccessfully = "Load was unposted successfully.";
            public const string LoadsUnpostedSuccessfully = "All loads were unposted successfully.";

            public const string LoadWasBookSuccessfully = "Load was booked successfully!";
            public const string LoadWasDeliveredSuccessfully = "Load was delivered successfully!";

			// PROFILE

			public const string ProfileUpdatedSuccessfully = "Your profile was updated successfully.";
            public const string PasswordUpdatedSuccessfully = "Your password has been changed successfully.";


        }

        public static class ActiveTabs
        {

            // LOAD BOARD TABS 

	        public const string CreatedActiveTab = "created";
	        public const string PostedActiveTab = "posted";
	        public const string BookedActiveTab = "booked";


            // PROFILE TABS 
            public const string ProfileActiveTab = "profile";
            public const string ProfileChangePasswordActiveTab = "changePassword";
            public const string ProfileEditActiveTab = "profileEdit";
            public const string ProfileChangePictureActiveTab = "profileChangePicture";
            
        }
        public static class UserImage
        {
	        public const string DefaultImagePath = "/images/default-user-image.png";

	        public const string ImageRemoveSuccessfully = "Profile image removed successfully.";
	        public const string ImageUpdatedSuccessfully = "Profile image updated successfully.";

            public const string ImageMustBeSelected = "Please upload an image first.";
            public const string ImageUploadFailed = "Failed to upload image.";
	        public const string ErrorUpdatingImage = "An error occurred while updating the image: ";
	        public const string ErrorRemovingImage = "Error removing the old profile image: ";
	        public const string ImageUrlExceedsLimit = "Image URL length exceeds limit.";

        }
    }
}
