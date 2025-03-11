namespace ClientNexus.Application.DTO;

public class ServiceProviderEmergencyDTO : ServiceProviderServiceDTO
{
    public required double MeetingLongitude { get; init; }
    public required double MeetingLatitude { get; init; }
}
