using System.ComponentModel.DataAnnotations;
using LoadVantage.Core.ValidationAttributes;
using static LoadVantage.Common.ValidationConstants.UserImageValidations;
using static LoadVantage.Common.GeneralConstants.UserImage;

namespace LoadVantage.Areas.Admin.Models
{
	public class AdminImageFileUploadModel
	{
		[Required(ErrorMessage = ImageMustBeSelected)]
		[AllowedExtensions(isAdmin: true,ErrorMessage = AdminInvalidImageFileExtension)]
		[FileSize(MaxSizeInBytes = UserImageMaxFileSize, ErrorMessage = ImageFileSizeExceeded)]
		public IFormFile FormFile { get; set; } = null!;
	}
}
