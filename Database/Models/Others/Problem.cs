using Database.Models.Services;
using Database.Models.Users;

namespace Database.Models
{
    public enum ProblemStatus
    {
        New = 'N',
        InProgress = 'I',
        Done = 'D',
        Cancelled = 'C',
    }

    public enum ReporterType
    {
        Client = 'C',
        ServiceProvider = 'S',
    }

    public class Problem
    {
        public int ServiceProviderId { get; set; }
        public int Id { get; set; }

        public string Description { get; set; } = default!;
        public ProblemStatus Status { get; set; } = ProblemStatus.New;

        // Identify who reported the problem
        public ReporterType ReportedBy { get; set; }

        // Client relationship
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        // ServiceProvider relationship
        public ServiceProvider? ServiceProvider { get; set; }

        // Admin relationship
        public int? SolvingAdminId { get; set; }
        public Admin? SolvingAdmin { get; set; }

        // Service relationship
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}
