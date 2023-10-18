using ImageService.Models;

namespace ImageService.Services;

public interface IUserService
{
    Task<ITaskResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model);
    Task<ITaskResult<RegisterResponse>> Register(RegisterRequest model);
}