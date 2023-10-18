using System.ComponentModel.DataAnnotations;
using ImageService.Extensions;

namespace ImageService.Models;

public record AddFriendRequest(string FriendName);

public record AuthenticateRequest([MinLength(6)] string Username, [MinLength(6)] string Password);

public record GetImagesRequest(string Username);

public record RegisterRequest([MinLength(6)] string Username, [MinLength(6)] string Password);

public record ImageUploadRequest
(
    [Required(ErrorMessage = "Image can't be null")]
    [DataType(DataType.Upload)]
    [MaxFileSize(5 * 1024 * 1024)]
    [AllowedExtensions(new[] { ".jpg", ".png" })]
    IFormFile Image
);