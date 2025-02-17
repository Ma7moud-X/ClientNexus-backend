using System;
using Database.Models.Others;
using Database.Models.Users;

namespace Database.Models.Services
{
    public enum ServiceStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled,
    }

    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public ServiceStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public ServicePayment? ServicePayment { get; set; }

        public Problem? Problem { get; set; }
    }
}
