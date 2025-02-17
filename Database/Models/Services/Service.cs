using Database.Models.Others;
using Database.Models.Users;

namespace Database.Models.Services
{
    public enum ServiceStatus
    {
        Pending = 'P',
        InProgress = 'I',
        Done = 'D',
        Cancelled = 'C',
    }

    public class Service
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public char Status { get; set; } = (char)ServiceStatus.Pending;
        public DateTime CreatedAt { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public int? ServiceProviderId { get; set; }
        public ServiceProvider? ServiceProvider { get; set; }

        public ServicePayment? ServicePayment { get; set; }

        public Problem? Problem { get; set; }
    }
}
