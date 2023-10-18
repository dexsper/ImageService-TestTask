using System.Security.Claims;
using ImageService.Models;
using ImageService.Schemas;
using ImageService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageService.Controllers;

[ApiController]
[Route("image")]
public class ImageController : AuthenticatedController
{
    private readonly IUserService _userService;

    public ImageController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("upload")]
    [Authorize]
    [ProducesResponseType(typeof(ImageUploadResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> UploadImage([FromForm] ImageUploadRequest model)
    {
        var result = await _userService.UploadImage(model, UserId);

        if (result.Succeeded)
        {
            return Ok(result.Result);
        }

        return BadRequest(result.Error);
    }


    [HttpGet("get")]
    [Authorize]
    [ProducesResponseType(typeof(GetImagesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImages()
    {
        var result = await _userService.GetImages(new() { Username = Username }, UserId);

        if (result.Succeeded)
        {
            return Ok(result.Result);
        }

        return BadRequest(result.Error);
    }
}