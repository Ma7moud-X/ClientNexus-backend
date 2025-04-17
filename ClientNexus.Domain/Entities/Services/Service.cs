using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;

namespace ClientNexus.Domain.Entities.Services
{
    public class Service
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ServiceType ServiceType { get; set; }
        public decimal? Price { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public int? ServiceProviderId { get; set; }
        public ServiceProvider? ServiceProvider { get; set; }

        public ServicePayment? ServicePayment { get; set; }

        public Problem? Problem { get; set; }
    }
}
