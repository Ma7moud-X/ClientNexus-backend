using ClientNexus.Domain.Enums;

namespace ClientNexus.Domain.Interfaces;

public interface IFileStorage
{
    Task<string> UploadFileAsync(Stream fileStream, string key, FileType fileType);
    Task<IEnumerable<string>> GetFilesUrlsWithPrefixAsync(string prefix);

}
