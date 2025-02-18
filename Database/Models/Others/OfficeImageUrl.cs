using Database.Models.Users;

namespace Database.Models.Others;

public class OfficeImageUrl
{
    public Ulid Id { get; set; }
    public string Url { get; set; } = default!;

    public int ServiceProviderId { get; set; }
    public ServiceProvider? ServiceProvider { get; set; } = default!;
}
