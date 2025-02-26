using ClientNexus.Application.Enums;

namespace ClientNexus.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(Stream fileStream, string key, FileType fileType);
    Task<IEnumerable<string>> GetFilesUrlsWithPrefixAsync(string prefix);
}
