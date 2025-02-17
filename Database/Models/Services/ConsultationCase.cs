namespace Database.Models.Services
{
    public class ConsultationCase : Service
    {
        public ICollection<CaseFile>? CaseFiles { get; set; }
        public decimal Price { get; set; }
    }
}