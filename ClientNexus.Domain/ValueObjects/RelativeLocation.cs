namespace ClientNexus.Domain.ValueObjects;

public class RelativeLocation : Location
{
    public required double Distance { get; init; }
}
