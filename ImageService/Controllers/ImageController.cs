using ImageService.Models;
using ImageService.Services;
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
    [ProducesResponseType(typeof(ImageUploadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadImage([FromForm] ImageUploadRequest model)
    {
        var result = await _userService.UploadImage(model, UserId);

        return Ok(result);
    }


    [HttpGet("get")]
    [ProducesResponseType(typeof(GetImagesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetImages()
    {
        var result = await _userService.GetImages(new GetImagesRequest(Username), UserId);

        return Ok(result);
    }
}