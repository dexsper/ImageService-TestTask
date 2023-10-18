using Microsoft.AspNetCore.Identity;

namespace ImageService.Models;

public class User : IdentityUser
{
    private readonly List<Image> _images;
    public IReadOnlyCollection<Image> Images => _images;

    public void AddImage(Image image)
    {
        if (image == null)
            throw new NullReferenceException($"Image can't be null!");

        if (!_images.Contains(image))
            _images.Add(image);
    }
}