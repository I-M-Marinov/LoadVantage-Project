
using static System.Net.Mime.MediaTypeNames;

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
			public const string PasswordDefaultAfterReset = "password1";

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
            public const string ErrorValidatingTheCityAndState = "Error validating the cities and states.";
            public const string CityAndStateNotProvided = "City and state must be provided.";

			public const string ErrorCreatingLoad = "There was an error creating the load. Please check your input and try again.";
            public const string ErrorUpdatingThisLoad = "An error occurred while trying to update the load.";
            public const string ErrorUpdatingLoad = "Load was not updated !";

            public const string LoadIsNotInCorrectStatus = "This load cannot be posted. It is not with status \"Created\"";
            public const string LoadIsAlreadyPosted = "This load is already posted.";
            public const string ErrorPostingLoad = "An error occured and the load was not posted.";
            public const string ErrorUnpostingLoad = "An error occured and the load was not unposted.";
            public const string ErrorUnpostingLoads = "An error occured and the loads were not unposted.";

            public const string NoPermissionToCancel = "You do not have permission to cancel the carrier for this load.";
            public const string FailedToCancelLoadCarrier = "Failed to cancel the carrier. Please try again.";
            public const string FailedToCancelLoadBroker = "Unable to cancel the booking. Please check the load's status.";

			public const string LoadNotInBookedStatus = "This load is not currently in status Booked.";
			public const string UnableToBookTheLoad = "Unable to book the load.It may no longer be available.";
            public const string UnableToMarkLoadDelivered = "Unable to mark the load as delivered. Ensure the load is in the Booked status.";
            public const string ErrorDeliveringLoad = "There was an error and the load was not marked as delivered.";
            public const string CannotMarkLoadDeliveredWithoutADriver = "There is no driver assigned on the load. You cannot deliver without a driver !";
            public const string NoPermissionToSeeTheLoad = "You do not have permission to see this load.";
            public const string LoadNotFound = "The load you are looking for was not found.";


			// USER

			public const string UserAccountNotFound = "Account was not found.";
			public const string UserCannotBeNull = "User cannot be null.";
			public const string UserNotFound = "The user was not found.";
            public const string UserProfileUpdateFailed = "Failed to update user profile.";
            public const string UserNameIsAlreadyTaken = "The username is already taken.";
            public const string EmailIsAlreadyTaken = "The email is already taken.";
            public const string CurrentAndNewPasswordCannotMatch = "The new password cannot be the same as the current password.";
            public const string UserCreationFailed = "User creation failed.";
            public const string InvalidUserModelOrRoleAdded = "Invalid user model or invalid role selected!";
            public const string InvalidAdminModelOrRoleAdded = "Invalid administrator model or invalid role selected!";
            public const string FailedToUpdateTheUser = "Failed to update the user!";
            public const string InvalidUserType = "User needs to be either a Dispatcher or a Broker.";

            public const string TooManyFailedLoginAttempts = "Too many failed login attempts. You can try again after {0}.";
            public const string RoleAssignmentFailed = "Role assignment failed: {0}";





			public const string EmailCannotBeNull = "Email cannot be null or empty.";
            public const string UserNameCannotBeNull = "Username cannot be null or empty.";
            public const string PasswordCannotBeNull = "Password cannot be null or empty.";
            public const string RoleCannotBeNull = "Role cannot be null or empty.";
            public const string ClaimsCannotBeNull = "Claims cannot be null or empty.";
            public const string ModelCannotBeNull = "The model cannot be null";
			public const string RoleAlreadyAssignedToUser = "That role is already assigned to the user.";




			// SESSION

			public const string InvalidSession = "Your session is invalid. Please log in again.";

			// PROFILE 

			public const string NoChangesMadeToProfile = "No changes were made to your profile.";

			public const string NoPermissionToView = "You do not have permission to access this system. Please register an account.";

			// TRUCK 

			public const string TruckNotFound = "Truck was not found";
			public const string TruckWasNotCreated = "The truck was not created successfully.";
            public const string TruckCreateError = "Error ! Truck was not created.";
            public const string TruckDoesNotExist = "The truck you are looking for does not exist.";
            public const string TruckCannotBeParkedRightNow = "Error ! Truck cannot be parked right now!";

            public const string InvalidTruckOrDriver = "Invalid truck or driver selection.";
            public const string ErrorAssigningDriverToTruck = "An error occurred while assigning the driver to the truck.";
            public const string CannotDeleteTruckInUse = "You cannot delete a truck that is currently in use !";


			// DRIVER

			public const string DriverWasNotFound = "Driver was not found.";
			public const string DriverWasNotCreated = "The driver was not created successfully.";
			public const string DriverCreateError = "Error ! Driver was not created.";
			public const string DriverDoesNotExist = "The driver you are looking for does not exist.";
			public const string DriverWasNotAssignedToTheLoad = "Failed to assign the driver to the load. Please check the details and try again.";
			public const string DriverCurrentlyUnderALoad = "Driver is currently doing a load and cannot leave the truck.";
			public const string DriverAlreadyOnAnotherLoad = "Status change unsuccessful ! Driver might already be on another load.";
			public const string ErrorRetrievingDrivers = "An error occurred while retrieving drivers.";

			// IMAGE 

			public const string ImageWasNotDeletedSuccessfully = "Image deletion failed.";



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
            public const string LoadWasRestoredSuccessfully = "The load was restored successfully.";
            public const string LoadWasMovedBackToBookedSuccessfully = "The load's status was changed successfully back to Booked";
            public const string LoadRepostedAgain = "The load is reposted again looking for a carrier.";
            public const string LoadReturnedToBrokerSuccessfully = "Load was successfully returned to the broker.";


			public const string LoadWasBookSuccessfully = "Load was booked successfully!";
            public const string LoadWasDeliveredSuccessfully = "Load was delivered successfully!";

			// PROFILE

			public const string ProfileUpdatedSuccessfully = "Your profile was updated successfully.";
            public const string PasswordUpdatedSuccessfully = "Your password has been changed successfully.";

            // TRUCK 

            public const string TruckWasAddedSuccessfully = "The truck was added successfully to your fleet.";
            public const string TruckWasUpdatedSuccessfully = "Truck was updated successfully.";
            public const string TruckWasRemovedSuccessfully = "Truck was removed/decommissioned from your fleet successfully.";
            public const string TruckParkedAtTheYard = "Driver unassigned successfully! Truck is parked at the yard.";

            public const string DriverAssignedToTruckSuccessfully = "Driver assigned to the truck successfully!";

			// DRIVER 

			public const string DriverWasAddedSuccessfully = "The driver was added to your company successfully.";
            public const string DriverWasUpdatedSuccessfully = "The driver was updated successfully.";
            public const string DriverWasFiredSuccessfully = "The driver was fired successfully.";
            public const string DriverWasAssignedToLoadSuccessfully = "Driver assigned to the load successfully!";

			// IMAGE 

			public const string ImageWasDeletedSuccessfully = "Image deleted successfully.";

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
	        public const string DefaultPublicId = "default-public-id";
	        public static readonly Guid DefaultImageId = new Guid("00000000-0000-0000-0000-000000000001");


	        public const string ImageRemoveSuccessfully = "Profile image removed successfully.";
	        public const string ImageUpdatedSuccessfully = "Profile image updated successfully.";

            public const string ImageMustBeSelected = "Please upload an image first.";
            public const string ImageUploadFailed = "Failed to upload image.";
	        public const string ErrorUpdatingImage = "An error occurred while updating the image: ";
	        public const string ErrorRemovingImage = "Error removing the old profile image: ";
	        public const string ImageUrlExceedsLimit = "Image URL length exceeds limit.";
        }

        public static class AdministratorManagement
        {
	        public const string NewUserCreatedSuccessfully = "New user with username --> {0} and name {1} was created successfully!";
	        public const string NewAdministratorCreatedSuccessfully = "New administrator with username --> {0} and name {1} was created successfully!";

	        public const string UserDeactivatedSuccessfully = "User was deactivated successfully.";
	        public const string UserActivatedSuccessfully = "User was activated successfully.";
	        public const string UserUpdatedSuccessfully = "User details updated successfully.";
	        public const string UserPasswordResetSuccessfully = "User's password was successfully reset to the default user password";

	        public const string CannotDeactivateBroker = "Cannot deactivate the broker as they have loads currently in transit.";
	        public const string CannotDeactivateDispatcher = "Cannot deactivate the dispatcher as they have drivers currently on a job.";



			public const string FailedToDeactivateTheUser = "Failed to deactivate the user.";
	        public const string FailedToReactivateThisAccount = "Failed to reactivate the user.";
	        

	        public const string ErrorTryLater = "An unkown error occurred. Please try again later.";

	        public const string ResetPasswordFailed = "Resetting the password for this user failed.";



        }
	}
}
