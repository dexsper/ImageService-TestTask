using ImageService.Models;

namespace ImageService.Services;

public interface IImageService
{
    Task<string> PutImage(User user, ImageUploadRequest model);
    Task<List<string>> GetImages(User user);
}

public class StorageException : Exception
{
    public StorageException(string? message) : base(message)
    {
        
    }
}
