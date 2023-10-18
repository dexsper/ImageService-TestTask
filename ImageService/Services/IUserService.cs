using ImageService.Models;

namespace ImageService.Services;

public interface IUserService
{
    Task<TaskResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model);
    Task<TaskResult<RegisterResponse>> Register(RegisterRequest model);
    Task<TaskResult<ImageUploadResult>> UploadImage(ImageUploadRequest imageStream, string userId);
}