using ImageService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ImageService.Data;

public class AppDbContext : IdentityDbContext<User>
{
    private readonly IConfiguration _configuration;

    public DbSet<Image> Images { get; set; } = null!;

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().Metadata.FindNavigation(nameof(User.Images))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Entity<User>()
            .HasMany(e => e.Images)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .IsRequired();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(_configuration.GetConnectionString("Database"));
    }
}