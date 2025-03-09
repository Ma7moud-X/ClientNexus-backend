using ClientNexus.Application.Domain;
using ClientNexus.Application.Enums;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Enums;

namespace ClientNexus.Application.Services;

class OrsRequest
{
    public List<List<double>> coordinates { get; set; } = default!;
    public string units { get; set; } = default!;
}

class OrsResponse
{
    public List<Route> routes { get; set; } = default!;
}

class Route
{
    public required Summary summary { get; set; }
}

class Summary
{
    public double distance { get; set; }
    public double duration { get; set; }
}

public class LocationService : ILocationService
{
    private readonly IHttpService _httpService;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.openrouteservice.org/v2/";
    private const string TravelDistanceEndpoint = "https://api.openrouteservice.org/v2/directions/";

    public LocationService(IHttpService httpService, string apiKey)
    {
        _httpService = httpService;
        _apiKey = apiKey;
    }

    public async Task<TravelDistance> GetTravelDistanceAsync(
        (double longitude, double latitude) origin,
        (double longitude, double latitude) destination,
        TravelProfile travelProfile,
        DistanceUnit distanceUnit = DistanceUnit.Meters
    )
    {
        var requestBody = new OrsRequest
        {
            coordinates =
            [
                [origin.longitude, origin.latitude],
                [destination.longitude, destination.latitude],
            ],
            units = distanceUnit.ToApiString(),
        };


        var orsResponse = await _httpService.SendRequestAsync<OrsResponse>(
            $"{TravelDistanceEndpoint}{travelProfile.ToApiString()}",
            HttpMethod.Post,
            [("Authorization", _apiKey)],
            requestBody
        );

        Summary summary = orsResponse.routes[0].summary;
        return new TravelDistance
        {
            Distance = (int)summary.distance,
            DistanceUnit = distanceUnit.ToApiString(),
            Duration = (int)summary.duration / 60,
            DurationUnit = "minutes",
        };
    }
}
