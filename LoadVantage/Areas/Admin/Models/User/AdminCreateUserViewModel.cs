using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.ValidationConstants.UserValidations;
using static LoadVantage.Common.ValidationConstants.EditProfile;

namespace LoadVantage.Areas.Admin.Models.User
{
	public class AdminCreateUserViewModel
	{
		public string Id { get; set; } = null!;
		public string? Role { get; set; }
		[Required(ErrorMessage = UsernameRequired)]
		[StringLength(UserNameMaxLength, MinimumLength = UserNameMinLength, ErrorMessage = UserNameLengthNotValid)]
		public required string UserName { get; set; }
		[Required(ErrorMessage = EmailAddressRequired)]
		[StringLength(EmailMaxLength, MinimumLength = EmailMinLength, ErrorMessage = EmailLengthNotValid)]
		[EmailAddress(ErrorMessage = EmailAddressNotValid)]
		[RegularExpression(EmailRegexPattern, ErrorMessage = EmailAddressNotValid)]
		public required string Email { get; set; }
		[Required(ErrorMessage = FirstNameRequired)]
		[StringLength(FirstNameMaxLength, MinimumLength = FirstNameMinLength, ErrorMessage = FirstNameLengthNotValid)]
		public required string FirstName { get; set; }
		[Required(ErrorMessage = LastNameRequired)]
		[StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength, ErrorMessage = LastNameLengthNotValid)]
		public required string LastName { get; set; }
		[Required(ErrorMessage = CompanyNameRequired)]
		[StringLength(CompanyNameMaxLength, MinimumLength = CompanyNameMinLength, ErrorMessage = CompanyNameLengthNotValid)]
		public required string CompanyName { get; set; }
		[Required]
		public required string Position { get; set; }
		[Required]
		[StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength, ErrorMessage = PasswordLengthNotValid)]
		[DataType(DataType.Password)]
		public required string Password { get; set; }

		[Required(ErrorMessage = PhoneNumberRequired)]
		[StringLength(UserPhoneNumberMaxLength, MinimumLength = UserPhoneNumberMinLength, ErrorMessage = UserPhoneNumberLengthNotValid)]
		[RegularExpression(PhoneNumberRegexPattern, ErrorMessage = UserPhoneNumberEmailAddressNotValid)]
		public string PhoneNumber { get; set; } = string.Empty;
		[Url]
		[StringLength(UserImageMaxLength, ErrorMessage = ImageUrlExceedsLimit)]
		public string? UserImageUrl { get; set; }
		public string FullName => $"{FirstName} {LastName}";
	}
}
