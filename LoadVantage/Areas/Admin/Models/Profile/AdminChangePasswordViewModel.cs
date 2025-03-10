﻿using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.ValidationConstants.UserValidations;
using static LoadVantage.Common.ValidationConstants.EditProfile;

namespace LoadVantage.Areas.Admin.Models.Profile
{
    public class AdminChangePasswordViewModel
    {

        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength, ErrorMessage = NewPasswordLengthNotValid)]
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
