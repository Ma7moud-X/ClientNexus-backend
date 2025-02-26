namespace ClientNexus.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(Stream fileStream, string key, string contentType);
    Task<IEnumerable<string>> GetFilesUrlsWithPrefixAsync(string prefix);
}
