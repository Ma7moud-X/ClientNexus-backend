using ClientNexus.Domain.Entities.Users;

namespace ClientNexus.Domain.Entities;

public class AccessLevel
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public ICollection<Admin>? Admins { get; set; }
}
