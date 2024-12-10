using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.ValidationConstants.UserValidations;
using static LoadVantage.Common.ValidationConstants.EditProfile;

namespace LoadVantage.Core.Models.Account
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength, ErrorMessage = PasswordLengthNotValid)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;


        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = PasswordAndConfirmPasswordDoNotMatch)]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = null!;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Required]
        [Display(Name = "Company name")]
        public string Company { get; set; } = null!;

        [Required]
        [Display(Name = "Position")]
        public string Position { get; set; } = null!; // Stores either "Dispatcher" or "Broker"

        [Required] 
        [Display(Name = "Role")]
        public string Role = UserRoleName; // default to "User"

    }
}
