namespace Database.Models.Others;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Abbreviation { get; set; }

    public ICollection<State>? States { get; set; }
    public ICollection<City>? Cities { get; set; }
}
