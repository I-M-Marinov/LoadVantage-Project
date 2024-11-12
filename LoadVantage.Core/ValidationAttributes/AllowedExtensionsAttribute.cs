using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.GeneralConstants;
using static LoadVantage.Common.ValidationConstants.UserImageValidations;

namespace LoadVantage.Core.ValidationAttributes
{
	public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private static readonly string[] DefaultExtensions = ValidImageExtensions;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var file = value as IFormFile;
			if (file != null)
			{
				var extension = Path.GetExtension(file.FileName);
				if (!DefaultExtensions.Contains(extension.ToLower()))
				{
					return new ValidationResult(ErrorMessage ?? "Invalid file extension.");
				}
			}

			return ValidationResult.Success!;
		}
	}
}
