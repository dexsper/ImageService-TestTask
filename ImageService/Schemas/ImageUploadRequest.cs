using System.ComponentModel.DataAnnotations;
using ImageService.Extensions;

namespace ImageService.Schemas;

public class ImageUploadRequest
{
    [Required(ErrorMessage = "Image can't be null")]
    [DataType(DataType.Upload)]
    [MaxFileSize(5 * 1024 * 1024)]
    [AllowedExtensions(new[] { ".jpg", ".png" })]
    
    public IFormFile Image { get; set; } = null!;
}