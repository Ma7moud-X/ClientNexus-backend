namespace ClientNexus.Application.Enums;

public enum TravelProfile
{
    Car = 'c',
    Bike = 'b',
    Walk = 'w',
}

public static class TravelProfileExtensions
{
    public static string ToApiString(this TravelProfile profile)
    {
        return profile switch
        {
            TravelProfile.Car => "driving-car",
            TravelProfile.Bike => "cycling-regular",
            TravelProfile.Walk => "foot-walking",
            _ => throw new ArgumentOutOfRangeException(nameof(profile), profile, null),
        };
    }
}
