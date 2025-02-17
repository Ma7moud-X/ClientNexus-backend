using Database.Models.Services;
using Database.Models.Users;

namespace Database.Models
{
    public enum ProblemStatus
    {
        Pending = 'P',
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
        public char Status { get; set; }

        // Identify who reported the problem
        public char ReportedBy { get; set; }

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
