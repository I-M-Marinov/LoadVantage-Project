using System.ComponentModel.DataAnnotations;

using static LoadVantage.Common.ValidationConstants;
using static LoadVantage.Common.ValidationConstants.UserValidations;

namespace LoadVantage.Core.Models.Profile
{
    public class ProfileViewModel
    {
        [Required]
        public required string Id { get; set; } = null!;
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(UserNameMaxLength, MinimumLength = UserNameMinLength, ErrorMessage = UserNameLengthNotValid)]
        public required string Username { get; set; } = null!;
        [Required(ErrorMessage = "Email name is required.")]
        [StringLength(EmailMaxLength, MinimumLength = EmailMinLength, ErrorMessage = EmailLengthNotValid)]
        public required string Email { get; set; } = null!;
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(FirstNameMaxLength, MinimumLength = FirstNameMinLength, ErrorMessage = FirstNameLengthNotValid)]
        public required string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength, ErrorMessage = LastNameLengthNotValid)]
        public required string LastName { get; set; } = null!;
        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(CompanyNameMaxLength, MinimumLength = CompanyNameMinLength, ErrorMessage = CompanyNameLengthNotValid)]
        public required string CompanyName { get; set; } = null!;
        [Required]
        [StringLength(PositionMaxLength, MinimumLength = PositionMinLength, ErrorMessage = PositionLengthNotValid)]
        public required string Position { get; set; } = null!;
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(UserPhoneNumberMaxLength, MinimumLength = UserPhoneNumberMinLength, ErrorMessage = UserPhoneNumberLengthNotValid)]
        public required string PhoneNumber { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
    }
}
