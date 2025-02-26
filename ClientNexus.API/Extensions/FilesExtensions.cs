using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;
using Amazon.S3;

namespace ClientNexus.API.Extensions;

public static class FilesExtensions
{
    public static void AddFileUploadService(this IServiceCollection services)
    {
        services.AddSingleton<IFileService, S3Service>(sp =>
        {
            IAmazonS3? s3Client = sp.GetRequiredService<IAmazonS3>();
            
            string? bucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
            if (string.IsNullOrEmpty(bucketName))
            {
                Console.WriteLine("S3 Bucket Name is null or empty, cannot initialize S3FileUploadService");
                throw new InvalidOperationException("S3 Bucket Name is null or empty");
            }

            return new S3Service(s3Client, bucketName);
        });
    }
}
