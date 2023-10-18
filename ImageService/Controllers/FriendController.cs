using ImageService.Models;
using ImageService.Services;
using Microsoft.AspNetCore.Authorization;
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
    
    [HttpPost("add")]
    [Authorize]
    [ProducesResponseType(typeof(AddFriendResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddFriend([FromBody] AddFriendRequest model)
    {
        var result = await _userService.AddFriend(model, UserId);

        if (result.Succeeded)
        {
            return Ok(result.Result);
        }

        return BadRequest(result.Error);
    }
    
    [HttpGet("get_images")]
    [Authorize]
    [ProducesResponseType(typeof(GetImagesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFriendImage([FromQuery] GetImagesRequest model)
    {
        var result = await _userService.GetImages(model, UserId);

        if (result.Succeeded)
        {
            return Ok(result.Result);
        }

        return BadRequest(result.Error);
    }
}