namespace ClientNexus.Application.DTO;

public record ServiceProviderEmergencyDTO : ServiceProviderServiceDTO
{
    public required string MeetingTextAddress { get; init; }
}
