using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;

namespace ClientNexus.API.Extensions;

public static class FilesExtensions
{
    public static void AddFileService(this IServiceCollection services)
    {
        services.AddSingleton<IFileService, FileService>();
    }
}
