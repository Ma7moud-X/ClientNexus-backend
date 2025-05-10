namespace ClientNexus.Application.DTO;

public record ServiceProviderServiceDTO
{
    public required int ServiceId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string ClientFirstName { get; init; }
    public required string ClientLastName { get; init; }
    // public required string ClientImageUrl { get; init; }
}
