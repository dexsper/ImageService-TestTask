using System.Text.Json.Serialization;

namespace ImageService.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    
    [JsonIgnore]
    public string PasswordHash { get; set; } = null!;
}