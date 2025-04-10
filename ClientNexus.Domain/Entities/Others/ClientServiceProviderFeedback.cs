using ClientNexus.Domain.Entities.Users;

namespace ClientNexus.Domain.Entities.Services
{
    public class ClientServiceProviderFeedback
    {
        public int ClientId { get; set; }
        public int ServiceProviderId { get; set; }
        public float Rate { get; set; }
        public string? Feedback {get; set;}
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Client? Client { get; set; }
        public ServiceProvider? ServiceProvider { get; set; }
    }
}