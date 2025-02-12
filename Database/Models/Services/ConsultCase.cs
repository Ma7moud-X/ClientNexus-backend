using System.Collections.Generic;

namespace Database.Models.Services
{
    public class ConsultCase : Service
    {
        public ICollection<CaseFile> CaseFiles { get; set; }
    }
}