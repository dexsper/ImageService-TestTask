using ImageService.Models;
using Minio;
using Minio.Exceptions;

namespace ImageService.Services;

public class MinioImageService : IImageService
{
    private const string BucketName = "photos";
    private readonly IMinioClient _minioClient;

    public MinioImageService(IMinioClient minioClient)
    {
        _minioClient = minioClient;
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