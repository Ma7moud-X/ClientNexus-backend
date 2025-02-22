using ClientNexus.Domain.Entities.Users;

namespace ClientNexus.Domain.Entities.Services;

public class EmergencyCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public int ServiceProviderTypeId { get; set; }
    public ServiceProviderType? ServiceProviderType {get; set;}
    
    public ICollection<EmergencyCase>? EmergencyCases { get; set; }
}