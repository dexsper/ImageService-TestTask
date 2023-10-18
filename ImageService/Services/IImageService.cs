using ImageService.Models;

namespace ImageService.Services;

public interface IImageService
{
    Task<TaskResult<Image>> PutImage(User user, ImageUploadRequest model);
}