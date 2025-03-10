namespace ClientNexus.Application.Enums;

public enum TravelProfile
{
    Car = 'c',
    Cycling = 'b',
    Walk = 'w',
}

public static class TravelProfileExtensions
{
    public static string ToApiString(this TravelProfile profile)
    {
        return profile switch
        {
            TravelProfile.Car => "car",
            TravelProfile.Cycling => "bike",
            TravelProfile.Walk => "foot",
            _ => throw new ArgumentOutOfRangeException(nameof(profile), profile, null),
        };
    }
}
