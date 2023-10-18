using ImageService.Schemas;
using ImageService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageService.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("authenticate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] AuthenticateRequest model)
    {
        var result = await _userService.Authenticate(model);

        if (result.Succeeded)
        {
            return Ok(result.Result);
        }

        return Unauthorized(result.Error);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        var result = await _userService.Register(model);

        return result.Succeeded ? StatusCode(201, result.Result) : BadRequest(result.Error);
    }
}