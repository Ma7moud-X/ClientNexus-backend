namespace Database.Models.Users;

public class Specialization
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public int ServiceProviderTypeId { get; set; }
    public ServiceProviderType? ServiceProviderType { get; set; }

    public ICollection<ServiceProviderSpecialization>? ServiceProviderSpecializations { get; set; }
    public ICollection<ServiceProvider>? ServiceProviders { get; set; }
}
