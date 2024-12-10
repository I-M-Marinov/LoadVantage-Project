namespace LoadVantage.Common
{
	public static class ValidationConstants
	{

		// VALIDATION MESSAGES 

		public const string EmailAlreadyExists = "An account with this email already exists.";
		public const string UserNameAlreadyExists = "An account with this username already exists.";
		public const string InvalidUserNameOrPassword = "Invalid username or password.";
		public const string ThisAccountIsInactive = "The account you are trying to log in with has been deactivated.";
		public const string InvalidPositionSelected = "The selected position is invalid.";
		public const string InvalidRoleSelected = "The selected role is not a valid one.";

        public static class LoadValidations
        {
            // LOAD 

            public const byte LoadOriginCityMinLength = 2;
            public const byte LoadOriginCityMaxLength = 30;

            public const byte LoadDestinationCityMinLength = 2;
            public const byte LoadDestinationCityMaxLength = 30;

            public const byte StateMinMaxLength = 2;

            public const double LoadPriceMinValue = 1.00;
            public const double LoadPriceMaxValue = 99999.99;

            public const double WeightMinValue = 1.00;
            public const double WeightMaxValue = 48000.00;

            public const string OriginCityRequired = "Origin city is required.";
            public const string OriginStateRequired = "Origin state is required.";
            public const string DestinationCityRequired = "Destination city is required.";
            public const string DestinationStateRequired = "Destination state is required.";
            public const string PickupTimeRequired = "Pickup time is required.";
            public const string DeliveryTimeRequired = "Delivery time is required.";
            public const string PriceRequired = "Price is required.";
            public const string WeightRequired = "Weight is required.";

            public const string OriginCityLengthNotValid = "Origin city should be between 2 and 30 characters";
            public const string DestinationCityLengthNotValid = "Destiantion city should be between 2 and 30 characters";
            public const string StateLengthNotValid = "State should be a 2-letter code.";
            public const string PickupTimeInvalidFormat = "Invalid date format for Pickup Time.";
            public const string DeliveryTimeInvalidFormat = "Invalid date format for Delivery Time.";
            public const string DeliveryTimeCannotBeBeforePickupTime = "Delivery time must be after the pickup time.";
            public const string PriceRangeInvalid = "Price needs to be between 1.00 and 99999.99";
            public const string WeightRangeInvalid = "Weight needs to be between 1.00 and 48000.00";

        }

        public static class UserValidations
        {


	        // USERNAME

			public const byte UserNameMinLength = 4;
	        public const byte UserNameMaxLength = 30;


	        // POSITION

			public const byte PositionMaxLength = 20;


	        // EMAIL

			public const byte EmailMinLength = 7;
	        public const byte EmailMaxLength = 50;
	        public const string EmailRegexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(?:\.[a-zA-Z]{2,})?$";


	        // FIRST NAME

			public const byte FirstNameMinLength = 2;
	        public const byte FirstNameMaxLength = 25;

	        // LAST NAME

			public const byte LastNameMinLength = 3;
	        public const byte LastNameMaxLength = 25;

	        // COMPANY NAME

			public const byte CompanyNameMinLength = 3;
	        public const byte CompanyNameMaxLength = 50;

			// PHONE NUMBER 

			public const byte UserPhoneNumberMinLength = 7;
	        public const byte UserPhoneNumberMaxLength = 15;
	        public const string PhoneNumberRegexPattern = @"^\+?(\(\d{1,4}\)|\d{1,4})([-\s]?\d{1,4}){1,3}$";

			// USER IMAGE 

			public const int UserImageMaxLength = 1000;
            public const int UserImagePublicIdMaxLength = 1000;

			// CHAT MESSAGES 

			public const int MessageMaxValue = 2000;

			// PASSWORD 

			public const byte PasswordMinLength = 6;
	        public const byte PasswordMaxLength = 30;



        }

        public static class EditProfile
        {
	        public const string UsernameRequired = "Username is required.";
	        public const string EmailAddressRequired = "Email address is required.";
	        public const string FirstNameRequired = "First name is required.";
	        public const string LastNameRequired = "Last name is required.";
	        public const string CompanyNameRequired = "Company name is required.";
	        public const string PhoneNumberRequired = "Phone number is required.";


	        public const string PasswordRequired = "Current password is required.";
	        public const string NewPasswordRequired = "New password is required.";
	        public const string ConfirmationPasswordRequired = "Confirmation of the new password is required.";


			public const string EmailLengthNotValid = "The length of the email must be between 7 and 50 characters.";
	        public const string EmailAddressNotValid = "Invalid email address format.";
	        public const string CompanyNameLengthNotValid = "The length of the company name must be between 3 and 50 characters.";
	        public const string UserPhoneNumberLengthNotValid = "The length of the phone number must be between 7 and 15 characters.";
	        public const string UserPhoneNumberEmailAddressNotValid = "Invalid phone number format - Accepted formats: (123) 456-7890 or +1-234-567-8900";
	        public const string UserNameLengthNotValid = "The length of the username must be between 4 and 30 characters.";
	        public const string FirstNameLengthNotValid = "The length of the firstname must be between 2 and 25 characters.";
	        public const string LastNameLengthNotValid = "The length of the lastname must be between 3 and 25 characters.";
	        public const string NewPasswordLengthNotValid = "The new password must be between 6 and 30 characters long.";
	        public const string NewAndConfirmPasswordDoNotMatch = "The new password and confirmation password do not match.";
	        public const string PasswordAndConfirmPasswordDoNotMatch = "The password and confirmation password do not match.";

			// USER CREATION 

			public const string PasswordLengthNotValid = "The new password must be between 6 and 30 characters long.";

		}

		public static class UserImageValidations
        {

            public static readonly string[] ValidImageExtensions = new [] { ".jpg", ".jpeg", ".png" };
            public static readonly string[] AdminValidImageExtensions = new [] { ".jpg", ".jpeg", ".png", ".gif" };
            public const int UserImageMaxFileSize = 10485760; // in bytes ( or 10 * 1024 * 1024 )
            public const string InvalidImageFileExtension = "Invalid file extension. Only JPG, JPEG and PNG files are allowed.";
            public const string AdminInvalidImageFileExtension = "Invalid file extension. Only JPG, JPEG, PNG and GIF files are allowed.";
            public const string ImageFileSizeExceeded = "File size must not exceed 10 MegaBytes.";

		}

        public static class TruckValidations
        {

	        public const string TruckNumberRequired = "The truck number is required.";
	        public const string TruckMakeRequired = "The truck make is required.";
	        public const string TruckModelRequired = "The truck model is required.";
	        public const string TruckYearRequired = "The truck production year is required.";
	        public const string AvailabilityRequired = "Availability is required.";

	        public const string TruckNumberInvalid = "Truck number must be 3-7 characters long and contain only letters and digits.";
	        public const string TruckMakeInvalid = "Truck make must be between 2 and 30 characters.";
	        public const string TruckModelInvalid = "Truck model must be between 1 and 50 characters.";
	        public const string TruckYearInvalid = "Truck year must be between 1990 and 2024.";

	        public const string TruckNumberRegexPattern = @"^[a-zA-Z0-9]{3,7}$";

			public const byte TruckMakeMinLength = 2;
			public const byte TruckMakeMaxLength = 30;

			public const byte TruckModelMinLength = 1;
			public const byte TruckModelMaxLength = 50;

			public const int TruckYearMinValue = 1990;
			public const int TruckYearMaxValue = 2024; 

		}

        public static class DriverValidations
        {

	        public const string DriverFirstNameRequired = "The driver's first name is required.";
	        public const string DriverLastNameRequired = "The driver's last name is required.";
	        public const string DriverLicenseNumberRequired = "The driver's license number is required.";

	        public const string DriverFirstNameInvalid = "First name must be between 2 and 25 characters.";
	        public const string DriverLastNameInvalid = "Last name must be between 3 and 25 characters.";
	        public const string DriverLicenseNumberInvalid = "License number must be between 8 and 15 characters ( not special )";
	       

			public const byte DriverFirstNameMinLength = 2;
	        public const byte DriverFirstNameMaxLength = 25;

	        public const byte DriverLastNameMinLength = 3;
	        public const byte DriverLastNameMaxLength = 25;

	        public const byte LicenseNumberMaxLength = 15;
			public const string DriverLicenceNumberRegexPattern = @"^[A-Za-z0-9]{8,15}$";

		}
	}
}
