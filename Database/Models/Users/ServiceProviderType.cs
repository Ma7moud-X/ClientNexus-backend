namespace Database.Models.Users;

public class ServiceProviderType
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public ICollection<ServiceProvider>? ServiceProviders { get; set; }
}
