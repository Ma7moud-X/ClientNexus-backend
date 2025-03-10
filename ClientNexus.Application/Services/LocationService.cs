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
        if (
            origin.longitude < -180
            || origin.longitude > 180
            || origin.latitude < -90
            || origin.latitude > 90
            || destination.longitude < -180
            || destination.longitude > 180
            || destination.latitude < -90
            || destination.latitude > 90
        )
        {
            throw new ArgumentOutOfRangeException("Coordinates are out of range");
        }

        OsrmResponse? response;
        try
        {
            response = await _httpService.SendRequestAsync<OsrmResponse>(
                $"{BaseUrl}{travelProfile.ToApiString()}/{origin.longitude},{origin.latitude};{destination.longitude},{destination.latitude}?overview=false",
                HttpMethod.Get
            );
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching travel distance", ex);
        }

        if (response is null || response.routes.Count == 0)
        {
            throw new Exception("No routes found");
        }

        Route route = response.routes[0];
        return new TravelDistance
        {
            Distance = (int)route.distance,
            DistanceUnit = "meters",
            Duration = (int)route.duration / 60,
            DurationUnit = "minutes",
        };
    }
}
