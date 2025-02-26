using Amazon.S3;
using ClientNexus.Infrastructure;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.API.Extensions;

public static class S3Extensions
{
    public static void AddS3Storage(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonS3>();
        services.AddSingleton<IFileStorage, S3Storage>(sp =>
        {
            IAmazonS3? s3Client = sp.GetRequiredService<IAmazonS3>();

            string? bucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
            if (string.IsNullOrEmpty(bucketName))
            {
                Console.WriteLine("S3 Bucket Name is null or empty, cannot initialize S3FileUploadService");
                throw new InvalidOperationException("S3 Bucket Name is null or empty");
            }

            return new S3Storage(s3Client, bucketName);
        });
    }
}
