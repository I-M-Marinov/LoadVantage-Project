using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.ValidationConstants.UserValidations;
using static LoadVantage.Common.ValidationConstants.EditProfile;


namespace LoadVantage.Core.Models.Profile
{
	public class ChangePasswordViewModel
	{
		[Required(ErrorMessage = PasswordRequired)]
		[DataType(DataType.Password)]
		[Display(Name = "Current Password")]
		public string CurrentPassword { get; set; } = null!;

		[Required(ErrorMessage = NewPasswordRequired)]
		[StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength, ErrorMessage = NewPasswordLengthNotValid)]
		[DataType(DataType.Password)]
		[Display(Name = "New Password")]
		public string NewPassword { get; set; } = null!;

		[Required(ErrorMessage = ConfirmationPasswordRequired)]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm New Password")]
		[Compare("NewPassword", ErrorMessage = NewAndConfirmPasswordDoNotMatch)]
		public string ConfirmPassword { get; set; } = null!;
	}
}
