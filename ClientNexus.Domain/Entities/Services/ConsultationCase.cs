namespace ClientNexus.Domain.Entities.Services
{
    public class ConsultationCase : Service
    {
        public ICollection<CaseFile>? CaseFiles { get; set; }
    }
}