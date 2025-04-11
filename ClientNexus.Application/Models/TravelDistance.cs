using ClientNexus.Domain.Enums;

namespace ClientNexus.Application.Domain;

public class TravelDistance
{
    public required int Duration { get; init; }
    public required string DurationUnit { get; init; }

    public required int Distance { get; init; }
    public required string DistanceUnit { get; init; }
}
