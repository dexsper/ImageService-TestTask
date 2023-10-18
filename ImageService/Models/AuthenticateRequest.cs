using System.ComponentModel.DataAnnotations;

namespace ImageService.Models;

public class AuthenticateRequest
{
    [MinLength(6)]
    public string Username { get; set; } = null!;

    [MinLength(6)]
    public string Password { get; set; } = null!;
}