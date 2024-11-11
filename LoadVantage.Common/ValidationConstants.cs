namespace LoadVantage.Common
{
	public static class ValidationConstants
	{
		// DRIVER 

		public const byte LicenseNumberMinLength = 5;
		public const byte LicenseNumberMaxLength = 15;


		// VALIDATION MESSAGES 

		public const string EmailAlreadyExists = "An account with this email already exists.";
		public const string UserNameAlreadyExists = "An account with this username already exists.";
		public const string InvalidUserNameOrPassword = "Invalid username or password.";
		public const string InvalidPositionSelected = "The selected position is invalid.";

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

	        public const byte UserNameMinLength = 4;
	        public const byte UserNameMaxLength = 30;

	        public const byte PositionMinLength = 6;
	        public const byte PositionMaxLength = 20;

	        public const byte EmailMinLength = 7;
	        public const byte EmailMaxLength = 50;

	        public const byte FirstNameMinLength = 2;
	        public const byte FirstNameMaxLength = 25;

	        public const byte LastNameMinLength = 3;
	        public const byte LastNameMaxLength = 25;

	        public const byte CompanyNameMinLength = 3;
	        public const byte CompanyNameMaxLength = 50;

	        public const byte UserPhoneNumberMinLength = 7;
	        public const byte UserPhoneNumberMaxLength = 15;

            // USER IMAGE 

            public const int UserImageMaxLength = 1000;
            public const int UserImagePublicIdMaxLength = 1000;



	        // NEW PASSWORD 

	        public const byte NewPasswordMinLength = 6;
	        public const byte NewPasswordMaxLength = 30;

			public const string EmailLengthNotValid = "The length of the email must be between 7 and 50 characters.";
            public const string CompanyNameLengthNotValid = "The length of the company name must be between 3 and 50 characters.";
            public const string UserPhoneNumberLengthNotValid = "The length of the phone number must be between 7 and 15 characters.";
            public const string UserNameLengthNotValid = "The length of the username must be between 4 and 30 characters.";
            public const string FirstNameLengthNotValid = "The length of the firstname must be between 2 and 25 characters.";
            public const string LastNameLengthNotValid = "The length of the lastname must be between 3 and 25 characters.";
            public const string PositionLengthNotValid = "The length of the postion must be between 6 and 20 characters.";
            public const string NewPasswordLengthNotValid = "The new password must be between 6 and 30 characters long.";
            public const string NewAndConfirmPasswordDoNotMatch = "The new password and confirmation password do not match.";


        }

        public static class UserImageValidations
        {
	        public const int UserImageMaxFileSize = 10485760; // in bytes ( or 10 * 1024 * 1024 )
            public const string InvalidImageFileExtension = "Invalid file extension. Only JPG, JPEG and PNG files are allowed.";
            public const string ImageFileSizeExceeded = "File size must not exceed 10 MegaBytes.";

		}
	}
}
