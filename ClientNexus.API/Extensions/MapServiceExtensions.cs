using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;

namespace ClientNexus.API.Extensions;

public static class MapServiceExtensions
{
    public static void AddMapService(this IServiceCollection services)
    {
        services.AddSingleton<IMapService, MapService>();
    }
}
