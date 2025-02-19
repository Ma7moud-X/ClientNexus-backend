namespace Database.Models.Others;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Abbreviation { get; set; }

    public int? StateId { get; set; }
    public State? State { get; set; }

    public int CountryId { get; set; }
    public Country? Country { get; set; }

    public ICollection<Address>? Addresses { get; set; }
}
