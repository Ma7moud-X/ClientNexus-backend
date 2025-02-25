using Amazon.S3;
using Amazon.S3.Model;
using ClientNexus.Application.Interfaces;

namespace ClientNexus.Application.Services;

public class S3FileUploadService : IFileUploadService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3FileUploadService(IAmazonS3 s3Client, string bucketName)
    {
        if (s3Client is null)
        {
            throw new ArgumentNullException(nameof(s3Client), "s3Client cannot be null");
        }

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            throw new ArgumentNullException(nameof(bucketName), "bucketName cannot be null or empty");
        }

        _s3Client = s3Client;
        _bucketName = bucketName;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string key, string contentType)
    {
        var putRequest = new PutObjectRequest()
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType
        };

        try
        {
            await _s3Client.PutObjectAsync(putRequest);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to upload file to S3: {ex.Message}", ex);
        }

        return $"https://{_bucketName}.s3.amazonaws.com/{key}";
    }
}
