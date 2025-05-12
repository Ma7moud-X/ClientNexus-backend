namespace ClientNexus.Application.DTO
{
    public record NotificationDTO
    {
        public required Ulid Id { get; init; }
        public required string Title { get; init; }
        public required string Body { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required int UserId { get; init; }
    }
}
