using Amazon.S3;
using Amazon.S3.Model;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Infrastructure;

public class S3Storage : IFileStorage
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Storage(IAmazonS3 s3Client, string bucketName)
    {
        if (s3Client is null)
        {
            throw new ArgumentNullException(nameof(s3Client), "s3Client cannot be null");
        }

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            throw new ArgumentNullException(
                nameof(bucketName),
                "bucketName cannot be null or empty"
            );
        }

        _s3Client = s3Client;
        _bucketName = bucketName;
    }

    public async Task<IEnumerable<string>> GetFilesUrlsWithPrefixAsync(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
        {
            throw new ArgumentNullException(nameof(prefix), "prefix cannot be null or empty");
        }

        string? continuationToken = null;
        List<string> filesUrls = new List<string>();
        do
        {
            var request = new ListObjectsV2Request()
            {
                BucketName = _bucketName,
                Prefix = prefix,
                ContinuationToken = continuationToken,
            };

            ListObjectsV2Response? response;
            try
            {
                response = await _s3Client.ListObjectsV2Async(request);
            }
            catch (Exception ex)
            {
                throw new FileStorageException($"Error getting objects with prefix `{prefix}`", ex);
            }

            foreach (var obj in response.S3Objects)
            {
                filesUrls.Add($"https://{_bucketName}.s3.amazonaws.com/{obj.Key}");
            }

            continuationToken = response.IsTruncated ? response.NextContinuationToken : null;
        } while (continuationToken != null);

        return filesUrls;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string key, FileType fileType)
    {
        if (fileStream is null || !fileStream.CanRead)
        {
            throw new ArgumentException(
                "Invalid file stream. File stream can not be null and must be readable"
            );
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Invalid key. Key cannot be null or whitespace");
        }

        var putRequest = new PutObjectRequest()
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = fileType.ToMimeType(),
        };

        try
        {
            await _s3Client.PutObjectAsync(putRequest);
        }
        catch (Exception ex)
        {
            throw new FileStorageException("Error uploading file to the storage server", ex);
        }

        return $"https://{_bucketName}.s3.amazonaws.com/{key}";
    }
   
}
