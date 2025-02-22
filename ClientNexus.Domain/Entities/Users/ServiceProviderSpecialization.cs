namespace ClientNexus.Domain.Entities.Users;

public class ServiceProviderSpecialization
{
    public int ServiceProviderId { get; set; }
    public ServiceProvider? ServiceProvider { get; set; }
    
    public int SpecializationId { get; set; }
    public Specialization? Specialization { get; set; }
}
