using Amazon.S3;
using Amazon.S3.Model;
using ClientNexus.Application.Enums;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace ClientNexus.Application.Services;

public class FileService : IFileService
{
    private readonly IFileStorage _fileStorage;
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    public FileService(IFileStorage fileStorage , IAmazonS3 s3Client, string bucketName)
    {
        if (s3Client is null)
        {
            throw new ArgumentNullException(nameof(s3Client), "s3Client cannot be null");
        }

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            throw new ArgumentNullException(nameof(bucketName), "bucketName cannot be null or empty");
        }
        _fileStorage = fileStorage;
        _bucketName = bucketName;
        _s3Client = s3Client;


    }

    public async Task<string> UploadPublicFileAsync(
        Stream fileStream,
        FileType fileType,
        string fileName
    )
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be null or whitespace", nameof(fileName));
        }

        return await _fileStorage.UploadFileAsync(
            fileStream,
            $"{fileType.ToAbstractType()}s/{fileName}",
            fileType
        );
    }

    public async Task<string> UploadPublicFileAsync(
        Stream fileStream,
        FileType fileType,
        string fileName,
        int folderId,
        UploadedFor uploadedFor
    )
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be null or whitespace", nameof(fileName));
        }

        if (folderId <= 0)
        {
            throw new ArgumentException("Folder ID must be greater than zero", nameof(folderId));
        }

        return await _fileStorage.UploadFileAsync(
            fileStream,
            $"{fileType.ToAbstractType()}s/{uploadedFor.ToString()}/{folderId}/{fileName}",
            fileType
        );
    }

    public async Task<IEnumerable<string>> GetPublicFilesUrlsAsync(
        UploadedFileType fileType,
        int folderId,
        UploadedFor uploadedFor
    )
    {
        if (folderId <= 0)
        {
            throw new ArgumentException("Folder ID must be greater than zero", nameof(folderId));
        }

        return await _fileStorage.GetFilesUrlsWithPrefixAsync(
            $"{fileType.ToString().ToLower()}s/{uploadedFor.ToString()}/{folderId}/"
        );
    }
    public async Task DeleteFileAsync(string key)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        await _s3Client.DeleteObjectAsync(request);
    }
    public FileType GetFileType(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        switch (extension)
        {
            case ".jpg":
                return FileType.Jpg;
            case ".jpeg":
                return FileType.Jpeg;
            case ".png":
                return FileType.Png;
            default:
                throw new ArgumentException($"Unsupported file type: {extension}");
        }
    }



}
