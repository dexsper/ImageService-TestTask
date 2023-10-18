using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ImageService.Controllers;

public class AuthenticatedController : Controller
{
    protected string UserId { get; private set; } = null!;
    protected string Username { get; private set; } = null!;

    
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var identifierClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var usernameClaim = User.FindFirst(ClaimTypes.Name);

        if (identifierClaim == null || usernameClaim == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        UserId = identifierClaim.Value;
        Username = usernameClaim.Value;

        await next();
    }
}