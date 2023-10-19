using ImageService.Models;
using ImageService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageService.Controllers;

[ApiController]
[Route("friend")]
public class FriendController : AuthenticatedController
{
    private readonly IUserService _userService;

    public FriendController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("get_images")]
    [ProducesResponseType(typeof(GetImagesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFriendImage([FromQuery] GetImagesRequest model)
    {
        var result = await _userService.GetImages(model, UserId);

        return Ok(result);
    }
}