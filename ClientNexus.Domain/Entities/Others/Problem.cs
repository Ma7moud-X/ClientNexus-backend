using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;

namespace ClientNexus.Domain.Entities
{
    public class Problem
    {
        public int Id { get; set; }

        public string Description { get; set; } = default!;
        public string? AdminComment { get; set; }

        public ProblemStatus Status { get; set; } = ProblemStatus.New;

        // Identify who reported the problem
        public ReporterType ReportedBy { get; set; }

        // Client relationship
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        // ServiceProvider relationship
        public int ServiceProviderId { get; set; }
        public ServiceProvider? ServiceProvider { get; set; }

        // Admin relationship
        public int? SolvingAdminId { get; set; }
        public Admin? SolvingAdmin { get; set; }

        // Service relationship
        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }
    }
}
