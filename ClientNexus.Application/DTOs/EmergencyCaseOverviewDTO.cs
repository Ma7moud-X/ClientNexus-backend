using ClientNexus.Domain.Enums;

namespace ClientNexus.Application.DTOs
{
    public record EmergencyCaseOverviewDTO
    {
        public required int Id { get; init; }
        public required string Title { get; init; }
        public required string Description { get; init; }
        public required char Status { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required decimal? Price { get; init; }
        public required double MeetingLongitude { get; init; }
        public required double MeetingLatitude { get; init; }

        public required int ClientId { get; init; }
        public required int? ServiceProviderId { get; init; }
    }
}
