using Amazon.S3;

namespace ClientNexus.API.Extensions;

public static class S3Extensions
{
    public static void AddS3Storage(this IServiceCollection services)
    {
        try
        {
            services.AddAWSService<IAmazonS3>();
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Error while try to adding the S3 Storaga service\nPlease check the environment variables\n"
            );
            Console.WriteLine($"Error: {e.Message}");

            Environment.Exit(-1);
        }
    }
}
