using Amazon.S3;

namespace ClientNexus.API.Extensions;

public static class S3Extensions
{
    public static void AddS3Storage(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonS3>();
    }
}
