using CloudinaryDotNet.Actions;

namespace BlogWebApi.Interfaces
{
    public interface IImageService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file);
        Task<DeletionResult> DeleteImageAsync(string imageUrl);
    }
}
