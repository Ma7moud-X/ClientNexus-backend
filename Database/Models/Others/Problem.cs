using Database.Models.Users;

namespace Database.Models
{
    public enum ProblemStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }
    public enum ReporterType
    {
        Client,
        ServiceProvider
    }
    public class Problem
    {
        public string Description { get; set; }
        public ProblemStatus Status { get; set; }

        // Identify who reported the problem
        public ReporterType ReportedBy { get; set; }

        // Client relationship
        public int ClientId { get; set; }
        public Client Client { get; set; }

        // ServiceProvider relationship
        public int ServiceProviderId { get; set; }
        public ServiceProvider ServiceProvider { get; set; }

        // Admin relationship
        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }
    }
}
