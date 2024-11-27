using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.GeneralConstants.UserImage;
using static LoadVantage.Common.ValidationConstants.UserValidations;

namespace LoadVantage.Areas.Admin.Models
{
    public class AdminProfileViewModel
    {

        public string Id { get; set; } = null!;

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
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = LastNameRequired)]
        [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength, ErrorMessage = LastNameLengthNotValid)]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = CompanyNameRequired)]
        [StringLength(CompanyNameMaxLength, MinimumLength = CompanyNameMinLength,
            ErrorMessage = CompanyNameLengthNotValid)]
        public string CompanyName { get; set; } = null!;

        [Required]
        [StringLength(PositionMaxLength, MinimumLength = PositionMinLength, ErrorMessage = PositionLengthNotValid)]
        public string Position { get; set; } = null!;

        [Required(ErrorMessage = PhoneNumberRequired)]
        [StringLength(UserPhoneNumberMaxLength, MinimumLength = UserPhoneNumberMinLength,
            ErrorMessage = UserPhoneNumberLengthNotValid)]
        [RegularExpression(PhoneNumberRegexPattern, ErrorMessage = UserPhoneNumberEmailAddressNotValid)]

        public string PhoneNumber { get; set; } = null!;

        public string FullName => $"{FirstName} {LastName}";

        [Url]
        [StringLength(UserImageMaxLength, ErrorMessage = ImageUrlExceedsLimit)]
        public string? UserImageUrl { get; set; }

        public AdminChangePasswordViewModel? AdminChangePasswordViewModel { get; set; }
        public AdminImageFileUploadModel? AdminImageFileUploadModel { get; set; }


    }
}
