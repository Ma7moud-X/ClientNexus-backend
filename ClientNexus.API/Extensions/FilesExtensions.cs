using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;
using Amazon.S3;

namespace ClientNexus.API.Extensions;

public static class FilesExtensions
{
    public static void AddFileUploadService(this IServiceCollection services)
    {
        services.AddSingleton<IFileUploadService, S3FileUploadService>(sp =>
        {
            IAmazonS3? s3Client = sp.GetRequiredService<IAmazonS3>();
            if (s3Client is null)
            {
                Console.WriteLine("S3 Client is null, cannot initialize S3FileUploadService");
                Environment.Exit(-1);
            }

            string? bucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
            if (string.IsNullOrEmpty(bucketName))
            {
                Console.WriteLine("S3 Bucket Name is null or empty, cannot initialize S3FileUploadService");
                Environment.Exit(-1);
            }

            return new S3FileUploadService(s3Client, bucketName);
        });
    }
}
