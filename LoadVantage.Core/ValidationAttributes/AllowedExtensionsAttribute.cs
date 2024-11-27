using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.GeneralConstants;
using static LoadVantage.Common.ValidationConstants.UserImageValidations;

namespace LoadVantage.Core.ValidationAttributes
{
	public class AllowedExtensionsAttribute : ValidationAttribute
    {
		private readonly string[] _validExtensions;

		public AllowedExtensionsAttribute()
		{
			_validExtensions = ValidImageExtensions;
		}
		public AllowedExtensionsAttribute(bool isAdmin)
        {
	        _validExtensions = isAdmin ? AdminValidImageExtensions : ValidImageExtensions;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
	        var file = value as IFormFile;

	        if (file != null)
	        {
		        var extension = Path.GetExtension(file.FileName).ToLower();
		        if (!_validExtensions.Contains(extension))
		        {
			        return new ValidationResult(ErrorMessage ?? "Invalid file extension.");
		        }
	        }

	        return ValidationResult.Success!;
        }
	}
}
