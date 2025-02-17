namespace Database.Models.Services
{
    public class CaseFile
    {
        public int Id { get; set; }
        public string FileUrl { get; set; } = default!;

        public int ConsultCaseId { get; set; }
        public ConsultationCase? ConsultCase { get; set; }
    }
}
