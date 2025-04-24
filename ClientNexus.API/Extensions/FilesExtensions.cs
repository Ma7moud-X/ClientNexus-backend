using Amazon.S3;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;
using ClientNexus.Infrastructure;

namespace ClientNexus.API.Extensions;

public static class FilesExtensions
{
    public static void AddFileService(this IServiceCollection services)
    {
        //services.AddSingleton<IFileService, FileService>();
        var bucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            throw new ArgumentNullException("S3_BUCKET_NAME", "Bucket name is not set in environment variables.");
        }

        services.AddSingleton<IFileService>(provider =>
        {
            var s3Client = provider.GetRequiredService<IAmazonS3>();
            return new FileService(new S3Storage(s3Client, bucketName), s3Client, bucketName);
        });
    }
}
