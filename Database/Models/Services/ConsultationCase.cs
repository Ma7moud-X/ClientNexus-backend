using System.Collections.Generic;
using Database.Models.Users;

namespace Database.Models.Services
{
    public class ConsultationCase : Service
    {
        public ICollection<CaseFile> CaseFiles { get; set; }
        public decimal Price { get; set; }
    }
}