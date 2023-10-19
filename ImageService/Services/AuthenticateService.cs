using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using ImageService.Models;
using ImageService.Schemas;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ImageService.Services;

public class AuthenticateService : IAuthenticateService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger _logger;

    public AuthenticateService(UserManager<User> userManager, SignInManager<User> signInManager,
        ILoggerFactory loggerFactory)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<AuthenticateService>();
    }


    public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: true,
            lockoutOnFailure: false);

        if (!result.Succeeded)
            throw new AuthenticationException($"Failed login user: {model.Username}: {result}.");

        var user = await _userManager.FindByNameAsync(model.Username);

        if (user == null)
            throw new NullReferenceException($"Failed login user: {model.Username}: user not found.");

        var requestAt = DateTime.Now;
        var expiresIn = requestAt + TokenAuthOption.ExpiresSpan;

        _logger.LogInformation($"User {user.UserName} has been authenticated.");

        return new AuthenticateResponse(GenerateToken(expiresIn, user), TokenAuthOption.TokenType);
    }

    public async Task<RegisterResponse> Register(RegisterRequest model)
    {
        var user = new User { UserName = model.Username };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation($"User {user.UserName} has been registered.");

            return new RegisterResponse(user.Id);
        }

        throw new AuthenticationException($"Failed register user: {user.UserName}, exception: {result}");
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