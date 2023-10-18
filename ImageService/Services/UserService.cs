using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ImageService.Data;
using ImageService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ImageService.Services;

public class UserService : IUserService
{
    private readonly IImageService _imageService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger _logger;

    public UserService(
        AppDbContext dbContext,
        IImageService imageService,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILoggerFactory loggerFactory)
    {
        _imageService = imageService;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<UserService>();
    }


    public async Task<TaskResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: true,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            _logger.LogInformation($"Failed login user: {model.Username}: {result}.");

            return new TaskResult<AuthenticateResponse>
            {
                Succeeded = false,
                Error = result.ToString()
            };
        }

        var user = await _userManager.FindByNameAsync(model.Username);

        if (user == null)
        {
            _logger.LogInformation($"Failed login user: {model.Username}: user not found.");

            return new TaskResult<AuthenticateResponse>
            {
                Succeeded = false,
                Error = "User not found."
            };
        }

        var requestAt = DateTime.Now;
        var expiresIn = requestAt + TokenAuthOption.ExpiresSpan;

        _logger.LogInformation($"User {user.UserName} has been authenticated.");

        return new()
        {
            Succeeded = true,
            Result = new()
            {
                AccessToken = GenerateToken(expiresIn, user),
                TokenType = TokenAuthOption.TokenType
            },
        };
    }

    public async Task<TaskResult<RegisterResponse>> Register(RegisterRequest model)
    {
        var user = new User { UserName = model.Username };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation($"User {user.UserName} has been registered.");

            return new TaskResult<RegisterResponse>
            {
                Succeeded = true,
                Result = new()
            };
        }

        string error = result.ToString();

        _logger.LogInformation($"Failed register user: {user.UserName}: {error}");

        return new()
        {
            Succeeded = false,
            Error = error
        };
    }

    public async Task<TaskResult<ImageUploadResult>> UploadImage(ImageUploadRequest model, string userId)
    {
        var user = await _userManager.Users.Include(u => u.Images).FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return new()
            {
                Succeeded = false,
                Error = "User not found."
            };
        }

        var result = await _imageService.PutImage(user, model);

        if (result is { Succeeded: true, Result: not null })
        {
            user.AddImage(result.Result);
            await _userManager.UpdateAsync(user);

            return new()
            {
                Succeeded = true,
                Result = new()
                {
                    Name = result.Result.Name
                }
            };
        }

        _logger.LogError($"Failed upload image: {result.Error}");

        return new()
        {
            Succeeded = false,
            Error = "Failed upload image"
        };
    }

    private string GenerateToken(DateTime expires, User user)
    {
        var handler = new JwtSecurityTokenHandler();

        var securityToken = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = TokenAuthOption.Issuer,
            Audience = TokenAuthOption.Audience,
            SigningCredentials = new(TokenAuthOption.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName!)
            }),
            NotBefore = DateTime.Now,
            Expires = expires
        });

        return handler.WriteToken(securityToken);
    }
}