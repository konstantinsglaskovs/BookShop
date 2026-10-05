namespace BookShop.Infrastructure.Storage;

public class S3StorageOptions
{
    public const string SectionName = "S3Storage";

    public string PublicUrl { get; set; } = null!;
    public string ServiceUrl { get; set; } = null!;
    public string AccessKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public string BucketName { get; set; } = null!;
}
