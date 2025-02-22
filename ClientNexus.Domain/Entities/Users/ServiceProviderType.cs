using ClientNexus.Domain.Entities.Services;

namespace ClientNexus.Domain.Entities.Users;

public class ServiceProviderType
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public ICollection<ServiceProvider>? ServiceProviders { get; set; }
    public ICollection<Specialization>? Specializations { get; set; }
    public ICollection<EmergencyCategory>? EmergencyCategories { get; set; }
}
