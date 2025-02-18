using Database.Models;

namespace Database.TypeExtensions;

public static class ReporterTypeExtensions
{
    public static char ToChar(this ReporterType type)
    {
        return (char)type;
    }

    public static ReporterType ToReporterType(this char type)
    {
        if (!Enum.IsDefined(typeof(ReporterType), (int)type))
        {
            throw new ArgumentException($"Invalid reporter type character: {type}");
        }

        return (ReporterType)type;
    }
}
