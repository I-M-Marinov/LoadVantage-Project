using System.ComponentModel.DataAnnotations;
using LoadVantage.Core.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using static LoadVantage.Common.ValidationConstants.UserImageValidations;

namespace LoadVantage.Core.Models.Image
{
	public class ImageFileUploadModel
	{
		[Required(ErrorMessage = "Please select an image.")]
		[FileExtensions(Extensions = UserImageValidFileExtensions, ErrorMessage = InvalidImageFileExtension)]
		[FileSize(MaxSizeInBytes = UserImageMaxFileSize, ErrorMessage = ImageFileSizeExceeded)]
		public IFormFile FormFile { get; set; } = null!;
	}

}