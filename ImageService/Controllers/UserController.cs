using System.Security.Claims;
using ImageService.Models;
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

    [HttpPost("auth")]
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

    [HttpPost("upload_image")]
    [Authorize]
    [ProducesResponseType(typeof(ImageUploadResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> UploadImage([FromForm] ImageUploadRequest model)
    {
        var identifierClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (identifierClaim == null)
            return Unauthorized("Failed authenticate");

        var result = await _userService.UploadImage(model, identifierClaim.Value);

        if (result.Succeeded)
        {
            return Ok(result.Result);
        }

        return BadRequest(result.Error);
    }


    [HttpGet("get_images")]
    [Authorize]
    [ProducesResponseType(typeof(GetImagesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImages()
    {
        var usernameClaim = User.FindFirst(ClaimTypes.Name);
        var identifierClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (identifierClaim == null || usernameClaim == null)
            return Unauthorized("Failed authenticate");

        var result = await _userService.GetImages(new() { Username = usernameClaim.Value }, identifierClaim.Value);

        if (result.Succeeded)
        {
            return Ok(result.Result);
        }

        return BadRequest(result.Error);
    }

    [HttpPost("friend/add")]
    [Authorize]
    [ProducesResponseType(typeof(AddFriendResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddFriend([FromBody] AddFriendRequest model)
    {
        var identifierClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (identifierClaim == null)
            return Unauthorized("Failed authenticate");

        var result = await _userService.AddFriend(model, identifierClaim.Value);

        if (result.Succeeded)
        {
            return Ok(result.Result);
        }

        return BadRequest(result.Error);
    }
    
    [HttpGet("friend/get_images")]
    [Authorize]
    [ProducesResponseType(typeof(GetImagesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFriendImage([FromBody] GetImagesRequest model)
    {
        var identifierClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (identifierClaim == null)
            return Unauthorized("Failed authenticate");

        var result = await _userService.GetImages(model, identifierClaim.Value);

        if (result.Succeeded)
        {
            return Ok(result.Result);
        }

        return BadRequest(result.Error);
    }
}