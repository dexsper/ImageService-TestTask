using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ImageService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Minio;
using Minio.Exceptions;

namespace ImageService.Services;

public class UserService : IUserService
{
    private const string BucketName = "photos";

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IMinioClient _minioClient;
    private readonly ILogger _logger;

    public UserService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IMinioClient minioClient,
        ILoggerFactory loggerFactory)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _minioClient = minioClient;
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
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return new()
            {
                Succeeded = false,
                Error = "User not found."
            };
        }

        try
        {
            var beArgs = new BucketExistsArgs().WithBucket(BucketName);
            bool found = await _minioClient.BucketExistsAsync(beArgs);

            if (!found)
            {
                var mbArgs = new MakeBucketArgs().WithBucket(BucketName);
                await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
            }

            await using var imageStream = model.Image.OpenReadStream();
            imageStream.Position = 0;

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(model.Image.FileName)
                .WithStreamData(imageStream)
                .WithObjectSize(imageStream.Length)
                .WithContentType(model.Image.ContentType);

            var result = await _minioClient.PutObjectAsync(putObjectArgs);

            user.AddImage(new()
            {
                Name = result.ObjectName,
                UserId = user.Id
            });
            await _userManager.UpdateAsync(user);

            return new()
            {
                Succeeded = true,
                Result = new()
                {
                    ObjectName = result.ObjectName
                }
            };
        }
        catch (MinioException exception)
        {
            _logger.LogError($"Failed upload image: {exception.Message}");

            return new()
            {
                Succeeded = false,
                Error = "Failed upload image"
            };
        }
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