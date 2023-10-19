using ImageService.Data;
using ImageService.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageService.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IImageService _imageService;
    private readonly ILogger _logger;

    public UserService(
        AppDbContext dbContext,
        IImageService imageService,
        ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _imageService = imageService;
        _logger = loggerFactory.CreateLogger<UserService>();
    }

    public async Task<ImageUploadResponse> UploadImage(ImageUploadRequest model, string? userId)
    {
        var user = await _dbContext.Users.Include(u => u.Images).FirstOrDefaultAsync(u => u.Id == userId)
                   ?? throw new NullReferenceException("User not found.");

        var result = await _imageService.PutImage(user, model);

        _logger.LogInformation($"User {user.UserName} upload image: {result}");

        return new ImageUploadResponse(result);
    }

    public async Task<AddFriendResponse> AddFriend(AddFriendRequest model, string? userId)
    {
        var user = await _dbContext.Users.Include(u => u.Friends).FirstOrDefaultAsync(u => u.Id == userId)
                   ?? throw new NullReferenceException("User not found.");

        var friendUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == model.FriendName)
                         ?? throw new NullReferenceException("Friend not found.");

        if (user.Id == friendUser.Id)
            throw new ArgumentException("You can't add yourself.");

        if (user.Friends.Any(f => f.UserName == friendUser.UserName))
            throw new ArgumentException("User already in friends");

        user.AddFriend(friendUser);
        await _dbContext.SaveChangesAsync();

        List<string> friendList = user.Friends
            .Where(x => !string.IsNullOrEmpty(x.UserName))
            .Select(x => x.UserName!)
            .ToList();

        _logger.LogInformation($"User {user.UserName} add friend: {friendUser.UserName}");

        return new AddFriendResponse(new(friendList));
    }

    public async Task<GetImagesResponse> GetImages(GetImagesRequest model, string? userId)
    {
        var user = await _dbContext.Users.Include(u => u.Images).FirstOrDefaultAsync(u => u.Id == userId)
                   ?? throw new NullReferenceException("Friend not found.");

        User targetUser = user;

        if (model.Username != user.UserName)
        {
            var friendUser = await _dbContext.Users
                .Include(u => u.Friends)
                .Include(u => u.Images)
                .FirstOrDefaultAsync(u => u.UserName == model.Username);

            if (friendUser == null || friendUser.Friends.All(u => u.Id != user.Id))
            {
                throw new NullReferenceException("Friend not found.");
            }

            targetUser = friendUser;
        }

        var getResult = await _imageService.GetImages(targetUser);

        return new GetImagesResponse(getResult);
    }
}