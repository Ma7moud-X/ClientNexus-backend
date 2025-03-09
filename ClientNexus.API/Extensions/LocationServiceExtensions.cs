using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;

namespace ClientNexus.API.Extensions;

public static class LocationServiceExtensions
{
    public static void AddLocationService(this IServiceCollection services)
    {
        services.AddSingleton<ILocationService, LocationService>(sp =>
        {
            string? apiKey = Environment.GetEnvironmentVariable("OPEN_ROUTE_SERVICE_API");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Open Route Service API key is not set.");
            }

            var httpService = sp.GetRequiredService<IHttpService>();
            return new LocationService(httpService, apiKey);
        });
    }
}
