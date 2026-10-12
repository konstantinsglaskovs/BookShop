using Amazon.S3;
using Amazon.S3.Model;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace BookShop.Infrastructure.Storage;

public class S3FileStorage:IFileStorage
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3StorageOptions _options;

    public S3FileStorage(IAmazonS3 s3Client,IOptions<S3StorageOptions> options)
    {
        _s3Client = s3Client;
        _options = options.Value;
    }

    public async Task<ErrorOr<Success>> UploadAsync(Stream stream,string storageKey,string contentType)
    {
        try
        {
            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = storageKey,
                InputStream = stream,
                ContentType = contentType
            };

            await _s3Client.PutObjectAsync(request);

            return Result.Success;
        }
        catch (AmazonS3Exception ex)
        {
            return Error.Failure(
                code: "Storage.UploadFailed",
                description: ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> DeleteAsync(string storageKey)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = storageKey
            };

            await _s3Client.DeleteObjectAsync(request);

            return Result.Success;
        }
        catch (AmazonS3Exception ex)
        {
            return Error.Failure(
                code: "Storage.DeleteFailed",
                description: ex.Message);
        }
    }

    public string GetPublicUrl(string storageKey)
    {
        return $"{_options.PublicUrl.TrimEnd('/')}/{_options.BucketName}/{storageKey}";
    }
}
