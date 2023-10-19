using ImageService.Models;

namespace ImageService.Services;

public interface IUserService
{
    Task<ImageUploadResponse> UploadImage(ImageUploadRequest imageStream, string? userId);
    Task<AddFriendResponse> AddFriend(AddFriendRequest model, string? userId);
    Task<GetImagesResponse> GetImages(GetImagesRequest model, string? userId);
}