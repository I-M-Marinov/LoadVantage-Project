using System.ComponentModel.DataAnnotations;
using LoadVantage.Core.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using static LoadVantage.Common.ValidationConstants.UserImageValidations;
using static LoadVantage.Common.GeneralConstants.UserImage;

namespace LoadVantage.Core.Models.Image
{
	public class ImageFileUploadModel
	{
		[Required(ErrorMessage = ImageMustBeSelected)]
		[AllowedExtensions(ErrorMessage = InvalidImageFileExtension)]
		[FileSize(MaxSizeInBytes = UserImageMaxFileSize, ErrorMessage = ImageFileSizeExceeded)]
		public IFormFile FormFile { get; set; } = null!;

	}

}