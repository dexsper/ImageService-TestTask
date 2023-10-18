using ImageService.Models;

namespace ImageService.Services;

public interface IImageService
{
    Task<TaskResult<Image>> PutImage(User user, ImageUploadRequest model);
    Task<TaskResult<List<string>>> GetImages(User user);
}