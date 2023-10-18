using ImageService.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageService.Data;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
    }

    public DbSet<User> Users { get; set; }
}