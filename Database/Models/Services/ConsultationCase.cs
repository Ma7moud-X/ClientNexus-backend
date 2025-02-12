using System.Collections.Generic;
using Database.Models.Users;

namespace Database.Models.Services
{
    public class ConsultationCase : Service
    {
        public ICollection<CaseFile> CaseFiles { get; set; }

        // Foreign key
        public int ServiceProviderId { get; set; }
        // Navigation property
        public ServiceProvider ServiceProvider { get; set; }
    }
}