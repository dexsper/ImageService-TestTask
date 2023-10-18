using System.Security.Claims;
using ImageService.Models;
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

    [HttpPost("auth")]
    [AllowAnonymous]
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
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        var result = await _userService.Register(model);

        if (result.Succeeded)
        {
            return Ok(result.Result);
        }

        return BadRequest(result.Error);
    }

    [HttpPost("upload_image")]
    [Authorize]
    public async Task<IActionResult> UploadImage([FromForm] ImageUploadRequest model)
    {
        var identifierClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (identifierClaim == null)
            return BadRequest("Failed authenticate");

        var result = await _userService.UploadImage(model, identifierClaim.Value);

        if (result.Succeeded)
        {
            return Ok(result.Result);
        }

        return BadRequest(result.Error);
    }
}