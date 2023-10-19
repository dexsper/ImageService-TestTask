using ImageService.Models;
using ImageService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageService.Controllers;

[ApiController]
[Route("user")]
public class UserController : AuthenticatedController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("add_friend")]
    [ProducesResponseType(typeof(AddFriendResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddFriend([FromBody] AddFriendRequest model)
    {
        var result = await _userService.AddFriend(model, UserId);

        return Ok(result);
    }
}