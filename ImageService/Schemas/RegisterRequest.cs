using System.ComponentModel.DataAnnotations;

namespace ImageService.Schemas;

public class RegisterRequest
{
    [MinLength(6)]
    public string Username { get; set; } = null!;
    
    [MinLength(6)]
    public string Password { get; set; } = null!;
}