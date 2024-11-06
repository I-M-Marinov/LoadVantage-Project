using System.ComponentModel.DataAnnotations;

using static LoadVantage.Common.ValidationConstants;
using static LoadVantage.Common.ValidationConstants.UserValidations;

namespace LoadVantage.Core.Models.Profile
{
    public class ProfileViewModel
    {
        [Required]
        public string Id { get; set; } = null!;
        [Required]
        [StringLength(UserNameMaxLength, MinimumLength = UserNameMinLength, ErrorMessage = UserNameLengthNotValid)]
        public string Username { get; set; } = null!;
        [Required]
        [StringLength(EmailMaxLength, MinimumLength = EmailMinLength, ErrorMessage = EmailLengthNotValid)]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(FirstNameMaxLength, MinimumLength = FirstNameMinLength, ErrorMessage = FirstNameLengthNotValid)]
        public string FirstName { get; set; } = null!;
        [Required]
        [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength, ErrorMessage = LastNameLengthNotValid)]
        public string LastName { get; set; } = null!;
        [Required]
        [StringLength(CompanyNameMaxLength, MinimumLength = CompanyNameMinLength, ErrorMessage = CompanyNameLengthNotValid)]
        public string CompanyName { get; set; } = null!;
        [Required]
        [StringLength(PositionMaxLength, MinimumLength = PositionMinLength, ErrorMessage = PositionLengthNotValid)]
        public string Position { get; set; } = null!;
        [Required]
        [StringLength(UserPhoneNumberMaxLength, MinimumLength = UserPhoneNumberMinLength, ErrorMessage = UserPhoneNumberLengthNotValid)]
        public string PhoneNumber { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
    }
}
