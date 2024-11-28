using System.ComponentModel.DataAnnotations;

using LoadVantage.Core.Models.Image;
using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.ValidationConstants.UserValidations;
using static LoadVantage.Common.ValidationConstants.EditProfile;

namespace LoadVantage.Core.Models.Profile
{
    public class ProfileViewModel : IValidatableObject
    {
        public  string Id { get; set; } = null!;
        [Required(ErrorMessage = UsernameRequired)]
        [StringLength(UserNameMaxLength, MinimumLength = UserNameMinLength, ErrorMessage = UserNameLengthNotValid)]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = EmailAddressRequired)]
        [StringLength(EmailMaxLength, MinimumLength = EmailMinLength, ErrorMessage = EmailLengthNotValid)]
        [EmailAddress(ErrorMessage = EmailAddressNotValid)]
        [RegularExpression(EmailRegexPattern, ErrorMessage = EmailAddressNotValid)]

		public string Email { get; set; } = null!;
        [Required(ErrorMessage = FirstNameRequired)]
        [StringLength(FirstNameMaxLength, MinimumLength = FirstNameMinLength, ErrorMessage = FirstNameLengthNotValid)]
        public  string FirstName { get; set; } = null!;
        [Required(ErrorMessage = LastNameRequired)]
        [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength, ErrorMessage = LastNameLengthNotValid)]
		public  string LastName { get; set; } = null!;
        [Required(ErrorMessage = CompanyNameRequired)]
        [StringLength(CompanyNameMaxLength, MinimumLength = CompanyNameMinLength, ErrorMessage = CompanyNameLengthNotValid)]
        public  string CompanyName { get; set; } = null!;
        [Required]
        [StringLength(PositionMaxLength, MinimumLength = PositionMinLength, ErrorMessage = PositionLengthNotValid)]
        public  string Position { get; set; } = null!;
        [Required(ErrorMessage = PhoneNumberRequired)]
        [StringLength(UserPhoneNumberMaxLength, MinimumLength = UserPhoneNumberMinLength, ErrorMessage = UserPhoneNumberLengthNotValid)]
        [RegularExpression(PhoneNumberRegexPattern, ErrorMessage = UserPhoneNumberEmailAddressNotValid)]

		public string PhoneNumber { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
		[Url]
		[StringLength(UserImageMaxLength, ErrorMessage = ImageUrlExceedsLimit)]
		public string? UserImageUrl { get; set; }

		public ChangePasswordViewModel? ChangePasswordViewModel { get; set; } 
		public ImageFileUploadModel? ImageFileUploadModel { get; set; } 

		// Custom validation for the different partial views
		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var results = new List<ValidationResult>();

			bool isEditingProfile = ChangePasswordViewModel == null && ImageFileUploadModel == null;
			bool isChangingPassword = ChangePasswordViewModel != null;
			bool isUploadingImage = ImageFileUploadModel?.FormFile != null;

			if (isEditingProfile)
			{
				Validator.TryValidateProperty(Id, new ValidationContext(this) { MemberName = nameof(Id) }, results);
				Validator.TryValidateProperty(Username, new ValidationContext(this) { MemberName = nameof(Username) }, results);
				Validator.TryValidateProperty(Email, new ValidationContext(this) { MemberName = nameof(Email) }, results);
				Validator.TryValidateProperty(FirstName, new ValidationContext(this) { MemberName = nameof(FirstName) }, results);
				Validator.TryValidateProperty(LastName, new ValidationContext(this) { MemberName = nameof(LastName) }, results);
				Validator.TryValidateProperty(CompanyName, new ValidationContext(this) { MemberName = nameof(CompanyName) }, results);
				Validator.TryValidateProperty(Position, new ValidationContext(this) { MemberName = nameof(Position) }, results);
				Validator.TryValidateProperty(PhoneNumber, new ValidationContext(this) { MemberName = nameof(PhoneNumber) }, results);
			}
			else if (isChangingPassword)
			{
				// Validate ChangePasswordViewModel only
				if (ChangePasswordViewModel != null)
				{
					var passwordResults = new List<ValidationResult>();
					Validator.TryValidateObject(ChangePasswordViewModel, new ValidationContext(ChangePasswordViewModel), passwordResults, true);
					results.AddRange(passwordResults);
				}
			}
			else if (isUploadingImage)
			{
				// Validate ImageFileUploadModel only
				if (ImageFileUploadModel != null)
				{
					var imageResults = new List<ValidationResult>();
					Validator.TryValidateObject(ImageFileUploadModel, new ValidationContext(ImageFileUploadModel), imageResults, true);
					results.AddRange(imageResults);
				}
			}

			return results;
		}

	}
}
