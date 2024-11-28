using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.ValidationConstants.UserValidations;
using static LoadVantage.Common.ValidationConstants.EditProfile;


namespace LoadVantage.Core.Models.Profile
{
	public class ChangePasswordViewModel
	{
		[Required(ErrorMessage = "Current password is required.")]
		[DataType(DataType.Password)]
		[Display(Name = "Current Password")]
		public string CurrentPassword { get; set; } = null!;

		[Required(ErrorMessage = "New password is required.")]
		[StringLength(NewPasswordMaxLength, MinimumLength = NewPasswordMinLength, ErrorMessage = NewPasswordLengthNotValid)]
		[DataType(DataType.Password)]
		[Display(Name = "New Password")]
		public string NewPassword { get; set; } = null!;

		[Required(ErrorMessage = "Confirmation of the new password is required.")]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm New Password")]
		[Compare("NewPassword", ErrorMessage = NewAndConfirmPasswordDoNotMatch)]
		public string ConfirmPassword { get; set; } = null!;
	}
}
