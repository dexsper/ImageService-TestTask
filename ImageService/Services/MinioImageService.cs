using ImageService.Models;
using ImageService.Schemas;
using Minio;
using Minio.Exceptions;

namespace ImageService.Services;

public class MinioImageService : IImageService
{
    private const string BucketName = "photos";

    private readonly ILogger _logger;
    private readonly IMinioClient _minioClient;

    public MinioImageService(IMinioClient minioClient, ILoggerFactory loggerFactory)
    {
        _minioClient = minioClient;
        _logger = loggerFactory.CreateLogger<MinioImageService>();
    }

    public async Task<TaskResult<Image>> PutImage(User user, ImageUploadRequest model)
    {
        try
        {
            await CreateBucket();

            await using var imageStream = model.Image.OpenReadStream();
            imageStream.Position = 0;

            string fileExt = Path.GetExtension(model.Image.FileName);
            int imagesId = user.Images?.Count ?? 0;
            string objectName = $"{user.Id}/{imagesId}{fileExt}";

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(objectName)
                .WithStreamData(imageStream)
                .WithObjectSize(imageStream.Length)
                .WithContentType(model.Image.ContentType);

            var result = await _minioClient.PutObjectAsync(putObjectArgs);

            return new()
            {
                Succeeded = true,
                Result = new()
                {
                    Name = result.ObjectName,
                    UserId = user.Id
                }
            };
        }
        catch (MinioException e)
        {
            _logger.LogError($"Failed put image to storage: {e.Message}");
            return new()
            {
                Succeeded = false,
                Error = e.Message
            };
        }
    }

    public async Task<TaskResult<List<string>>> GetImages(User user)
    {
        try
        {
            List<string> images = new List<string>();

            foreach (var userImage in user.Images)
            {
                var image = await _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                    .WithBucket(BucketName)
                    .WithObject(userImage.Name)
                    .WithExpiry(60 * 60 * 24));

                if (!string.IsNullOrEmpty(image))
                    images.Add(image);
            }

            return new()
            {
                Succeeded = true,
                Result = images
            };
        }
        catch (MinioException e)
        {
            _logger.LogError($"Failed to fetch image from storage: {e.Message}");
            
            return new()
            {
                Succeeded = false,
                Error = e.Message
            };
        }
    }

    private async Task CreateBucket()
    {
        var beArgs = new BucketExistsArgs().WithBucket(BucketName);
        bool found = await _minioClient.BucketExistsAsync(beArgs);

        if (!found)
        {
            var mbArgs = new MakeBucketArgs().WithBucket(BucketName);
            await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
        }
    }
}