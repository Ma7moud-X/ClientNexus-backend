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
        public int Id { get; set; }

        public string Description { get; set; } = default!;
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
        public int AdminId { get; set; }
        public Admin? Admin { get; set; }

        // Service relationship
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}
