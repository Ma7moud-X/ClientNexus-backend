using ClientNexus.Domain.Entities.Users;

namespace ClientNexus.Domain.Entities.Others
{
    public class Notification
    {
        public Ulid Id { get; set; } = Ulid.NewUlid();
        public required string Title { get; set; }
        public required string Body { get; set; }
        public DateTime CreatedAt = DateTime.UtcNow;

        public int BaseUserId { get; set; }
        public BaseUser? BaseUser { get; set; }
    }
}
