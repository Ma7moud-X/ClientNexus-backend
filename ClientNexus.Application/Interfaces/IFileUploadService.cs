namespace ClientNexus.Application.Interfaces;

public interface IFileUploadService
{
    Task<string> UploadFileAsync(Stream fileStream, string key, string contentType);
}
