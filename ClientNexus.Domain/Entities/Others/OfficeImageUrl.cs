using ClientNexus.Domain.Entities.Users;

namespace ClientNexus.Domain.Entities.Others;

public class OfficeImageUrl
{
    public int ServiceProviderId { get; set; }
    public Ulid Id { get; set; }

    public string Url { get; set; } = default!;

    public ServiceProvider? ServiceProvider { get; set; } = default!;
}
