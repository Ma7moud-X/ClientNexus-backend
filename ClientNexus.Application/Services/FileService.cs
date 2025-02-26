using ClientNexus.Application.Enums;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services;

public class FileService : IFileService
{
    private readonly IFileStorage _fileStorage;

    public FileService(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
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
}
