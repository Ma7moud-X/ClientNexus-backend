using ClientNexus.Application.Domain;
using ClientNexus.Application.Enums;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.ValueObjects;

namespace ClientNexus.Application.Interfaces;

public interface ILocationService
{
    public Task<TravelDistance> GetTravelDistanceAsync(
        (double longitude, double latitude) origin,
        (double longitude, double latitude) destination,
        TravelProfile travelProfile
    );
}
