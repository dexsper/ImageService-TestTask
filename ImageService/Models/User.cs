using Microsoft.AspNetCore.Identity;

namespace ImageService.Models;

public class User : IdentityUser
{
    private readonly List<User> _friends;
    private readonly List<Image> _images;
    
    public virtual IReadOnlyCollection<Image> Images => _images;
    public virtual IReadOnlyCollection<User> Friends => _friends;

    public void AddImage(Image image)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        if (!_images.Contains(image))
            _images.Add(image);
    }

    public void AddFriend(User friend)
    {
        if (friend == null)
            throw new ArgumentNullException(nameof(friend));

        if (!_friends.Contains(friend))
            _friends.Add(friend);
    }
}