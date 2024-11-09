using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LoadVantage.Core.Models.Image;

namespace LoadVantage.Core.Contracts
{
	public interface IImageService
	{
		Task<ImageUploadResult> UploadImageAsync(IFormFile file);
        Task<DeleteImageResult> DeleteImageAsync(string publicId);

    }
}
