using ClientNexus.Application.Domain;
using ClientNexus.Application.Enums;
using ClientNexus.Domain.ValueObjects;

namespace ClientNexus.Application.Interfaces;

public interface ILocationService
{
    public Task<TravelDistance> GetTravelDistanceAsync(
        Location origin,
        Location destination,
        TravelProfile travelProfile
    );
}
