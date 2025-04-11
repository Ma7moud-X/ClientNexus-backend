using ClientNexus.Application.Domain;
using ClientNexus.Application.Enums;

namespace ClientNexus.Application.Interfaces;

public interface IMapService
{
    public Task<TravelDistance> GetTravelDistanceAsync(
        (double longitude, double latitude) origin,
        (double longitude, double latitude) destination,
        TravelProfile travelProfile
    );
}
