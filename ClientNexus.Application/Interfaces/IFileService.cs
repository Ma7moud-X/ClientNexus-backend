using ClientNexus.Application.Enums;
using ClientNexus.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace ClientNexus.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadPublicFileAsync(Stream fileStream, FileType fileType, string fileName);
    Task<string> UploadPublicFileAsync(
        Stream fileStream,
        FileType fileType,
        string fileName,
        int folderId,
        UploadedFor uploadedFor
    );
    Task<IEnumerable<string>> GetPublicFilesUrlsAsync(
        UploadedFileType fileType,
        int folderId,
        UploadedFor uploadedFor
    );
    Task DeleteFileAsync(string key);
    public FileType GetFileType(IFormFile file);


}
