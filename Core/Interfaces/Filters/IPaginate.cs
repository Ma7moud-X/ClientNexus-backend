namespace Core.Interfaces.Filters;

public interface IPaginate
{
    public int Offset { get; }
    public int Limit { get; }
}
