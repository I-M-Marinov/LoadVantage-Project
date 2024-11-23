using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LoadVantage.Core.Models.Image;

namespace LoadVantage.Core.Contracts
{
	public interface IImageService
	{
		/// <summary>
		/// Uploads an image to Cloudinary ( sends the 
		/// </summary>
		Task<ImageUploadResult> UploadImageAsync(IFormFile file);
		/// <summary>
		/// Sends a publicId to Cloudinary to delete the image
		/// </summary>
		Task<DeleteImageResult> DeleteImageAsync(string publicId);

    }
}
