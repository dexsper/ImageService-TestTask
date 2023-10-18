using System.ComponentModel.DataAnnotations.Schema;

namespace ImageService.Models;

public class Image
{
    public int Id { get; set; }
    public string Name { get; set; }

    
    [ForeignKey("User")]
    public string UserId { get; set; }
    public User User { get; set; } = null!;
}