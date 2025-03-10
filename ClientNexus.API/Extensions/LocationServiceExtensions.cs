using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;

namespace ClientNexus.API.Extensions;

public static class LocationServiceExtensions
{
    public static void AddLocationService(this IServiceCollection services)
    {
        services.AddSingleton<ILocationService, LocationService>();
    }
}
