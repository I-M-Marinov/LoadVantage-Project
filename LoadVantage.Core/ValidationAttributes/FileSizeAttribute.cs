using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace LoadVantage.Core.ValidationAttributes
{
	public class FileSizeAttribute : ValidationAttribute
	{
		public int MaxSizeInBytes { get; set; }

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var file = value as IFormFile;

			if (file != null && file.Length > MaxSizeInBytes)
			{
				return new ValidationResult(ErrorMessage ?? $"File size should not exceed {MaxSizeInBytes / (1024 * 1024)} MB.");
			}

			return ValidationResult.Success;
		}
	}
}
