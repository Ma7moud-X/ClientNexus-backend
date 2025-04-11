namespace ClientNexus.Domain.Enums;

public enum DistanceUnit
{
    Meters,
    Kilometers,
    Miles,
}

public static class DistanceUnitExtensions
{
    public static string ToApiString(this DistanceUnit unit)
    {
        return unit switch
        {
            DistanceUnit.Meters => "m",
            DistanceUnit.Kilometers => "km",
            DistanceUnit.Miles => "mi",
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };
    }
}
