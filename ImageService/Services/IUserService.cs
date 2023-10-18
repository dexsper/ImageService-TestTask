using ImageService.Models;
using ImageService.Schemas;

namespace ImageService.Services;

public interface IUserService
{
    Task<TaskResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model);
    Task<TaskResult<RegisterResponse>> Register(RegisterRequest model);
    Task<TaskResult<ImageUploadResponse>> UploadImage(ImageUploadRequest imageStream, string userId);
    Task<TaskResult<AddFriendResponse>> AddFriend(AddFriendRequest model, string userId);
    Task<TaskResult<GetImagesResponse>> GetImages(GetImagesRequest model, string userId);
}