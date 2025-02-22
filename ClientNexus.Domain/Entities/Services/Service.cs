using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Users;

namespace ClientNexus.Domain.Entities.Services
{
    public enum ServiceStatus
    {
        Pending = 'P',
        InProgress = 'I',
        Done = 'D',
        Cancelled = 'C',
    }

    public enum ServiceType
    {
        Emergency = 'E',
        Appointment = 'A',
        Question = 'Q',
        Consultation = 'C',
    }

    public class Service
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;
        public DateTime CreatedAt { get; set; }
        public ServiceType ServiceType { get; set; }
        public decimal Price { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public int? ServiceProviderId { get; set; }
        public ServiceProvider? ServiceProvider { get; set; }

        public ServicePayment? ServicePayment { get; set; }

        public Problem? Problem { get; set; }
    }
}
