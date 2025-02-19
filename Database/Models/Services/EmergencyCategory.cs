using Database.Models.Users;

namespace Database.Models.Services;

public class EmergencyCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public int ServiceProviderTypeId { get; set; }
    public ServiceProviderType? ServiceProviderType {get; set;}
    
    public ICollection<EmergencyCase>? EmergencyCases { get; set; }
}