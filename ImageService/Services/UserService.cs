using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ImageService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ImageService.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger _logger;

    public UserService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILoggerFactory loggerFactory)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<UserService>();
    }


    public async Task<ITaskResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: true,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            _logger.LogInformation($"Failed login user: {model.Username}: {result}.");
            
            return new ITaskResult<AuthenticateResponse>
            {
                Succeeded = false,
                Error = result.ToString()
            };
        }

        var user = await _userManager.FindByNameAsync(model.Username);

        if (user == null)
        {
            _logger.LogInformation($"Failed login user: {model.Username}: user not found.");
            
            return new ITaskResult<AuthenticateResponse>
            {
                Succeeded = false,
                Error = "User not found."
            };
        }

        var claims = _userManager.GetClaimsAsync(user);
        var id = new ClaimsIdentity(claims.Result);

        var requestAt = DateTime.Now;
        var expiresIn = requestAt + TokenAuthOption.ExpiresSpan;

        _logger.LogInformation($"User {user.UserName} has been authenticated.");

        return new ITaskResult<AuthenticateResponse>
        {
            Succeeded = true,
            Result = new()
            {
                AccessToken = GenerateToken(expiresIn, id),
                TokenType = TokenAuthOption.TokenType
            },
        };
    }

    public async Task<ITaskResult<RegisterResponse>> Register(RegisterRequest model)
    {
        var user = new User { UserName = model.Username };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation($"User {user.UserName} has been registered.");
            
            return new ITaskResult<RegisterResponse>
            {
                Succeeded = true,
                Result = new ()
            };
        }

        string error = result.ToString();
        
        _logger.LogInformation($"Failed register user: {user.UserName}: {error}");

        return new ITaskResult<RegisterResponse>
        {
            Succeeded = false,
            Error = error
        };;
    }


    private string GenerateToken(DateTime expires, ClaimsIdentity claims)
    {
        var handler = new JwtSecurityTokenHandler();

        var securityToken = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = TokenAuthOption.Issuer,
            Audience = TokenAuthOption.Audience,
            SigningCredentials = new SigningCredentials(TokenAuthOption.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256),
            Subject = claims,
            NotBefore = DateTime.Now,
            Expires = expires
        });

        return handler.WriteToken(securityToken);
    }
}