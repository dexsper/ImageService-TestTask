using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImageService.Models;

public class User : IdentityUser
{
    private readonly List<Image> _images;

    public User()
    {
        _images = new List<Image>();
    }
    
    public void AddImage(Image image)
    {
        if (!_images.Contains(image))
            _images.Add(image);
    }

    public IReadOnlyCollection<Image> Images => _images;
}

internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.
            HasMany(u => u.Images)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId)
            .IsRequired();
    }
}