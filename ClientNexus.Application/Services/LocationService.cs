using ClientNexus.Application.Domain;
using ClientNexus.Application.Enums;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Enums;

namespace ClientNexus.Application.Services;

// class OrsRequest
// {
//     public List<List<double>> coordinates { get; set; } = default!;
//     public string units { get; set; } = default!;
// }

class OsrmResponse
{
    public List<Route> routes { get; set; } = default!;
}

class Route
{
    public double distance { get; set; }
    public double duration { get; set; }
}

public class LocationService : ILocationService
{
    private readonly IHttpService _httpService;
    private const string BaseUrl = "https://router.project-osrm.org/route/v1/";

    public LocationService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<TravelDistance> GetTravelDistanceAsync(
        (double longitude, double latitude) origin,
        (double longitude, double latitude) destination,
        TravelProfile travelProfile
    )
    {
        var orsResponse = await _httpService.SendRequestAsync<OsrmResponse>(
            $"{BaseUrl}{travelProfile.ToApiString()}/{origin.longitude},{origin.latitude};{destination.longitude},{destination.latitude}?overview=false",
            HttpMethod.Get
        );

        Route route = orsResponse.routes[0];
        return new TravelDistance
        {
            Distance = (int)route.distance,
            DistanceUnit = "meters",
            Duration = (int)route.duration / 60,
            DurationUnit = "minutes",
        };
    }
}
