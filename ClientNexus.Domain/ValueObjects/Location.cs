namespace ClientNexus.Domain.ValueObjects;

public class Location
{
    public required double Longitude { get; init; }
    public required double Latitude { get; init; }
    public required string Identifier { get; init; }
}
