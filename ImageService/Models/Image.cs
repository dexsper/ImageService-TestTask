using System.ComponentModel.DataAnnotations.Schema;

namespace ImageService.Models;

public class Image
{
    public int Id { get; set; }
    public required string Name { get; set; }


    [ForeignKey("User")]
    public required string UserId { get; set; }
    public User User { get; set; } = null!;
}