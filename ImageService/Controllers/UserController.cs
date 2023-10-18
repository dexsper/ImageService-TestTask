using ImageService.Models;
using ImageService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageService.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("auth")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] AuthenticateRequest model)
    {
        var response = await _userService.Authenticate(model);

        if (response != null)
        {
            return Ok(response);
        }

        return Unauthorized();
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        var response = await _userService.Register(model);

        if (response != null)
        {
            return Ok(response);
        }

        return BadRequest();
    }
}