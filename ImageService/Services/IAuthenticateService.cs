using ImageService.Models;

namespace ImageService.Services;

public interface IAuthenticateService
{
    Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
    Task<RegisterResponse> Register(RegisterRequest model);
}