namespace ClientNexus.Domain.Entities.Others;

public class State
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Abbreviation { get; set; }

    public int CountryId { get; set; }
    public Country? Country { get; set; }

    public ICollection<City>? Cities { get; set; }
}
