namespace ClientNexus.Application.DTO
{
    public class ClientEmergencyDTO
    {
        public required int Id { get; init; }
        public required int TimeoutInMinutes { get; init; }
    }
}
