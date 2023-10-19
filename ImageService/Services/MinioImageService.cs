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

    public async Task<string> PutImage(User user, ImageUploadRequest model)
    {
        try
        {
            await CreateBucket();

            await using var imageStream = model.Image.OpenReadStream();
            imageStream.Position = 0;

            string fileExt = Path.GetExtension(model.Image.FileName);
            string objectName = $"{user.Id}/{user.Images.Count}{fileExt}";

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(objectName)
                .WithStreamData(imageStream)
                .WithObjectSize(imageStream.Length)
                .WithContentType(model.Image.ContentType);

            var result = await _minioClient.PutObjectAsync(putObjectArgs);

            return new string(result.ObjectName);
        }
        catch (MinioException e)
        {
            throw new StorageException(e.Message);
        }
    }

    public async Task<List<string>> GetImages(User user)
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

            return new List<string>(images);
        }
        catch (MinioException e)
        {
            throw new Exception(e.Message);
        }
    }

    private async Task CreateBucket()
    {
        var beArgs = new BucketExistsArgs().WithBucket(BucketName);
        bool found = await _minioClient.BucketExistsAsync(beArgs);

        if (found)
            return;

        try
        {
            var mbArgs = new MakeBucketArgs().WithBucket(BucketName);
            await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
        }
        catch (MinioException e)
        {
            throw new StorageException(e.Message);
        }
    }
}